using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoServerApp.Data.interfaces;

namespace TodoServerApp.Data.Services
{
    public class MSSQLDataService : IDataService
    {
        private readonly ApplicationDbContext _context;

        public MSSQLDataService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<TaskItem>> GetTaskItemsAsync()
        {
            return await _context.TaskItems.ToArrayAsync();
        }

        public async Task SaveAsync(TaskItem item)
        {
            if (item.Id == 0)
            {
                item.CreatedDate = DateTime.Now;
                await _context.TaskItems.AddAsync(item);
            }
            else
            {
                _context.TaskItems.Update(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<TaskItem?> GetTaskAsync(int id)
        {
            return await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var taskItem = await _context.TaskItems.FirstOrDefaultAsync(x => x.Id == id);
            if (taskItem is null) return;
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }
    }
}
