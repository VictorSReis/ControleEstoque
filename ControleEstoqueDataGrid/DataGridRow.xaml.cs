using ControleEstoqueCore.Database;
using ControleEstoqueResources;
using ControleEstoqueSDK;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;


namespace ControleEstoqueDataGrid;

public sealed partial class DataGridRow : UserControl
{
    #region PROPRIEDADES
    /// <summary>
    /// Um identificador para a linha atual da DataGrid.
    /// </summary>
    public Guid GuidRow { get; }

    /// <summary>
    /// Verifica se a linha atual está selecionada
    /// </summary>
    public bool ItemIsSelected { get; private set; }

    /// <summary>
    /// Obtém o produto definido nesta linha
    /// </summary>
    public ILayoutProduto Produto { get => InternalProduto; private set => InternalProduto = value; }

    internal ILayoutProduto InternalProduto;
    #endregion

    #region EVENTOS
    /// <summary>
    /// Evento de notificação de alteração em um determinado valor do produto.
    /// </summary>
    public event EventHandler<ILayoutProduto> ProdutoChanged;

    /// <summary>
    /// Evento de notificação de seleção da Row atual.
    /// </summary>
    public event EventHandler<bool> RowSelectChanged;
    #endregion

    #region NOME CONTROLES EDITAVEIS
    internal string TextBlock_Name_IDProduto;
    internal string TextBox_Name_NomeProduto;
    internal string TextBox_Name_CustoProduto;
    internal string TextBox_Name_PrecoVendaProduto;
    internal string TextBox_Name_ValidadeProduto;
    internal string TextBox_Name_QtdEstoqueProduto;
    #endregion

    #region INTERNAL
    private UserInterationSaveChangeContent TipoInteracaoParaSalvar = 
        UserInterationSaveChangeContent.Unknown;
    #endregion

    public DataGridRow()
    {
        this.InitializeComponent();
        this.Loaded += DataGridRow_Loaded;
        GuidRow = Guid.NewGuid();
    }


    #region EVENTOS DE INTEREÇÃO
    private async void Interation_Editable_RowContent_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            bool ElementEditable = PrivateFrameworkElementIsEditableTextBox(sender);
            if (!ElementEditable)
                return;

            var TxbContent = PrivateGetTextBox(sender);
            bool ConteudoRowDiferenteProduto = PrivateValidateChangedContent
                (TxbContent.Tag as string, TxbContent.Text);
            if (!ConteudoRowDiferenteProduto)
                return;

