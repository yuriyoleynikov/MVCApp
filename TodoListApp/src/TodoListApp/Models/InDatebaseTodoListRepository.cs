using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InDatebaseTodoListRepository : ITodoListRepository
    {
        //private IDictionary<string, TodoList> _todoListByUser = new Dictionary<string, TodoList>(); // need delet this
        //private IDictionary<Guid, Entry> _entriesById = new Dictionary<Guid, Entry>(); // need delet this


        public InDatebaseTodoListRepository()
        {

            //_todoListByUser = context.Items.ToDictionary(x => x.UserId, new TodoList { Items = new TodoItem { Id = } });
            //_entriesById = context.Items.ToDictionary(x => x.Id, );
        }

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

            using (var context = new MyDbContext())
            {
                context.Items.Add(new TodoItemTable { Id = item.Id, Description = item.Description, Name = item.Name, UserId = userId });
                context.SaveChanges();
            }

            /*
            TodoList todoList;
            if (!_todoListByUser.TryGetValue(userId, out todoList))
                _todoListByUser.Add(userId, new TodoList { Items = new List<TodoItem> { item } });
            else
                ((List<TodoItem>)todoList.Items).Add(item);
            _entriesById.Add(item.Id, new Entry { UserId = userId, Item = item });*/
        }

        public void DeleteItem(string userId, Guid itemId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            using (var context = new MyDbContext())
            {
                var item = context.Items.SingleOrDefault(x => x.Id == itemId);

                if (item == null)
                    throw new KeyNotFoundException("Item was not found.");
                if (item.UserId != userId)
                    throw new SecurityException("User does not own the item.");

                context.Items.Remove(item);
                context.SaveChanges();
            }
            /*
            Entry entry;
            if (!_entriesById.TryGetValue(itemId, out entry))
                throw new KeyNotFoundException("Item was not found.");
            if (entry.UserId != userId)
                throw new SecurityException("User does not own the item.");

            TodoList todoList;
            _todoListByUser.TryGetValue(userId, out todoList);
            ((List<TodoItem>)todoList.Items).Remove(entry.Item);
            _entriesById.Remove(itemId);
            */
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            using (var context = new MyDbContext())
            {
                return context.Items.Where(x => x.UserId == userId)
                    .Select(x => new TodoItem { Id = x.Id, Name = x.Name, Description = x.Description });
            }

            /*
            TodoList todoList;
            if (_todoListByUser.TryGetValue(userId, out todoList))
                return todoList.Items;
            return Enumerable.Empty<TodoItem>();*/
        }

        public TodoItem GetItemByUserAndId(string userId, Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            using (var context = new MyDbContext())
            {
                var item = context.Items.SingleOrDefault(x => x.Id == itemId);

                if (item != null)
                    if (userId == item.UserId)
                        return new TodoItem { Id = item.Id, Name = item.Name, Description = item.Description };
                return null;
            }

            /*
            Entry entry;
            if (_entriesById.TryGetValue(itemId, out entry))
                if (userId == entry.UserId)
                    return entry.Item;
            return null;
            */
        }

        public void Update(string userId, TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            using (var context = new MyDbContext())
            {
                context.Items.Add(new TodoItemTable { Id = item.Id, Description = item.Description, Name = item.Name, UserId = userId });

                
                foreach (var _item in context.Items)
                {
                    if (_item.Id == item.Id)
                    {
                        if (userId != _item.UserId)
                            throw new SecurityException("User does not own the item.");
                        _item.Name = item.Name;
                        _item.Description = item.Description;
                        context.SaveChanges();
                        return;
                    }
                }
                throw new KeyNotFoundException("Item was not found.");                
            }
            /*
            Entry entry;
            if (!_entriesById.TryGetValue(item.Id, out entry))
                throw new KeyNotFoundException("Item was not found.");
            if (userId != entry.UserId)
                throw new SecurityException("User does not own the item.");

            entry.Item.Name = item.Name;
            entry.Item.Description = item.Description;
            */
        }
    }
}
