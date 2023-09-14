using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
using ControleEstoqueUserControls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.XPath;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ControleEstoquePages;


public sealed partial class Pg_Caixa : Page
{
    public DatabaseEstoqueContext DbEstoqueContext;

    public Pg_Caixa()
    {
        this.InitializeComponent();

    }
}
