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
        SharedResourcesApp.SetPageAppNavigator(FrameNavegacaoApp);
    }

    public void NavigateToHomePage()
    {
        bool Result = SharedResourcesApp._PageAppNavigator.Navigate(typeof(Pg_Home));
    }
}
