using ControleEstoqueCore.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoqueDataGrid.DataGridFunctions;

internal static class DataGridShared
{
    public static IControleDatabaseEstoque _DatabaseEstoque { get; private set; }
    public static DataGridPoolItems _PoolRows { get; private set; }

    public static void InitializeShared
        (IControleDatabaseEstoque pControleDatabase)
    {
        _DatabaseEstoque = pControleDatabase;
        if(_PoolRows is null)
        {
            _PoolRows = new DataGridPoolItems();
            _PoolRows.CreatePool();
            _PoolRows.InitializePool(250);
        }
    }

    public static IEnumerable<ILayoutProduto> ProdutosDb()
        => _DatabaseEstoque.GetProdutos();

    public static void SaveDb()
    {
        _DatabaseEstoque.SaveChanges();
    }
}
