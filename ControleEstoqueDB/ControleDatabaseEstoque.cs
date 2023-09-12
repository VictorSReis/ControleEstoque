using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueResources;
using ControleEstoqueSDK;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ControleEstoqueDB;

public sealed class ControleDatabaseEstoque : IControleDatabaseEstoque
{
    #region PROPERTIES
    public DatabaseType TypeDb => DatabaseType.Estoque;
    #endregion

    #region PRIVATE VALUES
    private DatabaseEstoqueContext _DbContext;
    #endregion


    #region IControleDatabase
    public DbContext GetDb()
    {
        return _DbContext;
    }

    public async Task<bool> OpenDatabase
        (string pPathDb, string pNameDb)
    {
        bool Result = false;

        try
        {
            _DbContext = new DatabaseEstoqueContext(pPathDb, pNameDb);
            _DbContext.Database.OpenConnection();
            if (_DbContext.Database.CanConnect())
                Result = true;
            else
                await SharedResourcesApp._MessageBox.ShowMessageAsync("Falhou ao tentar se conectar ao Banco de dados do estoque.\n\r" +
                    $"Db Name: {pNameDb}\n\r" +
                    $"Db Path: {pPathDb}");
        }
        catch (Exception Er)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync("Ocorreu uma exceção grave ao tentar abrir a conexão" +
                $"com o Banco de dados do estoque. Message > {Er.Message}");
        }

        return Result;
    }
    #endregion

    #region IControleDatabaseEstoque
    public bool UpdateProduto(ILayoutProduto pProduto)
    {
        bool ResultOp = false;

        LayoutProduto LtProduto = pProduto as LayoutProduto;
        var Result = _DbContext.Update<LayoutProduto>(LtProduto);
        if (Result.State == EntityState.Unchanged)
            goto Done;

        ResultOp = true;

        Done:;
        return ResultOp;
    }

    public bool ProdutoExiste(int pIDProduto)
    {
        bool Result = _DbContext.Produtos.Where(x=> x.IDProduto == pIDProduto).Any();
        return Result;
    }

    public bool ProdutoExiste(string pNomeProduto)
    {
        bool Result = _DbContext.Produtos.Where(x => x.NomeProduto == pNomeProduto).Any();
        return Result;
    }

    public ILayoutProduto SearchProdutoContains(string pNomeParcial)
    {
        var ResultObj = _DbContext.Produtos.Where(x => x.NomeProduto.Contains(pNomeParcial)).FirstOrDefault();
        return ResultObj;
    }

    public IEnumerable<ILayoutProduto> SearchAllProdutosContains(string pNomeParcial)
    {
        var ResultObj = _DbContext.Produtos.Where(x => x.NomeProduto.Contains(pNomeParcial));
        return ResultObj;
    }
   
    public IEnumerable<ILayoutProduto> SearchAllProdutosByFunc(Func<ILayoutProduto, bool> pFuncExecute)
    {
        var ResultObj = _DbContext.Produtos.OfType<ILayoutProduto>().Where(pFuncExecute);
        return ResultObj;
    }

    public ILayoutProduto GetProdutoByPrimaryKey(int pKey)
    {
        throw new NotImplementedException();
    }

    public ILayoutProduto GetProduto(int pIDProduto)
    {
        var ResultObj = _DbContext.Produtos.Where(x => x.IDProduto == pIDProduto).FirstOrDefault();
        return ResultObj;
    }

    public IEnumerable<ILayoutProduto> GetProdutos()
    {
        return _DbContext.Produtos;
    }

    public async Task<int> GetCountProdutos()
    {
        return await _DbContext.Produtos.CountAsync();
    }

    public void SaveChanges()
    {
        _DbContext.SaveChanges();
    }

    public void CloseDb()
    {
        if (_DbContext is null)
            return;

        _DbContext.Dispose();
        _DbContext = null;
    }
    #endregion
}
