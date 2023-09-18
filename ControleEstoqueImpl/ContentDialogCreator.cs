using ControleEstoqueCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace ControleEstoqueImpl;

public sealed class ContentDialogCreator : IContentDialogCreator
{
    public ContentDialog CreateSimpleDialog(
        XamlRoot pRootXmlView,
        string pTituloDialog, 
        object pContentThisDialog)
    {
        var ContentBase = PrivateCreateBasicContent(pRootXmlView);
        ContentBase.Title = pTituloDialog;
        ContentBase.Content = pContentThisDialog;

        return ContentBase;
    }





    private static ContentDialog PrivateCreateBasicContent(XamlRoot pRootXmlView)
    {
        ContentDialog NewDialog = new ();

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        NewDialog.XamlRoot = pRootXmlView;
        NewDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        NewDialog.Title = "NÃO DEFINIDO";
        NewDialog.PrimaryButtonText = "Fechar";
        NewDialog.DefaultButton = ContentDialogButton.Primary;

        return NewDialog;
    }
}
