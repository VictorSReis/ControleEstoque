using ControleEstoqueCore.Database;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace ControleEstoqueDB.Database;

public sealed class DatabaseCaixaContext : DbContext
{
    #region DATABASE CONFIGS
    public DbSet<LayoutCaixa> Caixa { get; set; }

    private string PathDb = string.Empty;

    private string DbName = string.Empty;
    #endregion

    #region INSTANCE
    public DatabaseCaixaContext
       (string pPathDb, string pNameDb)
    {
        PathDb = pPathDb;
        DbName = pNameDb;

        PathDb = Path.Join(PathDb, DbName);
    }
    #endregion

    #region MÉTODOS
    protected override void OnConfiguring(DbContextOptionsBuilder options)
       => options.UseSqlite($"Data Source={PathDb}");
    #endregion
}
