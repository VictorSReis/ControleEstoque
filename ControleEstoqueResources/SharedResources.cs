using ControleEstoqueCore;
using ControleEstoqueImpl;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ControleEstoqueResources;

public static class SharedResourcesApp
{
    public static INavigatePage _PageAppNavigator { get; private set; }
    public static INavigatePage _PageRecursoNavigator { get; private set; }
    public static IAppMessageBox _MessageBox { get; private set; }
    public static IContentDialogCreator _DialogCreator { get; private set; }
    public static Window _MainWindow { get; private set; }
    public static IntPtr _WindowHandle { get; private set; }
    public static WindowId _WindowId { get; private set; }


    public static void SetWindow(Window pWindow)
    {
        _MainWindow = pWindow;

        _WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(_MainWindow);
        _WindowId = Win32Interop.GetWindowIdFromWindow(_WindowHandle);
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
    public static void SetAppMessageBox(IAppMessageBox pAppMessageBox)
    {
        _MessageBox = pAppMessageBox;
    }
    public static void SetContentDialogCreator(IContentDialogCreator pContentDialog)
    {
        _DialogCreator = pContentDialog;
    }
}
