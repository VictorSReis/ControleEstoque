using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace ControleEstoqueCore;

public interface IAppMessageBox
{
    public void ConfigureWindowHandle
        (IntPtr pWindowHandleForMessageBox);

    public Task<IUICommand> ShowMessageAsync
        (string pTexto, string pTitle = "Controle Estoque");
}
