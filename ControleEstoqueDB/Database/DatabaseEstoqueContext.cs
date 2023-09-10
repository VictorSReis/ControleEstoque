using ControleEstoqueCore.Database;
using Microsoft.EntityFrameworkCore;
using System.IO;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoqueDB.Database;


public sealed class DatabaseEstoqueContext : DbContext
{
    #region DATABASE CONFIGS
    public DbSet<LayoutProduto> Produtos { get; set; }

    private string PathDb = string.Empty;

    private string DbName = string.Empty;
    #endregion

    #region INSTANCE
    public DatabaseEstoqueContext
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
