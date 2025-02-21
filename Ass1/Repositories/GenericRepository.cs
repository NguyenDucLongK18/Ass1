using Ass1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ass1.Repositories
{
    public class GenericRepository<T> where T : class
    {
        protected readonly FunewsManagementContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(FunewsManagementContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public List<T> GetAll() => _dbSet.ToList();

        public T GetById(object id) => _dbSet.Find(id);

        public void Insert(T entity)
        {
            _dbSet.Add(entity);
            Save();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            Save();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            Save();
        }

        public void Save() => _context.SaveChanges();
    }
}
