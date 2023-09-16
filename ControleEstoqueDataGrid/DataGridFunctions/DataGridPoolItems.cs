using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoqueDataGrid.DataGridFunctions;

internal class DataGridPoolItems
{
    private ConcurrentBag<DataGridRow> _Bag;


    internal void CreatePool()
    {
        _Bag = new ConcurrentBag<DataGridRow>();
    }

    internal void InitializePool(int pCountInitialize)
    {
        for (int i = 0; i < pCountInitialize; i++)
        {
            _Bag.Add(new DataGridRow());
        }
    }

    internal DataGridRow GetPoolObject()
    {
        var result = _Bag.TryTake(out var row);
        if (result)
            return row;

        return null;
    }

    internal void ReturnObjectToPool(DataGridRow pGridRow)
    {
        _Bag.Add(pGridRow);
    }
}
