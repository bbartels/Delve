using System;
using System.Threading.Tasks;

namespace Delve.Demo.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }

        void SaveChanges();

        Task SaveChangesAsync();
    }
}
