using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEstoqueCore.Database;

public interface IControleDatabaseEstoque: IControleDatabase
{
    public bool UpdateProduto(ILayoutProduto pProduto);

    public bool ProdutoExiste(int pIDProduto);

    public bool ProdutoExiste(String pNomeProduto);

    public ILayoutProduto SearchProdutoContains(String pNomeParcial);

    public IEnumerable<ILayoutProduto> SearchAllProdutosContains(String pNomeParcial);

    public IEnumerable<ILayoutProduto> SearchAllProdutosByFunc(Func<ILayoutProduto, bool> pFuncExecute);

    public ILayoutProduto GetProduto(int pIDProduto);

    public ILayoutProduto GetProdutoByPrimaryKey(int pKey);

    public IEnumerable<ILayoutProduto> GetProdutos();

    public Task<int> GetCountProdutos();

    public void SaveChanges();

    public void CloseDb();
}