            //Verifica se deseja salvar os dados ou não.
            var ResultSave = await PrivateShowAlteracaoDesejaSalvar(sender);
            if (ResultSave == UserInterationSaveChangeContent.Salvar)
            {
                //UPDATE PRODUTO INSTANCE
                PrivateUpdateProdutoInstance(TxbContent);

                //CARREGA NOVAMENTE OS DADOS PARA OS CAMPOS
                PrivateLoadInfoProduto();

                //SEND NOTIFY FOR UPDATE
                PrivateNotifyProdutoChanged();
            }
            else if (ResultSave == UserInterationSaveChangeContent.Fechar)
            {

            }
            else
            {
                //RESET TEXT VALUE
                TxbContent.Text = TxbContent.Tag as string;
                TxbContent.UpdateLayout();
            }

        }
    }

    private void Btn_TeachingTip_Salvar_Click(object sender, RoutedEventArgs e)
    {
        //O USUÁRIO DESEJA SALVAR.
        TipoInteracaoParaSalvar = UserInterationSaveChangeContent.Salvar;
    }

    private void Btn_TeachingTip_Fechar_Click(object sender, RoutedEventArgs e)
    {
        //O USUÁRIO DESEJA MANTER A ALTERAÇÃO MAIS NÃO ENVIAR PARA O DB AGORA.
        TipoInteracaoParaSalvar = UserInterationSaveChangeContent.Fechar;
    }

    private void Btn_TeachingTip_Cancelar_Click(object sender, RoutedEventArgs e)
    {
        //O USUÁRIO DESEJA CANCELAR
        TipoInteracaoParaSalvar = UserInterationSaveChangeContent.Cancelar;
    }
    #endregion

    #region EVENTOS DE DESIGN
    private void DataGridRow_Loaded(object sender, RoutedEventArgs e)
    {
        //SET NAME CONTROLES
        TextBlock_Name_IDProduto = TextBlock_IdProduto.Name;
        TextBox_Name_NomeProduto = TextBox_NomeProduto.Name;
        TextBox_Name_CustoProduto = TextBox_CustoProduto.Name;
        TextBox_Name_PrecoVendaProduto = TextBox_PrecoVendaProduto.Name;
        TextBox_Name_ValidadeProduto = TextBox_ValidadeProduto.Name;
        TextBox_Name_QtdEstoqueProduto = TextBox_QtdEstoqueProduto.Name;
    }

    private void GridPrincipal_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (!ItemIsSelected)
            GridPrincipal.Background = new
                SolidColorBrush(Util.CreateColorFromHex("0xFFE3E3E3"));
    }

    private void GridPrincipal_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!ItemIsSelected)
            GridPrincipal.Background = new
                SolidColorBrush(Util.CreateColorFromHex("0xFFF0F0F0"));
    }

    private void GridPrincipal_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ItemIsSelected)
        {
            PrivateDeselectRow();
            RowSelectChanged?.Invoke(this, false);
        }
        else
        {
            PrivateSelectRow();
            RowSelectChanged?.Invoke(this, true);
        }
    }
    #endregion

    #region MÉTODOS PUBLICOS
    public void LoadProduto
        (ILayoutProduto pProduto)
    {
        Produto = pProduto;
        PrivateLoadInfoProduto();
    }

    public ILayoutProduto GetProduto()
        => Produto;

    public void ForceSaveAll()
    {
        bool ItemUpdated = false;
        List<TextBox> ListaForUpdate = new(5)
        {
            TextBox_NomeProduto,
            TextBox_CustoProduto,
            TextBox_PrecoVendaProduto,
            TextBox_ValidadeProduto,
            TextBox_QtdEstoqueProduto
        };

        foreach (var ItemForCheckUpdate in ListaForUpdate)
        {
            bool ConteudoRowDiferenteProduto = PrivateValidateChangedContent
                (ItemForCheckUpdate.Tag as string, ItemForCheckUpdate.Text);
            
            if (!ConteudoRowDiferenteProduto)
                continue;

            PrivateUpdateProdutoInstance(ItemForCheckUpdate);
            ItemUpdated = true;
        }

        if(ItemUpdated)
        {
            PrivateLoadInfoProduto();
            PrivateNotifyProdutoChanged();
        }
    }

    public bool CheckIsSelected()
        => ItemIsSelected;

    public void SelectRow()
    {
        PrivateSelectRow();
    }

    public void DeselectRow()
    {
        PrivateDeselectRow();
    }
    #endregion


    #region PRIVATE
    private void PrivateSelectRow()
    {
        GridPrincipal.Background = new
               SolidColorBrush(Util.CreateColorFromHex("0xFFD2DFED"));
        ItemIsSelected = true;
    }

    private void PrivateDeselectRow()
    {
        ItemIsSelected = false;
        GridPrincipal.Background = new
            SolidColorBrush(Util.CreateColorFromHex("0xFFF0F0F0"));
    }

    private bool PrivateUpdateProdutoInstance
        (TextBox pTextBox)
    {
        //ID DO PRODUTO NÃO É EDITÁVEL
        try
        {
            if (string.Equals(pTextBox.Name, TextBox_Name_NomeProduto))
            {
                Produto.NomeProduto = pTextBox.Text;
            }
            else if (string.Equals(pTextBox.Name, TextBox_Name_CustoProduto))
            {
                Produto.CustoProduto = float.Parse(pTextBox.Text, CultureInfo.CurrentCulture);
            }
            else if (string.Equals(pTextBox.Name, TextBox_Name_PrecoVendaProduto))
            {
                Produto.CustoVenda = float.Parse(pTextBox.Text, CultureInfo.CurrentCulture);
            }
            else if (string.Equals(pTextBox.Name, TextBox_Name_ValidadeProduto))
            {
                Produto.ValidadeProduto = pTextBox.Text;
            }
            else if (string.Equals(pTextBox.Name, TextBox_Name_QtdEstoqueProduto))
            {
                Produto.EstoqueProduto = int.Parse(pTextBox.Text, CultureInfo.InvariantCulture.NumberFormat);
            }
            else
            {
                throw new NotSupportedException($"O tipo 'TextBox' informado no parametro {nameof(pTextBox)} com o nome do controle definido para" +
                    $"({pTextBox.Name}) não existia no cadastro dos nomes de controle editaveis!");
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void PrivateLoadInfoProduto()
    {
        //LOAD INFORMATION
        TextBlock_IdProduto.Text = Produto.IDProduto.ToString();
        TextBox_NomeProduto.Text = Produto.NomeProduto;
        TextBox_CustoProduto.Text = $"{Produto.CustoProduto.ToString("C2", CultureInfo.CurrentCulture)}";
        TextBox_PrecoVendaProduto.Text = $"{Produto.CustoVenda.ToString("C2", CultureInfo.CurrentCulture)}";
        TextBox_ValidadeProduto.Text = Produto.ValidadeProduto;
        TextBox_QtdEstoqueProduto.Text = Produto.EstoqueProduto.ToString();

        //SET TAG DATA EDITABLE CONTENT
        TextBox_NomeProduto.Tag = Produto.NomeProduto;
        TextBox_CustoProduto.Tag = $"{Produto.CustoProduto.ToString("C2", CultureInfo.CurrentCulture)}";
        TextBox_PrecoVendaProduto.Tag = $"{Produto.CustoVenda.ToString("C2", CultureInfo.CurrentCulture)}";
        TextBox_ValidadeProduto.Tag = Produto.ValidadeProduto;
        TextBox_QtdEstoqueProduto.Tag = Produto.EstoqueProduto.ToString();

        //UPDATE TOOL TIP
        PrivateUpdateToolTipElements();
    }

    private void PrivateUpdateToolTipElements()
    {
        ToolTipService.SetToolTip(TextBox_NomeProduto, TextBox_NomeProduto.Text);
        ToolTipService.SetToolTip(TextBox_CustoProduto, TextBox_CustoProduto.Text);
        ToolTipService.SetToolTip(TextBox_PrecoVendaProduto, TextBox_PrecoVendaProduto.Text);
        ToolTipService.SetToolTip(TextBox_ValidadeProduto, TextBox_ValidadeProduto.Text);
        ToolTipService.SetToolTip(TextBox_QtdEstoqueProduto, TextBox_QtdEstoqueProduto.Text);
    }

    private void PrivateNotifyProdutoChanged()
    {
        ProdutoChanged?.Invoke(this, Produto);
    }

    private async Task<UserInterationSaveChangeContent> PrivateShowAlteracaoDesejaSalvar
        (object pFrameworkElement)
    {
        UserInterationSaveChangeContent Result = UserInterationSaveChangeContent.Unknown;

        TeachingTip_ShowMessageSaveAlteracaoProduto.Target = pFrameworkElement as FrameworkElement;
        TeachingTip_ShowMessageSaveAlteracaoProduto.IsOpen = true;

        try
        {
            //AGUARDA O RESULTADO
            while (true)
            {
                if (TipoInteracaoParaSalvar == UserInterationSaveChangeContent.Unknown)
                    goto Done;

                switch (TipoInteracaoParaSalvar)
                {
                    case UserInterationSaveChangeContent.Salvar:
                        Result = UserInterationSaveChangeContent.Salvar;
                        break;

                    case UserInterationSaveChangeContent.Fechar:
                        Result = UserInterationSaveChangeContent.Fechar;
                        break;

                    case UserInterationSaveChangeContent.Cancelar:
                        Result = UserInterationSaveChangeContent.Cancelar;
                        break;

                    case UserInterationSaveChangeContent.Unknown:
                        Result = UserInterationSaveChangeContent.Unknown;
                        break;

                    default:
                        break;
                }

                //O USUÁRIO INTERAGIU COM O CONTROLE
                break;

            Done:;
                await Task.Delay(10);
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            //RESET
            TipoInteracaoParaSalvar = UserInterationSaveChangeContent.Unknown;

            //HIDE PAINEL
            TeachingTip_ShowMessageSaveAlteracaoProduto.IsOpen = false;
            TeachingTip_ShowMessageSaveAlteracaoProduto.Target = null;
        }

        return Result;
    }

    private static bool PrivateValidateChangedContent
        (string pProdutoValue, string pRowValue)
    {
        return !string.Equals(pProdutoValue, pRowValue, StringComparison.Ordinal);
    }

    private static bool PrivateFrameworkElementIsEditableTextBox
        (object pFrameworkElement)
    {
        if (pFrameworkElement is null)
            return false;
        if (pFrameworkElement is TextBox)
            return true;

        return false;
    }

    private static TextBox PrivateGetTextBox
        (object pFrameworkElement)
        => pFrameworkElement as TextBox;
    #endregion

    private void Interation_Editable_Content_TextChanged(object sender, TextChangedEventArgs e)
    {
        //UPDATE PRODUTO INSTANCE
        bool ResultUpdate = PrivateUpdateProdutoInstance(sender as TextBox);
        if (!ResultUpdate)
            return;

        //CARREGA NOVAMENTE OS DADOS PARA OS CAMPOS
        //PrivateLoadInfoProduto();
        PrivateUpdateToolTipElements();

        //SEND NOTIFY FOR UPDATE
        PrivateNotifyProdutoChanged();
    }
}
