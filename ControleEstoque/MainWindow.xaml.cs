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

    public void ConfigureResources()
    {
        //RESOURCES FOR USE APP
        IAppMessageBox MsgBox = new AppMessageBox();
        IControleDatabaseEstoque DbEstoque = new ControleDatabaseEstoque();
        IControleDatabaseCaixa DbCaixa = new ControleDatabaseCaixa();
        IContentDialogCreator ContentCreator = new ContentDialogCreator();

        //SET RESOURCE BASE
        SharedResourcesApp.SetWindow(this);
        SharedResourcesApp.SetPageAppNavigator(FrameNavegacaoApp);
        MsgBox.ConfigureWindowHandle(SharedResourcesApp._WindowHandle);
        SharedResourcesApp.SetAppMessageBox(MsgBox);
        SharedResourcesApp.SetContentDialogCreator(ContentCreator);

        //SET INSTANCE FOR DATABASES
        SharedResourcesDatabase.SetDbEstoque(DbEstoque);
        SharedResourcesDatabase.SetDbCaixa(DbCaixa);

        //OPEN DB`S
        SharedResourcesDatabase.OpenDatabases();
    }

    public void NavigateToHomePage()
    {
        bool Result = SharedResourcesApp._PageAppNavigator.Navigate(typeof(Pg_Home));
    }
}
