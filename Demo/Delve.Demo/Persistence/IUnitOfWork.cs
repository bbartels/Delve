using System;
using System.Collections.Generic;
using System.Linq;
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
