using CommunityToolkit.Common;
using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueResources;
using ControleEstoqueUserControls;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using Windows.Devices.AllJoyn;
using Windows.Foundation.Collections;

namespace ControleEstoquePages;


public sealed partial class Pg_Caixa : Page
{
    #region BANCO DE DADOS
    public IControleDatabaseEstoque DbEstoqueContext;
    public IControleDatabaseCaixa DbCaixaContext;
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

        //Carrega as ultimas vendas
        PrivateLoadLastVendas();

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
        ListView_CaixaSaida.Items.Add(NewProdutoAdd);

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

    private async void Btn_FecharVenda_Click(object sender, RoutedEventArgs e)
    {
        if (!ListView_CaixaSaida.Items.Any())
            return;

        List<ILayoutProdutoVendido> ListaProdutosVendidos = new(ListView_CaixaSaida.Items.Count);
        ILayoutCaixa NewCaixa = new LayoutCaixa
        {
            Data = DateTime.Now.ToShortDateString(),
            IID = PrivateCreateNewIIDCaixa(),
        };

        //OBTÉM OS ITEMS
        List<ProdutoCaixaSaidaControleUsuario> CaixaSaidaLista = new(ListView_CaixaSaida.Items.Count);
        foreach (var ItemSaida in ListView_CaixaSaida.Items.OfType<ProdutoCaixaSaidaControleUsuario>())
        {
            if (ItemSaida.EstoqueDisponivel())
                CaixaSaidaLista.Add(ItemSaida);
            else
                await SharedResourcesApp._MessageBox.ShowMessageAsync($"O produto '{ItemSaida.NomeProduto}' está fora de estoque!");
        }

        if (!CaixaSaidaLista.Any())
            goto Done;

        //PROCESSA A SAIDA
        foreach (var ItemSaida in CaixaSaidaLista)
        {
            //CRIA O ITEM DE SAIDA
            ILayoutProdutoVendido NewProdutoSaida = new LayoutProdutoVendido
            {
                IDProduto = ItemSaida.IdProduto,
                NomeProduto = ItemSaida.NomeProduto,
                QuantidadeVendidos = ItemSaida.ObterQuantidadeItemsVendidos(),
                ValorVenda = ItemSaida.ObterValorTotal()
            };

            //ADICIONA NA LISTA DE ITENS VENDIDOS PARA O RELATORIO DE CAIXA
            ListaProdutosVendidos.Add(NewProdutoSaida);

            //DA BAIXA NA QUANTIDADE DO PRODUTO NO ESTOQUE
            PrivateDarBaixaEstoque(
                ItemSaida.IdProduto,
                ItemSaida.ObterQuantidadeItemsVendidos());
        }

        //SERIALIZA OS ITENS VENDIDOS PARA O OBJETO DE DADOS
        NewCaixa.Objeto = PrivateSerializarListaItems(ListaProdutosVendidos);

        //ADICIONA UM NOVA ENTRADA NO CAIXA
        PrivateAdicionarNovaSaida(NewCaixa);

        //MESSAGE
        await SharedResourcesApp._MessageBox.ShowMessageAsync($"Venda fechada com sucesso!\nVENDA#{NewCaixa.IID}");

        //UPDATE ULTIMAS VENDAS
        PrivateLoadLastVendas();

    Done:;
        //LIMPA TUDO
        PrivateClearAllProdutos();
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
        DbCaixaContext = SharedResourcesDatabase.DatabaseCaixa;

        //LOAD LIST DB
        ListaProdutos = DbEstoqueContext.GetProdutos().ToList();
    }

    private void PrivateClearAllProdutos()
    {
        ListView_CaixaSaida.Items.Clear();
    }

    private bool PrivateCheckProdutoJaListado(int pIdProduto)
    {
        bool Result =
            ListView_CaixaSaida.Items.OfType<ProdutoCaixaSaidaControleUsuario>().Any
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

            foreach (var item in ListView_CaixaSaida.Items)
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
                ListView_CaixaSaida.Items.Count.ToString("D3");
        }
        catch (System.Exception)
        {


        }
    }
    #endregion

    #region PRIVATE PROCESSAMENTO DE SAIDA
    private void PrivateDarBaixaEstoque
        (int pIdProduto, int pQuantidadeVendidos)
    {
        var ProdutoBaixa = DbEstoqueContext.GetProduto(pIdProduto);
        if(ProdutoBaixa is null)
            throw new NullReferenceException("Falhou ao tentar obter o produto a da baixa!");

        ProdutoBaixa.EstoqueProduto -= pQuantidadeVendidos;
        bool Result = DbEstoqueContext.UpdateProduto(ProdutoBaixa);
        if (!Result)
            throw new Exception("Falhou ao dar baixa no estoque!");

        DbEstoqueContext.SaveChanges();
    }

    private void PrivateAdicionarNovaSaida(ILayoutCaixa pCaixaSaida)
    {
        DbCaixaContext.AdicionarSaida(pCaixaSaida);
        DbCaixaContext.SaveChanges();
    }

    private byte[] PrivateSerializarListaItems(List<ILayoutProdutoVendido> pListaProdutosVendidos)
    {
        return JsonSerializer.SerializeToUtf8Bytes(pListaProdutosVendidos);
    }

    private string PrivateCreateNewIIDCaixa()
    {
        byte[] NumberIID = new byte[4];
        string IIDText;
        using (var Rng = RandomNumberGenerator.Create())
        {
            while(true)
            {
                Rng.GetBytes(NumberIID);
                BigInteger Bi = new BigInteger(NumberIID);
                if (Bi < 0)
                    continue;

                IIDText = Bi.ToString();
                break;
            }         
        }

        return IIDText;
    }
    #endregion

    #region PRIVATE - CARREGAR ULTIMAS VENDAS
    private async void PrivateLoadLastVendas()
    {
        //CARREGA AS ULTIMAS 5 VENDAS

        //LIMPA TODOS OS ITENS
        ListView_UltimasVendas.Items.Clear();

        //OBTÉM A QUANTIDADE DE ITENS NO DB
        int CountItems = await DbCaixaContext.GetCount();
        if (CountItems <= 0)
            return;

        var ListCaixa = DbCaixaContext.GetCaixa().ToList();

        if(CountItems <= 5)
        {
            foreach (var item in ListCaixa)
            {
                var NewItem = PrivateCreateAndLoadRelatorioObjeto(item);
                ListView_UltimasVendas.Items.Add(NewItem);
            }
        }
        else
        {
            for (int i = (CountItems-1); i >= (CountItems - 5); i--)
            {
                var Item = ListCaixa[i];

                var NewItem = PrivateCreateAndLoadRelatorioObjeto(Item);
                ListView_UltimasVendas.Items.Add(NewItem);
            }
        }
    }

    private RelatorioVendaItemControleUsuario PrivateCreateAndLoadRelatorioObjeto(ILayoutCaixa pDados)
    {
        var NewRelatorio = new RelatorioVendaItemControleUsuario();
        NewRelatorio.SetContentDialogCreator(SharedResourcesApp._DialogCreator);
        NewRelatorio.LoadCaixaItem(
            pDados,
            JsonSerializer.Deserialize<List<LayoutProdutoVendido>>(pDados.Objeto).OfType<ILayoutProdutoVendido>().ToList());
        return NewRelatorio;
    }
    #endregion
}
