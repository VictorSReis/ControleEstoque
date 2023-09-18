using ControleEstoqueCore;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoqueUserControls;

public sealed partial class RelatorioVendaItemControleUsuario : UserControl
{
    #region PRIVATE
    private ILayoutCaixa _CaixaItemVendido;
    private List<ILayoutProdutoVendido> _ItemsVendidos;
    private IContentDialogCreator _DialogCreator;
    #endregion

    public RelatorioVendaItemControleUsuario()
    {
        this.InitializeComponent();
    }

    #region EVENTOS DE INTERAÇÃO
    private async void UserControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (_CaixaItemVendido is null)
            return;
        if (_DialogCreator is null)
            return;

        var ComprovanteVenda = new ComprovanteVendaControleUsuario();
        ComprovanteVenda.LoadComprovanteVenda(_CaixaItemVendido, _ItemsVendidos);

        var Dialog = _DialogCreator.CreateSimpleDialog(this.XamlRoot, "COMPROVANTE", ComprovanteVenda);
        await Dialog.ShowAsync();
    }
    #endregion


    #region PUBLICO
    public void LoadCaixaItem(
        ILayoutCaixa pItemVendido, 
        List<ILayoutProdutoVendido> pListaItensVendidos)
    {
        _CaixaItemVendido = pItemVendido;
        _ItemsVendidos = pListaItensVendidos;
        PrivateLoadElementsData();
    }

    public void SetContentDialogCreator(IContentDialogCreator pDialogCreator)
    {
        _DialogCreator = pDialogCreator;
    }
    #endregion

    #region PRIVATE
    private void PrivateLoadElementsData()
    {
        TextBlock_DataVenda.Text = _CaixaItemVendido.Data;
        TextBlock_VendaID.Text = _CaixaItemVendido.IID;
        TextBlock_ValorTotal.Text = PrivateCalculeTotalVenda().ToString("C2", CultureInfo.CurrentCulture);
    }

    private float PrivateCalculeTotalVenda()
    {
        float Total = 0.0f;
        foreach (var item in _ItemsVendidos)
        {
            Total += item.ValorVenda;
        }
        return Total;
    }
    #endregion
}
