using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Data.Repositories;

namespace TicTacToe.Data.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private DbContext context;
        private IDbSet<T> set;

        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.set = context.Set<T>();
        }

        public void Add(T entity)
        {
            var entry = this.context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this.set.Add(entity);
            }
        }

        public IQueryable<T> All()
        {
            return this.set.AsQueryable();
        }

        public void Delete(object id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }
        }

        public void Delete(T entity)
        {
            var entry = this.context.Entry(entity);
            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                this.set.Attach(entity);
                this.set.Remove(entity);
            }
        }

        public T Attach(T entity)
        {
            return this.context.Set<T>().Attach(entity);
        }

        public void Detach(T entity)
        {
            var entry = this.context.Entry(entity);
            entry.State = EntityState.Detached;
        }

        public T GetById(object id)
        {
            return this.set.Find(id);
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public void Update(T entity)
        {
            var entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.set.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
