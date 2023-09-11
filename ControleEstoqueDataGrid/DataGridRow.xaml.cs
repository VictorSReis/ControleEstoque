using ControleEstoqueResources;
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
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace ControleEstoqueDataGrid;

public sealed partial class DataGridRow : UserControl
{
    public bool ItemIsSelected { get; set; }

    public DataGridRow()
    {
        this.InitializeComponent();
    }



    private void TextBox_NomeProduto_KeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            ToggleThemeTeachingTip1.IsOpen = true;
        }
    }

    private void GridPrincipal_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if(!ItemIsSelected)
            GridPrincipal.Background = new 
                SolidColorBrush(Util.CreateColorFromHex("0xFFE3E3E3"));
    }

    private void GridPrincipal_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (!ItemIsSelected)
            GridPrincipal.Background = new
                SolidColorBrush(Util.CreateColorFromHex("0xFFF0F0F0"));
    }

    private void GridPrincipal_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (ItemIsSelected)
        {
            ItemIsSelected = false;
            GridPrincipal.Background = new
                SolidColorBrush(Util.CreateColorFromHex("0xFFF0F0F0"));
        }
        else
        {
            GridPrincipal.Background = new
               SolidColorBrush(Util.CreateColorFromHex("0xFFD2DFED"));
            ItemIsSelected = true;
        }
    }
}
