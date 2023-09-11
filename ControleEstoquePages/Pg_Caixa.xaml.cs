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
        ShowItem();
        //CreateDb();
        //AddItem();
        
    }


    public void CreateDb()
    {
        DbEstoqueContext = new DatabaseEstoqueContext
            ("C:\\Users\\VictorSReis\\OneDrive\\Documentos\\ControleEstoqueRenata", "Estoque.db");
    }

    public void AddItem()
    {
        var NewProduto = new LayoutProduto()
        {
            IDProduto = 546,
            NomeProduto = "Esmalte",
            CustoProduto = 10.52f,
            CustoVenda = 30.45f,
            ValidadeProduto = "01:01:2023",
            EstoqueProduto = 50
        };
        var ResultObj = DbEstoqueContext.Produtos.Any(x => x.IDProduto == 546);


        var Result = DbEstoqueContext.Add(NewProduto);
        DbEstoqueContext.SaveChanges(true);
    }

    public void ShowItem()
    {
        Usr_Ctl_Item_Produto_Vendido NewItem = new();
        Usr_Ctl_Item_Produto_Procura_Caixa NewItemProcura = new();
        ListViewUltimasVendas.Items.Add(NewItem);
        ListViewResultadosProcuraProduto.Items.Add(NewItemProcura);
    }
}
