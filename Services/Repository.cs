
using Microsoft.EntityFrameworkCore;
using STORE_Website.Data;
using STORE_Website.Models;

namespace STORE_Website.Services
{
    public class Repository<T> : IReposirory<T> where T : class
    {
        private readonly ApplicationDbContext context;

        public Repository(ApplicationDbContext context) 
        {
            this.context = context;
        }
        public async Task<T> Create(T entity)
        {
            context.Set<T>().Add(entity);
            Save();
            return entity;
        }
        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
        public async Task<List<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }
        public async Task<T> GetOne(int id)
        {
            return await context.Set<T>().FindAsync(id);
        }
        public async Task<T> Update(T entity)
        {
            context.Set<T>().Update(entity);
            Save();
            return entity;
        }
        public void Save()
        {
             context.SaveChanges();
        }
    }

}
