using ControleEstoqueCore.Database;
using ControleEstoqueDataGrid;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ControleEstoquePages;


public sealed partial class Pg_Estoque : Page
{
    public Pg_Estoque()
    {
        this.InitializeComponent();
        this.Loaded += Pg_Estoque_Loaded;
        
    }

    private void Pg_Estoque_Loaded(object sender, RoutedEventArgs e)
    {
        ConfigureShadownPage();
        DataGridControle.SetDb(SharedResourcesDatabase.DatabaseEstoque);
    }

    #region EVENTOS DE INTERAÇÃO
    private async void Btn_TesteAddItem_Click(object sender, RoutedEventArgs e)
    {
        await DataGridControle.LoadDb();
        DataGridControle.UpdateLayoutDb();
    }
    #endregion

    #region PRIVATE
    private void ConfigureShadownPage()
    {
        GridPainelSuperior.Translation = new System.Numerics.Vector3(0, 0, 10);
        GridAreaConteudo.Translation = new System.Numerics.Vector3(0, 0, 20);
    }
    #endregion
}
