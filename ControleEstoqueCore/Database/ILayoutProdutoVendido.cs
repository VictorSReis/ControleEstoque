using System;

namespace ControleEstoqueCore.Database;

public interface ILayoutProdutoVendido
{
    public int Key { get; set; }

    public int IDProduto { get; set; }

    public string NomeProduto { get; set; }

    public float ValorVenda { get; set; }

    public string DataVenda { get; set; }

    public int QuantidadeVendidos { get; set; }
}
