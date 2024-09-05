using MegStore.Core.Interfaces;
using MegStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegStore.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class

    {
        private readonly MegStoreContext _context;
        private readonly DbSet<T> _set;
        public Repository(MegStoreContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _set.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _set.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

    }
}
