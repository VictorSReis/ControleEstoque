using CommunityToolkit.Common;
using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueResources;
using ControleEstoqueUserControls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.Foundation.Collections;

namespace ControleEstoquePages;


public sealed partial class Pg_Caixa : Page
{
    #region BANCO DE DADOS
    public IControleDatabaseEstoque DbEstoqueContext;
    public IControleDatabaseCaixa DbVendasContext;
    public IList<ILayoutProduto> ListaProdutos;
    #endregion

    #region INTERNAL VALUES
    private float _TotalFechamentoVenda = 0.0f;
    private int _IdProdutoSelecionadoSuggestion = -1;
    #endregion

    public Pg_Caixa()
    {
        this.InitializeComponent();
        this.Loaded += Pg_Caixa_Loaded;
    }

    #region DESIGN EVENTS
    private void Pg_Caixa_Loaded(object sender, RoutedEventArgs e)
    {
        //CONFIGURE SHADOWN
        ConfigureShadownPage();

        //LOAD DB
        PrivateLoadDb();

    }
    #endregion

    #region EVENTOS INTERAÇÃO
    private async void Btn_AdicionarProdutoCaixaSaida_Click(object sender, RoutedEventArgs e)
    {
        if (_IdProdutoSelecionadoSuggestion < 0)
            goto Done;

        var GetProdutoIdSelecionado = 
            DbEstoqueContext.GetProduto(_IdProdutoSelecionadoSuggestion);
        if (GetProdutoIdSelecionado is null)
            goto Done;

        bool ResultProdutoJaListado =
            PrivateCheckProdutoJaListado(GetProdutoIdSelecionado.IDProduto);
        if(ResultProdutoJaListado)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync(
                $"O produto '{GetProdutoIdSelecionado.NomeProduto}' já foi adicionado!");
            
            goto Done;
        }

        var NewProdutoAdd = CreateNewSaida(GetProdutoIdSelecionado);
        ListView_ProdutosSaida.Items.Add(NewProdutoAdd);

