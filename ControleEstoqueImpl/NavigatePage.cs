using ControleEstoqueCore;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ControleEstoqueImpl;

public sealed class NavigatePage : INavigatePage
{
    private Frame _Frame;
    private Type _CurrentPage;

    public void SetFrame(Frame pFrame)
    {
        _Frame = pFrame;
    }

    public bool CurrentPageIs(Type pTypePageCheck)
    {
        return _CurrentPage == pTypePageCheck;
    }

    public bool Navigate(Type pTypePage)
    {
        if(pTypePage is not null)
            _CurrentPage = pTypePage;

        return _Frame.Navigate(pTypePage);
    }
}
