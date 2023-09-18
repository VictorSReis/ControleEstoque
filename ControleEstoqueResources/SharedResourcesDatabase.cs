using ControleEstoqueCore.Database;
using Microsoft.UI.Xaml;
using System;
using System.IO;

namespace ControleEstoqueResources;

public static class SharedResourcesDatabase
{
    #region PROPERTIES
    public static IControleDatabaseEstoque DatabaseEstoque { get; private set; }
    
    public static IControleDatabaseCaixa DatabaseCaixa { get; private set; }
    #endregion

    #region CONSTANTES
    public const string DB_NAME_ESTOQUE = "Estoque.db";
    public const string DB_NAME_CAIXA = "Caixa.db";
    public const string DB_FOLDER_NAME = "ControleEstoqueRenata";
    #endregion

    #region Métodos
    public static void SetDbEstoque(IControleDatabaseEstoque pDatabaseEstoque)
    { DatabaseEstoque = pDatabaseEstoque; }

    public static void SetDbCaixa(IControleDatabaseCaixa pDatabaseCaixa)
    { DatabaseCaixa = pDatabaseCaixa; }

    public static async void OpenDatabases()
    {
        //CREATE URL DB
        var PathDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var PathCompleteFolder = Path.Combine(PathDocument, DB_FOLDER_NAME);

        //OPEN
        bool ResultOpenDb = await DatabaseEstoque.OpenDatabase(PathCompleteFolder, DB_NAME_ESTOQUE);
        if (!ResultOpenDb)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync("Ocorreu uma falha ao tentar abrir o Db do estoque!");
            Application.Current.Exit();
        }
        ResultOpenDb = await DatabaseCaixa.OpenDatabase(PathCompleteFolder, DB_NAME_CAIXA);
        if (!ResultOpenDb)
        {
            await SharedResourcesApp._MessageBox.ShowMessageAsync("Ocorreu uma falha ao tentar abrir o Db do caixa!");
            Application.Current.Exit();
        }
    }
    #endregion
}
