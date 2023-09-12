using ControleEstoqueSDK;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ControleEstoqueCore.Database;

public interface IControleDatabase
{
    public DatabaseType TypeDb { get; }


    public Task<bool> OpenDatabase
        (string pPathDb, string pNameDb);

    public DbContext GetDb();
}
