using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private IDictionary<string, TodoList> _todoListByUser = new Dictionary<string, TodoList>();

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException("item.Id must not be empty", nameof(item));
            TodoList todoList;
            if (!_todoListByUser.TryGetValue(userId, out todoList))
            {
                todoList = new TodoList { Items = new List<TodoItem>() };
                _todoListByUser.Add(userId, todoList);
            }
            ((List<TodoItem>)todoList.Items).Add(item);
        }

        public void DeleteItem(Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            foreach (var user in _todoListByUser)
                foreach (var item in user.Value.Items)
                    if (item.Id == itemId)
                    {
                        ((List<TodoItem>)user.Value.Items).Remove(item);
                        return;
                    }
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            TodoList todoList;
            if (_todoListByUser.TryGetValue(userId, out todoList))
            {
                foreach (var item in todoList.Items)
                    yield return item;
            }
            yield break;
        }
    }
}
