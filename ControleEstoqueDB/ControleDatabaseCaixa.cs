using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueResources;
using ControleEstoqueSDK;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleEstoqueDB;

public sealed class ControleDatabaseCaixa : IControleDatabaseCaixa
{
    #region PROPERTIES
    public DatabaseType TypeDb => DatabaseType.Estoque;
    #endregion

    #region PRIVATE VALUES
    private DatabaseCaixaContext _DbContext;
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
            _DbContext = new DatabaseCaixaContext(pPathDb, pNameDb);
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
                $"com o Banco de dados. Message > {Er.Message}");
        }

        return Result;
    }
    #endregion

    #region IControleDatabaseCaixa
    public bool AdicionarSaida
        (ILayoutCaixa pCaixa)
    {
        var ResultAdd = _DbContext.Add<LayoutCaixa>((LayoutCaixa)pCaixa);
        _DbContext.SaveChanges();

        //CHECK ITEM ADDED.
        bool ResultItemAdded = _DbContext.Caixa.Where(x => x.IID == pCaixa.IID).Any();
        return ResultItemAdded;
    }

    public bool AtualizarSaida
        (ILayoutCaixa pCaixa)
    {
        bool ResultOp = false;

        LayoutCaixa AttSaida = pCaixa as LayoutCaixa;
        var Result = _DbContext.Update<LayoutCaixa>(AttSaida);
        if (Result.State == EntityState.Unchanged)
            goto Done;

        ResultOp = true;

    Done:;
        return ResultOp;
    }

    public ILayoutCaixa GetSaida
        (string pIIDSaida)
    {
        var ResultObj = _DbContext.Caixa.Where(x => x.IID == pIIDSaida).FirstOrDefault();
        return ResultObj;
    }

    public IEnumerable<ILayoutCaixa> GetCaixa()
    {
        return _DbContext.Caixa;
    }

    public IEnumerable<ILayoutCaixa> SearchAllSaidasByFunc
        (Func<ILayoutCaixa, bool> pFuncExecute)
    {
        var ResultObj = _DbContext.Caixa.OfType<ILayoutCaixa>().Where(pFuncExecute);
        return ResultObj;
    }

    public async Task<int> GetCount()
    {
        return await _DbContext.Caixa.CountAsync();
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
