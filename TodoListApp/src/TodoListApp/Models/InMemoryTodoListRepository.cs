using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private static TodoListModel todoListModel = new TodoListModel { Items = new List<TodoItemModel>() };

        public void AddItem(string userId, TodoItemModel item)
        {
            item.itemId = Guid.NewGuid();
            item.userId = userId;
            ((List<TodoItemModel>)todoListModel.Items).Add(item);
        }

        public void DeleteItem(Guid itemId)
        {
            ((List<TodoItemModel>)todoListModel.Items).RemoveAll(item => item.itemId == itemId);
        }

        public IEnumerable<TodoItemModel> GetTodoListByUser(string userId)
        {
            foreach (var item in todoListModel.Items)
                if (item.userId == userId)
                    yield return item;
        }
    }
}
