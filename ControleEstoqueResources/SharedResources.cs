using ControleEstoqueCore;
using ControleEstoqueImpl;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ControleEstoqueResources;

public static class SharedResourcesApp
{
    public static INavigatePage _PageAppNavigator { get; private set; }
    public static INavigatePage _PageRecursoNavigator { get; private set; }
    public static Window _MainWindow { get; private set; }


    public static void SetWindow(Window pWindow)
    {
        _MainWindow = pWindow;
    }
    public static void SetPageAppNavigator(Frame pFrameNav)
    {
        _PageAppNavigator = new NavigatePage();
        _PageAppNavigator.SetFrame(pFrameNav);
    }
    public static void SetPageRecursoNavigator(Frame pFrameNav)
    {
        _PageRecursoNavigator = new NavigatePage();
        _PageRecursoNavigator.SetFrame(pFrameNav);
    }
}
