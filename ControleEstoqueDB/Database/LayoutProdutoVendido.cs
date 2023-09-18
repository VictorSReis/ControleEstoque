using ControleEstoqueCore.Database;
using System;

namespace ControleEstoqueDB.Database;

public sealed class LayoutProdutoVendido : ILayoutProdutoVendido
{
    public int IDProduto { get; set; }
    public string NomeProduto { get; set; }
    public float ValorVenda { get; set; }
    public int QuantidadeVendidos { get; set; }
}