    Done:;
        PrivateAtualizarTotalItens();
        PrivateCalcularValorTotal();
        AutoSugestionProdutoAdd.Text = string.Empty;
    }

    private void Btn_RemoverProdutosListagem_Click(object sender, RoutedEventArgs e)
    {
        _TotalFechamentoVenda = 0.0f;
        _IdProdutoSelecionadoSuggestion = -1;
        PrivateClearAllProdutos();
        PrivateAtualizarTotalItens();
        PrivateSetValueValorTotalFechamento();
    }

    private void Btn_CalcularVenda_Click(object sender, RoutedEventArgs e)
    {

    }

    private async void Btn_FecharVenda_Click(object sender, RoutedEventArgs e)
    {
        ContentDialog dialog = new ContentDialog();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = this.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "RELATÓRIO DE VENDA";
        dialog.PrimaryButtonText = "Fechar";
        dialog.DefaultButton = ContentDialogButton.Primary;
        dialog.Content = new ComprovanteVendaControleUsuario();

        var result = await dialog.ShowAsync();

    }

    private void AutoSugestionProdutoAdd_SuggestionChosen
        (AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        var SelectedItem = args.SelectedItem;
        if (SelectedItem is null)
            return;
        if (SelectedItem is not string)
            return;

        string Texto = SelectedItem as string;

        //GET ID PRODUTO
        try
        {
            _IdProdutoSelecionadoSuggestion = 
                int.Parse((Texto.Split('-')[0]).Trim());
        }
        catch (System.Exception)
        {

        }

        AutoSugestionProdutoAdd.Text = SelectedItem.ToString();
    }

    private void AutoSugestionProdutoAdd_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // Since selecting an item will also change the text,
        // only listen to changes caused by user entering text.
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (ListaProdutos is null)
                return;
            if (ListaProdutos.Count == 0)
                return;

            var suitableItems = new List<string>();
            var splitText = sender.Text.ToLower().Split(" ");
            string StringTexto = string.Empty;
            foreach (var ProdutoInDb in ListaProdutos)
            {
                var found = splitText.All((key) =>
                {
                    if(key.IsNumeric())
                        return ProdutoInDb.IDProduto.ToString().Contains(key);
                    else
                        return ProdutoInDb.NomeProduto.ToLower().Contains(key);
                });
                if (found)
                {
                    StringTexto = $"{ProdutoInDb.IDProduto} - {ProdutoInDb.NomeProduto}";
                    suitableItems.Add(StringTexto);
                }
            }
            if (suitableItems.Count == 0)
            {
                suitableItems.Add("Nenhum produto encontrado");
            }
            sender.ItemsSource = suitableItems;
        }

    }

    private async void NewItemSaida_OnSemEstoqueQtd(object sender, int e)
    {
        await SharedResourcesApp._MessageBox.ShowMessageAsync("A quantidade informada não é válida! \n\r " +
            "O valor deve ser igual ou menor do que o disponível no estoque");
    }
    #endregion

    #region PRIVATE
    private void ConfigureShadownPage()
    {
        GridPainelSuperior.Translation = new System.Numerics.Vector3(0, 0, 10);
        GridAreaConteudo.Translation = new System.Numerics.Vector3(0, 0, 20);
        GridBarraSuperiorAddProduto.Translation = new System.Numerics.Vector3(0, 0, 10);
    }

    private void PrivateLoadDb()
    {
        //LOAD DB
        DbEstoqueContext = SharedResourcesDatabase.DatabaseEstoque;
        DbVendasContext = SharedResourcesDatabase.DatabaseCaixa;

        //LOAD LIST DB
        ListaProdutos = DbEstoqueContext.GetProdutos().ToList();
    }

    private void PrivateClearAllProdutos()
    {
        ListView_ProdutosSaida.Items.Clear();
    }

    private bool PrivateCheckProdutoJaListado(int pIdProduto)
    {
        bool Result =
            ListView_ProdutosSaida.Items.OfType<ProdutoCaixaSaidaControleUsuario>().Any
            (x => x.IdProduto == pIdProduto);
        if (Result)
            return true;
        else
            return false;
    }

    private void PrivateCalcularValorTotal()
    {
        try
        {
            _TotalFechamentoVenda = 0.0f;

            foreach (var item in ListView_ProdutosSaida.Items)
            {
                var Produto =
                    PrivateCovnertObjectProduto(item);
                if (Produto is null)
                    continue;

                PrivateIncrementarValorTotalFechamento(
                    Produto.ObterValorTotal());
            }
        }
        catch (System.Exception)
        {

        }
    }

    private ProdutoCaixaSaidaControleUsuario CreateNewSaida
        (ILayoutProduto pProduto)
    {
        var NewItemSaida = new ProdutoCaixaSaidaControleUsuario();
        NewItemSaida.LoadProdutoInfo(
            pProduto.IDProduto,
            pProduto.NomeProduto,
            pProduto.CustoVenda,
            pProduto.EstoqueProduto);
        NewItemSaida.OnSemEstoqueQtd += NewItemSaida_OnSemEstoqueQtd;

        return NewItemSaida;
    }

    private ProdutoCaixaSaidaControleUsuario PrivateCovnertObjectProduto(object pObjectProduto)
    {
        if (pObjectProduto is null)
            return null;
        if (pObjectProduto is not ProdutoCaixaSaidaControleUsuario)
            return null;

        return (pObjectProduto as ProdutoCaixaSaidaControleUsuario);
    }

    private void PrivateIncrementarValorTotalFechamento(float pValue)
    {
        _TotalFechamentoVenda += pValue;
        PrivateSetValueValorTotalFechamento();
    }

    private void PrivateSetValueValorTotalFechamento()
    {
        try
        {
            TextBlock_ValorTotalAPagar.Text = 
                _TotalFechamentoVenda.ToString("C2", CultureInfo.CurrentCulture);

        }
        catch (System.Exception)
        {

        }
    }

    private void PrivateAtualizarTotalItens()
    {
        try
        {
            TextBlock_QuantidadeTotalProdutosSaida.Text = 
                ListView_ProdutosSaida.Items.Count.ToString("D3");
        }
        catch (System.Exception)
        {


        }
    }
    #endregion
}
