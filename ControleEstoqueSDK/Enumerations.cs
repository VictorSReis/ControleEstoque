using System;

namespace ControleEstoqueSDK;

public enum DatabaseType: byte
{
    Estoque,
    Caixa
}

public enum RemoveItemDatabaseResult 
{
    Success,
    NotExist,
    Error,
    Unknown
}

public enum UserInterationSaveChangeContent
{
    /// <summary>
    /// O conteúdo deve ser salvo.
    /// </summary>
    Salvar,
    
    /// <summary>
    /// Deixa a modificação mais não salve no DB ainda.
    /// </summary>
    Fechar,

    /// <summary>
    /// Cancela a alteração realizada.
    /// </summary>
    Cancelar,

    /// <summary>
    /// Valor desconhecido.
    /// </summary>
    Unknown,
}
