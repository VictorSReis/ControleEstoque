using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueSDK;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ControleEstoqueDataGrid;

public sealed partial class Usr_Ctl_Controle_Estoque_Data_Grid : UserControl
{
    #region PROPRIEDAES
    private IControleDatabaseEstoque DbEstoque { get; set; }
    private int CountProdutos { get; set; }
    private IEnumerable<ILayoutProduto> _CurrentProdutos { get; set; }
    #endregion

    public Usr_Ctl_Controle_Estoque_Data_Grid()
    {
        this.InitializeComponent();
    }

    #region MÉTODOS PÚBLICOS
    public void SetDb (IControleDatabaseEstoque pEstoqueDb)
    {
        DbEstoque = pEstoqueDb;
    }

    public async Task LoadDb()
    {
        await PrivateGetDatabaseInfo();
        PrivatePopulateData();
    }

    public DataGridRow CreateNewRow(ILayoutProduto pProduto)
    {
        DataGridRow NewRow = new ();
        NewRow.LoadProduto(pProduto);
        return NewRow;
    }

    public void AddItem(DataGridRow pDataGridRow)
    {
        //GET EVENTS
        pDataGridRow.ProdutoChanged += PDataGridRow_ProdutoChanged;
        pDataGridRow.RowSelectChanged += PDataGridRow_RowSelectChanged;

        //REGISTER
        PrivateRegistrarProduto(pDataGridRow);
    }

    public RemoveItemDatabaseResult RemoveItem(int pIdProduto) 
    {
        RemoveItemDatabaseResult Result = RemoveItemDatabaseResult.Unknown;

        var Row = PrivateSearchRowByProdutoID(pIdProduto);
        if(Row is null)
        {
            Result = RemoveItemDatabaseResult.NotExist;
            goto Done;
        }    

        //REMOVE FROM DB STACK
        PrivateRemoverProduto(Row);

        //REMOVE ENVETS
        Row.ProdutoChanged -= PDataGridRow_ProdutoChanged;
        Row.RowSelectChanged -= PDataGridRow_RowSelectChanged;

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
    #endregion

    #region EVENTS ROW
    private void PDataGridRow_RowSelectChanged(object sender, bool e)
    {
        Debug.WriteLine("ROW SELECIONADA");
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

    #region MÉTODOS
    private void PrivateRegistrarProduto(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Add(pDataGridRow);
    }

    private void PrivateRemoverProduto(DataGridRow pDataGridRow)
    {
        StackPanelDataGridItems.Children.Remove(pDataGridRow);
    }

    private bool PrivateUpdateProdutoNoDb(ref ILayoutProduto pRefProduto)
    {
        return DbEstoque.UpdateProduto(pRefProduto);
    }

    private DataGridRow PrivateSearchRowByProdutoID(int pId)
        => StackPanelDataGridItems.Children.OfType<DataGridRow>().FirstOrDefault(x => x.Produto.IDProduto == pId);

    private async Task PrivateGetDatabaseInfo()
    {
        //GET COUNT PRODUTOS
        CountProdutos = await DbEstoque.GetCountProdutos();

        //GET ALL PRODUTOS.
        _CurrentProdutos = DbEstoque.GetProdutos();

        var Teste = DbEstoque.SearchAllProdutosByFunc(x => x.NomeProduto.Contains("Eudora"));
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
                AddItem(NewRow);
            }
        });
    }

    private void PrivateUpdateLayoutDb()
    {
        UpdateLayout();
    }
    #endregion
}
