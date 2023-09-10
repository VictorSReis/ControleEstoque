using ControleEstoqueSDK;
using Microsoft.EntityFrameworkCore;
using System;

namespace ControleEstoqueCore.Database;

public interface IControleDatabase
{
    public DatabaseType TypeDb { get; }


    public bool OpenDatabase();

    public DbContext GetDb();
}
