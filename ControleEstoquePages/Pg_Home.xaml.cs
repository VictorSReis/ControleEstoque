using ControleEstoqueCore;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoquePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Pg_Home : Page
    {
        INavigatePage PageNav;

        public Pg_Home()
        {
            this.InitializeComponent();
            this.Loaded += Pg_Home_Loaded;
        }


        private void Pg_Home_Loaded(object sender, RoutedEventArgs e)
        {
            SharedResourcesApp.SetPageRecursoNavigator(FrameNavegacaoPages);
            PageNav = SharedResourcesApp._PageRecursoNavigator;
        }

        private void Btn_Caixa_Click(object sender, RoutedEventArgs e)
        {
            PageNav.Navigate(typeof(Pg_Caixa));
        }

        private void Btn_Estoque_Click(object sender, RoutedEventArgs e)
        {
            PageNav.Navigate(typeof(Pg_Estoque));
        }

        private void Btn_Despesas_Click(object sender, RoutedEventArgs e)
        {
            PageNav.Navigate(typeof(Pg_Despesas));
        }

        private void Btn_Realatorios_Click(object sender, RoutedEventArgs e)
        {
            PageNav.Navigate(typeof(Pg_Relatorios));
        }
    }
}
