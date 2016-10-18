using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private static IDictionary<string, TodoList> userTodoList = new Dictionary<string, TodoList>();

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!userTodoList.ContainsKey(userId))
                userTodoList.Add(userId, new TodoList { Items = new List<TodoItem>() });
            foreach (var user in userTodoList)
                if (user.Key == userId)
                    ((List<TodoItem>)user.Value.Items).Add(item);
        }

        public void DeleteItem(Guid itemId)
        {
            foreach (var user in userTodoList)
                foreach (var item in user.Value.Items)
                    if (item.Id == itemId)
                    {
                        ((List<TodoItem>)user.Value.Items).Remove(item);
                        return;
                    }
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            foreach (var user in userTodoList)
                if (user.Key == userId)
                    foreach (var item in user.Value.Items)
                        yield return item;
        }
    }
}
