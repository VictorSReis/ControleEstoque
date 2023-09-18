using System;

namespace ControleEstoqueCore.Database;

/// <summary>
/// Interface que declara um produto vendido. Este item é serializado dentro de uma lista
/// e enviado para o Banco de dados (Caixa).
/// </summary>
public interface ILayoutProdutoVendido
{
    public int IDProduto { get; set; }

    public string NomeProduto { get; set; }

    public float ValorVenda { get; set; }

    public int QuantidadeVendidos { get; set; }
}
