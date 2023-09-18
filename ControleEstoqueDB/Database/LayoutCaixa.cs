using ControleEstoqueCore.Database;
using System;
using System.ComponentModel.DataAnnotations;

namespace ControleEstoqueDB.Database;

public sealed class LayoutCaixa : ILayoutCaixa
{
    [Key]
    public int Key { get; set; }
    public string IID { get; set; }
    public string Data { get; set; }
    public byte[] Objeto { get; set; }
}
