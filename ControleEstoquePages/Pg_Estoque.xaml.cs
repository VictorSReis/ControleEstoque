using ControleEstoqueCore.Database;
using ControleEstoqueDataGrid;
using ControleEstoqueResources;
using ControleEstoqueUserControls;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoquePages;


public sealed partial class Pg_Estoque : Page
{
    #region PRIVATE
    private IControleDatabaseEstoque _DbEstoque;
    #endregion

    public Pg_Estoque()
    {
        this.InitializeComponent();
        this.Loaded += Pg_Estoque_Loaded;
        this.Unloaded += Pg_Estoque_Unloaded;
        
    }

    #region DESIGN EVENTS
    private void Pg_Estoque_Loaded(object sender, RoutedEventArgs e)
    {
        //CONFIGURE SHADOWNS
        ConfigureShadownPage();

        //SET DB
        PrivateSetDatabase();
    }

    private void Pg_Estoque_Unloaded(object sender, RoutedEventArgs e)
    {
        DataGridControle.ClearAll();
    }
    #endregion

    #region EVENTOS DE INTERAÇÃO
    private void Btn_UpdateListDb_Click(object sender, RoutedEventArgs e)
    {
        PrivateLoadProdutosView(null);
    }

    private async void Btn_AddNewItemDb_Click(object sender, RoutedEventArgs e)
    {
        var NovoProduto = await PrivateShowCadastroNovoProduto();
        if (NovoProduto is null)
            goto Done;

        var ResultValidate = await PrivateValidarNovoProduto(NovoProduto);
        if (!ResultValidate)
            goto Done;

        var ResultCadastroInDb = PrivateCadastroProdutoDb(NovoProduto);
        if (ResultCadastroInDb)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync($"O produto '{NovoProduto.NomeProduto}' foi cadastrado com sucesso!");
        }
        else
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync("Ocorreu uma falha interna ao tentar cadastrar o produto!");
            goto Done;
        }


        //UPDATE DATA GRID
        PrivateLoadProdutosView(null);


        Done:;
    }

    private void Btn_Confirm_Delete_Produto_Click(object sender, RoutedEventArgs e)
    {
        DataGridControle.DeletarProdutoRowSelecionada();
    }

    private void Btn_Cancel_Delete_Produto_Click(object sender, RoutedEventArgs e)
    {
        //Usuário cancelou a operação.
    }

    private void Btn_SaveDb_Click(object sender, RoutedEventArgs e)
    {
        DataGridControle.SaveDb();
    }

    private void Btn_SearchInDbPesquisa_Click(object sender, RoutedEventArgs e)
    {
        PrivateSearchItemInList();
    }

    private void Txb_TextoPesquisaProduto_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if(e.Key == Windows.System.VirtualKey.Enter)
        {
            Txb_TextoPesquisaProduto.IsEnabled = false;
            PrivateSearchItemInList();
            Txb_TextoPesquisaProduto.IsEnabled = true;
        }
    }
    #endregion

    #region PRIVATE
    private void ConfigureShadownPage()
    {
        GridPainelSuperior.Translation = new System.Numerics.Vector3(0, 0, 10);
        GridAreaConteudo.Translation = new System.Numerics.Vector3(0, 0, 20);
    }

    private void PrivateSetDatabase()
    {
        _DbEstoque = SharedResourcesDatabase.DatabaseEstoque;
        DataGridControle.LoadDb(_DbEstoque);
    }

    private void PrivateSearchItemInList()
    {
        string TextoPesquisa = Txb_TextoPesquisaProduto.Text;
        if (string.IsNullOrEmpty(TextoPesquisa))
            return;
        if (TextoPesquisa.Count() < 2)
            return;

        PrivateLoadProdutosView(x => x.NomeProduto.Contains(TextoPesquisa, StringComparison.OrdinalIgnoreCase));
    }

    private void PrivateLoadProdutosView
        (Func<ILayoutProduto, bool> pLoadDbProdutosByFunc)
    {
        DataGridControle.ClearAll();      
        DataGridControle.PopulateDataGrid(pLoadDbProdutosByFunc);
    }

    private async Task<ILayoutProduto> PrivateShowCadastroNovoProduto()
    {
        ILayoutProduto NovoProduto = default;

        PgContentDialogAddNewProdutoStyle ct = new();
        ContentDialog CtDialog = new();
        CtDialog.XamlRoot = this.XamlRoot;
        CtDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        CtDialog.Title = "CADASTRO DE NOVO PRODUTO---";
        CtDialog.Content = ct;
        CtDialog.PrimaryButtonText = "Cadastrar";
        CtDialog.SecondaryButtonText = "Cancel";
        ContentDialogResult ResultDialog = await CtDialog.ShowAsync();
        if(ResultDialog == ContentDialogResult.Secondary)
        {
            //USER CANCELED
            goto Done;
        }

        try
        {
            NovoProduto = ct.ObterRegistroProduto();
        }
        catch (Exception)
        {

        }

    Done:;
        return NovoProduto;
    }

    private async Task<bool> PrivateValidarNovoProduto(ILayoutProduto pProduto)
    {
        bool ValidInterfaceData = pProduto.ValidarProduto();
        if (!ValidInterfaceData)
            return false;

        //VALIDA O ID
        bool ProdutoIdExiste = _DbEstoque.ProdutoExiste(pProduto.IDProduto);
        if(ProdutoIdExiste)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync
                ($"O produto '{pProduto.NomeProduto}' com o ID '{pProduto.IDProduto}' já existia no banco de dados.\n\r" +
                $"O ID deve ser único para cada produto!");
            return false;
        }

        return true;
    }

    private bool PrivateCadastroProdutoDb(ILayoutProduto pProduto)
    {
        return _DbEstoque.AdicionarProduto(pProduto);
    }
    #endregion
}
