using Microsoft.UI.Xaml.Controls;
using System;

namespace ControleEstoqueCore;

public interface INavigatePage
{
    public void SetFrame(Frame pFrame);

    public bool Navigate(Type pTypePage);

    public bool CurrentPageIs(Type pTypePageCheck);
}
