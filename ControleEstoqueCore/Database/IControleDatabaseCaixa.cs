using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControleEstoqueCore.Database;

public interface IControleDatabaseCaixa: IControleDatabase
{
    public bool AdicionarSaida(ILayoutCaixa pCaixa);

    public bool AtualizarSaida(ILayoutCaixa pCaixa);

    public ILayoutCaixa GetSaida(string pIIDSaida);

    public IEnumerable<ILayoutCaixa> SearchAllSaidasByFunc(Func<ILayoutCaixa, bool> pFuncExecute);

    public IEnumerable<ILayoutCaixa> GetCaixa();

    public Task<int> GetCount();

    public void SaveChanges();

    public void CloseDb();
}
