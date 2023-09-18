using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleEstoqueCore;

public interface IContentDialogCreator
{
    public ContentDialog CreateSimpleDialog(XamlRoot pRootXmlView, string pTituloDialog, object pContentThisDialog);

}
