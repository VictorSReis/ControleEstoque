using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ControleEstoqueCore.Database;

/// <summary>
/// Interface que declara o item enviado para o banco de dados de caixa, que contém
/// as saidas.
/// </summary>
public interface ILayoutCaixa
{

    [Key]
    public int Key { get; set; }
    
    public string IID { get; set; }

    public string Data { get; set; }

    public byte[] Objeto { get; set; }
}
