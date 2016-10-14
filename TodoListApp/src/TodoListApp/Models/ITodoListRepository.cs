using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public interface ITodoListRepository
    {
        IEnumerable<TodoItemModel> GetTodoListByUser(string userId);
        void DeleteItem(Guid itemId);
        void AddItem(string userId, TodoItemModel item);
    }
}
