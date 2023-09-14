using ControleEstoqueCore;
using ControleEstoqueCore.Database;
using ControleEstoqueDB.Database;
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace ControleEstoquePages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PgContentDialogAddNewProdutoStyle : Page
{
    public PgContentDialogAddNewProdutoStyle()
    {
        this.InitializeComponent();
        this.Loaded += PgContentDialogAddNewProdutoStyle_Loaded;
    }

    #region EVENTS PAGE
    private void PgContentDialogAddNewProdutoStyle_Loaded(object sender, RoutedEventArgs e)
    {
        DatePicker_ValidadeProduto.Date = DateTime.Now;
    }
    #endregion

    #region EVENTS INTERATIONS
    private void Txb_PrecoVenda_LostFocus(object sender, RoutedEventArgs e)
    {
        string DadosTxt = Txb_PrecoVenda.Text;
        float Value = 0.0f;
        if (string.IsNullOrEmpty(DadosTxt))
            return;

        bool ResultConvert = float.TryParse(DadosTxt, NumberStyles.Currency, CultureInfo.CurrentCulture, out Value);
        if (!ResultConvert)
            return;

        Txb_PrecoVenda.Text = Value.ToString("C2", CultureInfo.CurrentCulture);
    }

    private void Txb_CustoProduto_LostFocus(object sender, RoutedEventArgs e)
    {
        string DadosTxt = Txb_CustoProduto.Text;
        float Value = 0.0f;
        if (string.IsNullOrEmpty(DadosTxt))
            return;

        bool ResultConvert = float.TryParse(DadosTxt, NumberStyles.Currency, CultureInfo.CurrentCulture, out Value);
        if (!ResultConvert)
            return;

        Txb_CustoProduto.Text = Value.ToString("C2", CultureInfo.CurrentCulture);
    }
    #endregion

    #region GET DATA
    public ILayoutProduto ObterRegistroProduto()
    {
        ILayoutProduto NewProduto = new LayoutProduto()
        {
            IDProduto = int.Parse(Txb_IDProduto.Text),
            NomeProduto = Txb_NomeProduto.Text,
            CustoProduto = float.Parse(Txb_CustoProduto.Text, NumberStyles.Currency, CultureInfo.CurrentCulture),
            CustoVenda = float.Parse(Txb_PrecoVenda.Text, NumberStyles.Currency, CultureInfo.CurrentCulture),
            ValidadeProduto = DatePicker_ValidadeProduto.Date.Date.ToShortDateString(),
            EstoqueProduto = int.Parse(Txb_QtdProduto.Text, NumberStyles.Number)
        };

        return NewProduto;
    }

    public void ClearData()
    {
        Txb_IDProduto.Text = string.Empty;
        Txb_NomeProduto.Text = string.Empty;
        Txb_CustoProduto.Text = string.Empty;
        Txb_PrecoVenda.Text = string.Empty;
        DatePicker_ValidadeProduto.Date = DateTime.Now;
        Txb_QtdProduto.Text = string.Empty;
    }
    #endregion
}
