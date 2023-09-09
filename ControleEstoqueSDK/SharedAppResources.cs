using ControleEstoqueCore;
using ControleEstoqueImpl;
using Microsoft.UI.Xaml.Controls;

namespace ControleEstoque;

public static class SharedAppResources
{
    public static INavigatePage _PageAppNavigator { get; private set; }
    public static INavigatePage _PageRecursoNavigator { get; private set; }


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
