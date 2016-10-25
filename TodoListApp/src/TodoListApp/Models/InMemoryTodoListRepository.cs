using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private IDictionary<string, TodoList> _todoListByUser = new Dictionary<string, TodoList>();
        private IDictionary<Guid, Entry> _entriesById = new Dictionary<Guid, Entry>();

        private class Entry
        {
            public string UserId;
            public TodoItem Item;
        }

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
                _todoListByUser.Add(userId, new TodoList { Items = new List<TodoItem> { item } });
            else
                ((List<TodoItem>)todoList.Items).Add(item);
            _entriesById.Add(item.Id, new Entry { UserId = userId, Item = item });
        }

        public void DeleteItem(Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            Entry entry;
            if (_entriesById.TryGetValue(itemId, out entry))
            {
                var userId = entry.UserId;
                var todoList = _todoListByUser[userId];
                ((List<TodoItem>)todoList.Items).Remove(entry.Item);
                _entriesById.Remove(itemId);
            }
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            TodoList todoList;
            if (_todoListByUser.TryGetValue(userId, out todoList))
                return todoList.Items;
            return Enumerable.Empty<TodoItem>();
        }

        public TodoItem GetItemByUserAndId(string userId, Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            Entry entry;
            if (_entriesById.TryGetValue(itemId, out entry))
                if (userId == entry.UserId)
                    return entry.Item;
            return null;
        }

        public void Update(string userId, TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            Entry entry;
            if (!_entriesById.TryGetValue(item.Id, out entry))
                throw new KeyNotFoundException("Item was not found.");
            if (userId != entry.UserId)
                throw new SecurityException("User does not own the item.");

            entry.Item.Name = item.Name;
            entry.Item.Description = item.Description;
        }
    }
}
