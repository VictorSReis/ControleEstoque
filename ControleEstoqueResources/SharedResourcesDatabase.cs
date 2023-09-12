using ControleEstoqueCore.Database;
using System.Collections.ObjectModel;

namespace ControleEstoqueResources;

public static class SharedResourcesDatabase
{
    public static IControleDatabaseEstoque DatabaseEstoque { get; private set; }
    
    public static IControleDatabaseCaixa DatabaseCaixa { get; private set; }


    public static void SetDatabaseEstoque(IControleDatabaseEstoque pDatabase)
    { DatabaseEstoque = pDatabase; }

    public static void SetDatabaseCaixa(IControleDatabaseCaixa pDatabase)
    { DatabaseCaixa = pDatabase; }
}
