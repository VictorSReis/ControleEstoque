using ControleEstoqueCore;
using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueResources;
using ControleEstoqueSDK;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControleEstoqueDataGrid;

public sealed partial class DataGridControleEstoque : UserControl
{
    #region PROPRIEDAES
    private IControleDatabaseEstoque DbEstoque { get; set; }
    private int CountProdutos { get; set; }
    private IList<ILayoutProduto> _CurrentProdutos { get; set; }
    private IAppMessageBox MsgBox { get; set; }
    #endregion

    #region EVENTOS
    /// <summary>
    /// Informa que a lista do banco de dados foi limpa.
    /// </summary>
    public event EventHandler OnDataGridCleared;
    #endregion

    #region ROW ACTUAL SELECTED
    private DataGridRow Global_RowSelecionada = default;
    #endregion

    public DataGridControleEstoque()
    {
        this.InitializeComponent();
        this.Loaded += DataGridControleEstoque_Loaded;
    }


    #region EVENTOS DE DESIGN
    private void DataGridControleEstoque_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        MsgBox = SharedResourcesApp._MessageBox;
    }
    #endregion

    #region EVENTS ROW
    private void PDataGridRow_RowSelectChanged(object sender, bool e)
    {
        var RowSelectChanged = sender as DataGridRow;
        
        //SET GLOBAL ROW SELECTED IF NULL
        if (Global_RowSelecionada is null)
        {
            Global_RowSelecionada = RowSelectChanged;
            goto Done;
        }

        //UPDATE ACTUAL ROW SELECTED
        if (RowSelectChanged.GuidRow != Global_RowSelecionada.GuidRow)
        {
            Global_RowSelecionada.DeselectRow();
            Global_RowSelecionada = RowSelectChanged;
        }

        //REMOVE ROW IN DE-SELECT
        if (!e & (Global_RowSelecionada.GuidRow == RowSelectChanged.GuidRow))
            Global_RowSelecionada = null;

        Done:;
    }

    private void PDataGridRow_ProdutoChanged(object sender, ILayoutProduto e)
    {
        bool ResultUpdate = PrivateUpdateProdutoNoDb(ref e);
        if (!ResultUpdate)
            Debug.WriteLine($"PRODUTO ID: {e.IDProduto} NÃO FOI ATUALIZADO!");
        else
            Debug.WriteLine($"PRODUTO ID: {e.IDProduto} FOI ATUALIZADO!");

        if (ResultUpdate)
            DbEstoque.SaveChanges();
    }
    #endregion

    #region MÉTODOS PÚBLICOS
    public void SetDb (IControleDatabaseEstoque pEstoqueDb)
    {
        DbEstoque = pEstoqueDb;
    }

    public async Task LoadDb()
    {
        await PrivateGetDatabaseInfo();
    }

    public void LoadProdutos(Func<ILayoutProduto, bool> pFuncLoadProdutos)
    {
        if (pFuncLoadProdutos is null)
            PrivateLoadAllProdutos();
        else
            PrivateLoadProdutosByFunc(pFuncLoadProdutos);
    }

    public void PopulateDataGrid()
    {
        PrivatePopulateData();
    }

    public DataGridRow CreateNewRow(ILayoutProduto pProduto)
    {
        DataGridRow NewRow = new ();
        NewRow.LoadProduto(pProduto);
        return NewRow;
    }

    public void AddNewRow(DataGridRow pDataGridRow)
    {
        //GET EVENTS
        pDataGridRow.ProdutoChanged += PDataGridRow_ProdutoChanged;
        pDataGridRow.RowSelectChanged += PDataGridRow_RowSelectChanged;

        //REGISTER
        PrivateRegistrarProduto(pDataGridRow);
    }

    public RemoveItemDatabaseResult DeletarProdutoRowSelecionada() 
    {
        RemoveItemDatabaseResult Result = RemoveItemDatabaseResult.Unknown;

        if(Global_RowSelecionada is null)
        {
            Result = RemoveItemDatabaseResult.NotExist;
            goto Done;
        }

        //REMOVE FROM DATA GRID ROWS
        PrivateRemoverProdutoFromDataGrid(Global_RowSelecionada);

        //DELETE IN DB
        DbEstoque.DeletarProduto(Global_RowSelecionada.Produto);

        //REMOVE ENVETS
        Global_RowSelecionada.ProdutoChanged -= PDataGridRow_ProdutoChanged;
        Global_RowSelecionada.RowSelectChanged -= PDataGridRow_RowSelectChanged;

        Result = RemoveItemDatabaseResult.Success;

    Done:;
        return Result;
    }

    public int GetCountRows()
        => StackPanelDataGridItems.Children.Count;

    public void UpdateLayoutDb()
    {
        PrivateUpdateLayoutDb();
    }

    public void ClearAll()
    {
        PrivateClearAllDataList();
    }

    public void SaveDb()
    {
        if (DbEstoque is null)
            return;

        DbEstoque.SaveChanges();
    }
    #endregion

    #region MÉTODOS PRIVADOS
    private void PrivateRegistrarProduto(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Add(pDataGridRow);
    }

    private void PrivateRemoverProdutoFromDataGrid(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Remove(pDataGridRow);
    }

    private bool PrivateUpdateProdutoNoDb(ref ILayoutProduto pRefProduto)
    {
        return DbEstoque.UpdateProduto(pRefProduto);
    }

    private void PrivateClearAllDataList()
    {
        if (Global_RowSelecionada is not null)
            Global_RowSelecionada = null;

        StackPanelDataGridItems.Children.Clear();
        OnDataGridCleared?.Invoke(this, null);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private DataGridRow PrivateSearchRowByProdutoID(int pId)
        => StackPanelDataGridItems.Children.OfType<DataGridRow>().FirstOrDefault(x => x.Produto.IDProduto == pId);

    private async Task PrivateGetDatabaseInfo()
    {
        //GET COUNT PRODUTOS
        CountProdutos = await DbEstoque.GetCountProdutos();

        //CREATE CURRENT LIST PRODUTOS
        _CurrentProdutos = new List<ILayoutProduto>(CountProdutos);
    }

    private void PrivateLoadAllProdutos()
    {
        if (_CurrentProdutos.Count > 0)
            _CurrentProdutos.Clear();

        //GET ALL PRODUTOS.
        var Dados = DbEstoque.GetProdutos();
        if (Dados is null)
        {
            MsgBox.ShowMessageAsync("Nenhum produto registrado no banco de dados!");
            return;
        }
        if (Dados.Count() <= 0)
        {
            MsgBox.ShowMessageAsync("Nenhum produto registrado no banco de dados!");
            return;
        }

        foreach (var item in Dados)
        {
            _CurrentProdutos.Add(item);
        }
    }

    private void PrivateLoadProdutosByFunc(Func<ILayoutProduto, bool> pFuncLoadProdutos)
    {
        if (_CurrentProdutos.Count > 0)
            _CurrentProdutos.Clear();

        //GET ALL PRODUTOS.
        var Dados = DbEstoque.SearchAllProdutosByFunc(pFuncLoadProdutos);
        if (Dados is null)
        {
            MsgBox.ShowMessageAsync("Nenhum produto foi encontrado com o filtro informado!");
            return;
        }
        if (Dados.Count() <= 0)
        {
            MsgBox.ShowMessageAsync("Nenhum produto foi encontrado com o filtro informado!");
            return;
        }

        foreach (var item in Dados)
        {
            _CurrentProdutos.Add(item);
        }
    }

    private void PrivatePopulateData()
    {
        if (CountProdutos <= 0)
            return;

        bool Result = DispatcherQueue.TryEnqueue(() =>
        {
            foreach (var NewProdutoAddRow in _CurrentProdutos)
            {
                var NewRow = CreateNewRow(NewProdutoAddRow);
                AddNewRow(NewRow);
            }
        });
    }

    private void PrivateUpdateLayoutDb()
    {
        UpdateLayout();
    }
    #endregion
}
