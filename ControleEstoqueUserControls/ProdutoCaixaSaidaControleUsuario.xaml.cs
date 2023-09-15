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

namespace ControleEstoqueUserControls;

public sealed partial class ProdutoCaixaSaidaControleUsuario : UserControl
{
    #region PROPRIEDADES
    public int IdProduto { get; private set; }

    public string NomeProduto { get; private set; }

    public float ValorUnitario { get; private set; }

    public int QtdEstoque { get; private set; }
    #endregion

    #region INTERNAL
    private double _ValorTotalProduto;
    #endregion

    #region EVENTS
    public event EventHandler<int> OnSemEstoqueQtd;
    #endregion

    public ProdutoCaixaSaidaControleUsuario()
    {
        this.InitializeComponent();
    }

    #region EVENTS INTERATION
    private void NumberBox_QuantidadeSaida_LostFocus(object sender, RoutedEventArgs e)
    {
        PrivateLoadValueElements();
    }
    #endregion

    #region MÉTODOS PUBLICOS
    public void LoadProdutoInfo(
        int pIdProduto,
        String pNomeProduto,
        float pValorUnitario,
        int pQtdEstoque)
    {
        IdProduto = pIdProduto;
        NomeProduto = pNomeProduto;
        ValorUnitario = pValorUnitario;
        QtdEstoque = pQtdEstoque;

        PrivateLoadValueElements();
    }

    public float ObterValorTotal()
    {
        return (float)_ValorTotalProduto;
    }
    #endregion

    #region PRIVATE METODOS
    private void PrivateLoadValueElements()
    {
        TextBlock_NomeProduto.Text = NomeProduto;
        TextBlock_ValorUnitario.Text = ValorUnitario.ToString("C2", CultureInfo.CurrentCulture);
        TextBlock_DisponiveisEstoque.Text = QtdEstoque.ToString();


        if (NumberBox_QuantidadeSaida.Value <= -1)
        {
            OnSemEstoqueQtd?.Invoke(this, (int)NumberBox_QuantidadeSaida.Value);
            NumberBox_QuantidadeSaida.Value = 0;
            return;
        }

        if (NumberBox_QuantidadeSaida.Value > QtdEstoque)
        {
            OnSemEstoqueQtd?.Invoke(this, (int)NumberBox_QuantidadeSaida.Value);
            NumberBox_QuantidadeSaida.Value = 0;
            return;
        }

        if(NumberBox_QuantidadeSaida.Value > 0)
        {
            _ValorTotalProduto = NumberBox_QuantidadeSaida.Value * ValorUnitario;
           
            try
            {
                TextBlock_ValorTotal.Text = _ValorTotalProduto.ToString("C2", CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                TextBlock_ValorTotal.Text = "-1";
            }
        }
    }
    #endregion
}
