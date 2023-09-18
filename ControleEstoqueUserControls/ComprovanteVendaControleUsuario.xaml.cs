using ControleEstoqueCore.Database;
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
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoqueUserControls;

public sealed partial class ComprovanteVendaControleUsuario : UserControl
{
    #region PRIVATE
    private ILayoutCaixa _ItemVendido;
    private List<ILayoutProdutoVendido> _ProdutosVendidos;
    #endregion

    public ComprovanteVendaControleUsuario()
    {
        this.InitializeComponent();
        this.Unloaded += ComprovanteVendaControleUsuario_Unloaded;
    }

    #region EVENTS DESIGN
    private void ComprovanteVendaControleUsuario_Unloaded(object sender, RoutedEventArgs e)
    {
        UnloadData();
    }
    #endregion

    #region PUBLICO
    public void LoadComprovanteVenda(ILayoutCaixa pCaixaItemVendido, List<ILayoutProdutoVendido> pListaItemsVendidos)
    {
        _ItemVendido = pCaixaItemVendido;
        _ProdutosVendidos = pListaItemsVendidos;
        PrivateLoadData();
        PrivateLoadItensVendidos();
    }
    #endregion

    #region PRIVATE
    private void PrivateLoadData()
    {
        TextBlock_VendaID.Text = $"VENDA #{_ItemVendido.IID}";
        TextBlock_QuantidadeItensVenda.Text = $"{_ProdutosVendidos.Count.ToString("D2")} Itens";
        TextBlock_DataVenda.Text = $"Data Venda: {_ItemVendido.Data}";
        TextBlock_ValorTotal.Text = $"Valor Total: {PrivateCalculeTotalVenda()}";

    }

    private void PrivateLoadItensVendidos()
    {
        foreach (var item in _ProdutosVendidos)
        {
            TextBlock NewTxt = new ();
            NewTxt.FontSize = 14;
            NewTxt.Text = $"x{item.QuantidadeVendidos}   {item.NomeProduto}   {item.ValorVenda.ToString("C2", CultureInfo.CurrentCulture)}";
            StackPanel_ItensVendidos.Children.Add(NewTxt);
        }
    }

    private float PrivateCalculeTotalVenda()
    {
        float Total = 0.0f;
        foreach (var item in _ProdutosVendidos)
        {
            Total += item.ValorVenda;
        }
        return Total;
    }

    private void UnloadData()
    {
        if (_ItemVendido is not null)
            _ItemVendido = null;
        if (_ProdutosVendidos is not null)
            _ProdutosVendidos = null;
    }
    #endregion
}
