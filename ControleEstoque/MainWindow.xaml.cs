using ControleEstoqueCore;
using ControleEstoqueCore.Database;
using ControleEstoqueDB;
using ControleEstoqueDB.Database;
using ControleEstoqueImpl;
using ControleEstoquePages;
using ControleEstoqueResources;
using Microsoft.UI.Xaml;

namespace ControleEstoque;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        ConfigureResources();
        NavigateToHomePage();
    }

    public async void ConfigureResources()
    {
        //RESOURCES FOR USE APP
        IAppMessageBox MsgBox = new AppMessageBox();
        IControleDatabaseEstoque DbEstoque = new ControleDatabaseEstoque();

        //SET RESOURCE BASE
        SharedResourcesApp.SetWindow(this);
        SharedResourcesApp.SetPageAppNavigator(FrameNavegacaoApp);
        MsgBox.ConfigureWindowHandle(SharedResourcesApp._WindowHandle);
        SharedResourcesApp.SetAppMessageBox(MsgBox);

        //CONFIGURE DATABASE RESOURCES
        bool ResultOpenDb = await DbEstoque.OpenDatabase(@"C:\Users\VictorSReis\OneDrive\Documentos\ControleEstoqueRenata", "Estoque.db");
        SharedResourcesDatabase.SetDatabaseEstoque(DbEstoque);
    }

    public void NavigateToHomePage()
    {
        bool Result = SharedResourcesApp._PageAppNavigator.Navigate(typeof(Pg_Home));
    }
}
