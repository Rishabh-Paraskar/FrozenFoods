using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenFoods.Core
{
    public class SQLRepository<T> : IRepository<T> where T : BaseEntity
    {
        internal DatabaseFirstEntities dbcontext;
        internal DbSet<T> dbSet;

        public SQLRepository(DatabaseFirstEntities context)
        {
            this.dbcontext = context;
            this.dbSet = context.Set<T>();
        }
        public IQueryable<T> collection()
        {
            return dbSet;
        }

        public void commit()
        {

            dbcontext.SaveChanges();
        }

        public void delete(string id)
        {
            var v = dbSet.Find(id);
            if (dbcontext.Entry(v).State == EntityState.Detached)
            {
                dbSet.Attach(v);
            }

            dbSet.Remove(v);
        }

        public T find(string id)
        {
            return dbSet.Find(id);
        }

        public void insert(T t)
        {
            dbSet.Add(t);
        }

        public void update(T t)
        {
            dbSet.Attach(t);
            dbcontext.Entry(t).State = EntityState.Modified;
        }
    }
}
