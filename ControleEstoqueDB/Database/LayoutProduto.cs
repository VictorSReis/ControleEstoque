using ControleEstoqueCore.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace ControleEstoqueDB.Database;

public class LayoutProduto : ILayoutProduto
{
    [Key]
    public int Key { get; set; }

    public int IDProduto { get; set; }
    public string NomeProduto { get; set; }
    public float CustoProduto { get; set; }
    public float CustoVenda { get; set; }
    public string ValidadeProduto { get; set; }
    public int EstoqueProduto { get; set; }
}
