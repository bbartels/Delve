using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Delve.Demo.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private IUserRepository _users;

        public IUserRepository Users => _users ?? (_users = new UserRepository(_context));

        public UnitOfWork(UserManagerContext context)
        {
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) { _context.Dispose(); }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
