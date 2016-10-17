using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private static TodoList todoList = new TodoList { Items = new List<TodoItem>() };
        private static IDictionary<string, Guid> userItems = new Dictionary<string, Guid>();

        public void AddItem(string userId, TodoItem item)
        {
            item.Id = Guid.NewGuid();
            userItems.Add(userId, item.Id);
            ((List<TodoItem>)todoList.Items).Add(item);
        }

        public void DeleteItem(Guid itemId)
        {
            ((List<TodoItem>)todoList.Items).RemoveAll(item => item.Id == itemId);
            foreach (var kv in userItems.Where(kvp => kvp.Value == itemId).ToList())
                userItems.Remove(kv);
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            foreach (var user in userItems.Where(kvp => kvp.Key == userId).ToList())
                foreach (var item in todoList.Items.Where(i => i.Id == user.Value))
                    yield return item;
        }
    }
}
