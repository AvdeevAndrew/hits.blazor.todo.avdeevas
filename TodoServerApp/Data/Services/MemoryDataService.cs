using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoServerApp.Data.interfaces;

namespace TodoServerApp.Data.Services
{
    public class MemoryDataService : IDataService
    {
        private static readonly object _lock = new();
        private static List<TaskItem> Tasks { get; } = new()
        {
            new () {Id = 1, Title = "Задача 1", Description = "Описание задачи 1", CreatedDate = DateTime.Now },
            new () {Id = 2, Title = "Задача 2", Description = "Описание задачи 2", CreatedDate = DateTime.Now },
            new () {Id = 3, Title = "Задача 3", Description = "Описание задачи 3", CreatedDate = DateTime.Now },
        };

        public async Task<IEnumerable<TaskItem>> GetTaskItemsAsync()
        {
            await Task.Delay(500);
            lock (_lock)
            {
                // return a shallow copy to avoid external modifications of internal list
                return Tasks.Select(t => t).ToList();
            }
        }

        public Task<TaskItem?> GetTaskAsync(int id)
        {
            lock (_lock)
            {
                var item = Tasks.FirstOrDefault(t => t.Id == id);
                return Task.FromResult<TaskItem?>(item);
            }
        }

        public Task SaveAsync(TaskItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lock (_lock)
            {
                var existing = Tasks.FirstOrDefault(t => t.Id == item.Id && item.Id != 0);
                if (existing == null)
                {
                    var nextId = Tasks.Any() ? Tasks.Max(t => t.Id) + 1 : 1;
                    item.Id = nextId;
                    if (item.CreatedDate == null)
                        item.CreatedDate = DateTime.Now;
                    Tasks.Add(item);
                }
                else
                {
                    existing.Title = item.Title;
                    existing.Description = item.Description;
                    existing.FinishDate = item.FinishDate;
                    if (existing.CreatedDate == null)
                        existing.CreatedDate = item.CreatedDate ?? DateTime.Now;
                }
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            lock (_lock)
            {
                var existing = Tasks.FirstOrDefault(t => t.Id == id);
                if (existing != null)
                {
                    Tasks.Remove(existing);
                }
            }

            return Task.CompletedTask;
        }
    }
}
