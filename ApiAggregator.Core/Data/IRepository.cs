using System.Collections.Generic;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Core.Data
{
    public interface IRepository<T> where T : Root, new()
    {
        IEnumerable<T> All();
        T FindById(int id);
        void Add(T item);
        void Delete(T item);
        void Update(T item);
        void SubmitChanges();
        void Dispose();
    }
}