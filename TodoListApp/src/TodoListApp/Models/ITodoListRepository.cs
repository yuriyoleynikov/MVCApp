using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public interface ITodoListRepository
    {
        IEnumerable<TodoItem> GetTodoListByUser(string userId);
        void DeleteItem(string userId, Guid itemId);
        void AddItem(string userId, TodoItem item);
        TodoItem GetItemByUserAndId(string userId, Guid itemId);
        void Update(string userId, TodoItem item);
    }
}
