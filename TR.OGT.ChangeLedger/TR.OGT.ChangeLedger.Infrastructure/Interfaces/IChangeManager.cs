using System.Threading;
using System.Threading.Tasks;

namespace TR.OGT.ChangeLedger.Infrastructure.Interfaces
{
    public interface IChangeManager
    {
        Task<bool> StartChangeLedger(CancellationToken token);
    }
}
