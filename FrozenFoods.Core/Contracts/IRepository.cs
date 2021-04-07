using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenFoods.Core.Contracts
{
    public interface IRepository<T> where T : BaseEntity
    {
        void commit();
        void delete(string id);
        void insert(T t);
        void update(T t);
        T find(string id);
        IQueryable<T> collection();

    }
}
