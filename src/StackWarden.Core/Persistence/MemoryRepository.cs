using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StackWarden.Core.Persistence
{
    public class MemoryRepository : IRepository
    {
        private readonly ConcurrentDictionary<Type, List<object>> _items = new ConcurrentDictionary<Type, List<object>>();

        public int? MaximumSize { get; set; } = null;

        public void Save<T>(T entity)
        {
            _items.AddOrUpdate(typeof(T),
                               type => new List<object>(),
                               (type, list) =>
                               {
                                   var entityList = _items[type];

                                   if (!entityList.Contains(entity))
                                       entityList.Add(entity);

                                   if (MaximumSize.HasValue && entityList.Count > MaximumSize.Value)
                                       entityList.RemoveRange(0, MaximumSize.Value);

                                   return list;
                               });
        }

        public void Delete<T>(T entity)
        {
            var entityType = typeof (T);

            if (!_items.ContainsKey(entityType))
                return;

            _items[entityType].Remove(entity);
        }

        public IQueryable<T> Query<T>()
        {
            return _items.GetOrAdd(typeof(T), type => new List<object>())
                         .OfType<T>()
                         .ToList()
                         .AsQueryable();
        }
    }
}