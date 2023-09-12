using ControleEstoqueCore;
using System.Threading.Tasks;
using System;
using Windows.UI.Popups;
using WinRT.Interop;
using System.Threading;

namespace ControleEstoqueImpl;

public sealed class AppMessageBox: IAppMessageBox
{
    #region PRIVATE
    private MessageDialog _Dialog;
    private IntPtr _WindowHandle;
    #endregion


    #region CONSTRUCTOR
    public AppMessageBox()
    {
        _Dialog = new MessageDialog(null);
    }
    #endregion

    #region IAppMessageBox
    public void ConfigureWindowHandle
        (IntPtr pWindowHandleForMessageBox)
    {
        _WindowHandle = pWindowHandleForMessageBox;

        if (pWindowHandleForMessageBox == IntPtr.Zero)
            throw new InvalidOperationException($"" +
                $"O ponteiro({nameof(IntPtr)}) passado pelo parametro ({nameof(pWindowHandleForMessageBox)}) não era válido para uma janela do Window.");

        InitializeWithWindow.Initialize(_Dialog, _WindowHandle);
    }

    public Task<IUICommand> ShowMessageAsync
        (string pTexto, string pTitle = "Controle Estoque")
    {
        return ShowMessageAsync(_Dialog, pTexto, pTitle);
    }
    #endregion

    #region PRIVATE
    private static async Task<IUICommand> ShowMessageAsync(MessageDialog pDialog, string pTexto, string pTitle)
    {
        Monitor.Enter(pDialog);

        pDialog.Content = pTexto;
        pDialog.Title = pTitle;
        var Result = await pDialog.ShowAsync();

        Monitor.Exit(pDialog);
        return Result;
    }
    #endregion
}
