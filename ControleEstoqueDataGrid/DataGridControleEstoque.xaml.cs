using ControleEstoqueCore;
using ControleEstoqueCore.Database;
using ControleEstoqueDataGrid.DataGridFunctions;
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
            DataGridShared.SaveDb();
    }
    #endregion

    #region MÉTODOS PÚBLICOS
    public void LoadDb(IControleDatabaseEstoque pEstoqueDb)
    {
        DataGridShared.InitializeShared(pEstoqueDb);
    }

    public void PopulateDataGrid(Func<ILayoutProduto, bool> pFuncLoadProdutos)
    {
        if(pFuncLoadProdutos is not null)
            PrivatePopulateDataByFunc(pFuncLoadProdutos);
        else
            PrivatePopulateData();
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
        PrivateRemoverProdutoFromGridView(Global_RowSelecionada);

        //DELETE IN DB
        DataGridShared._DatabaseEstoque.DeletarProduto(Global_RowSelecionada.Produto);

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
        if (DataGridShared._DatabaseEstoque is null)
            return;

        DataGridShared.SaveDb();
    }
    #endregion

    #region MÉTODOS PRIVADOS
    private void PrivateAddProdutoInGridView(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Add(pDataGridRow);
    }

    private void PrivateRemoverProdutoFromGridView(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Remove(pDataGridRow);
    }

    private bool PrivateUpdateProdutoNoDb(ref ILayoutProduto pRefProduto)
    {
        return DataGridShared._DatabaseEstoque.UpdateProduto(pRefProduto);
    }

    private void PrivateClearAllDataList()
    {
        if (Global_RowSelecionada is not null)
            Global_RowSelecionada = null;

        foreach (var Row in StackPanelDataGridItems.Children.OfType<DataGridRow>())
        {
            //LIMPA OS DADOS
            Row.RowSelectChanged -= PDataGridRow_RowSelectChanged;
            Row.ProdutoChanged -= PDataGridRow_ProdutoChanged;
            Row.UnloadRow();

            //DEVOLVE PARA O POOL
            DataGridShared._PoolRows.ReturnObjectToPool(Row);
        }

        StackPanelDataGridItems.Children.Clear();
        OnDataGridCleared?.Invoke(this, null);
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    private DataGridRow PrivateSearchRowByProdutoID(int pId)
        => StackPanelDataGridItems.Children.OfType<DataGridRow>().FirstOrDefault(x => x.Produto.IDProduto == pId);

    private async void PrivatePopulateDataByFunc(Func<ILayoutProduto, bool> pFuncLoadProdutos)
    {
        int _CurrentCountProdutos = await DataGridShared._DatabaseEstoque.GetCountProdutos();
        if (_CurrentCountProdutos == 0)
        {
            await MsgBox.ShowMessageAsync("Nenum produto foi registrando no banco de dados!");
            return;
        }

        bool Result = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
        {
            foreach (var NewProduto in DataGridShared._DatabaseEstoque.SearchAllProdutosByFunc(pFuncLoadProdutos))
            {
                //GET OBJ FROM POOL
                var RowObj = DataGridShared._PoolRows.GetPoolObject();

                //REGISTER EVENTS
                RowObj.ProdutoChanged += PDataGridRow_ProdutoChanged;
                RowObj.RowSelectChanged += PDataGridRow_RowSelectChanged;

                //LOAD INFO
                RowObj.LoadProduto(NewProduto);

                //ADICIONA NA VIEW
                PrivateAddProdutoInGridView(RowObj);
            }
        });
    }

    private async void PrivatePopulateData()
    {
        int _CurrentCountProdutos = await DataGridShared._DatabaseEstoque.GetCountProdutos();
        if (_CurrentCountProdutos == 0)
        {
            await MsgBox.ShowMessageAsync("Nenum produto foi registrando no banco de dados!");
            return;
        }

        bool Result = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.High, () =>
        {
            foreach (var NewProduto in DataGridShared.ProdutosDb())
            {
                //GET OBJ FROM POOL
                var RowObj = DataGridShared._PoolRows.GetPoolObject();

                //REGISTER EVENTS
                RowObj.ProdutoChanged += PDataGridRow_ProdutoChanged;
                RowObj.RowSelectChanged += PDataGridRow_RowSelectChanged;

                //LOAD INFO
                RowObj.LoadProduto(NewProduto);

                //ADICIONA NA VIEW
                PrivateAddProdutoInGridView(RowObj);
            }
        });
    }

    private void PrivateUpdateLayoutDb()
    {
        UpdateLayout();
    }
    #endregion
}
