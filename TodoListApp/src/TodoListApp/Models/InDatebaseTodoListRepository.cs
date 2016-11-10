using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using TodoListApp.Data;

namespace TodoListApp.Models
{
    public class InDatebaseTodoListRepository : ITodoListRepository
    {
        private readonly TodoListDbContext _context;

        public InDatebaseTodoListRepository(TodoListDbContext context)
        {
            _context = context;
        }

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException("item.Id must not be empty", nameof(item));

            _context.Items.Add(new Data.TodoItem { Id = item.Id, Description = item.Description, Name = item.Name, UserId = userId });
            _context.SaveChanges();
        }

        public void DeleteItem(string userId, Guid itemId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            var item = _context.Items.SingleOrDefault(x => x.Id == itemId);

            if (item == null)
                throw new KeyNotFoundException("Item was not found.");
            if (item.UserId != userId)
                throw new SecurityException("User does not own the item.");

            _context.Items.Remove(item);
            _context.SaveChanges();
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            return _context.Items.Where(x => x.UserId == userId)
                .Select(x => new TodoItem { Id = x.Id, Name = x.Name, Description = x.Description });
        }

        public TodoItem GetItemByUserAndId(string userId, Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var item = _context.Items.SingleOrDefault(x => x.Id == itemId);

            if (item != null)
                if (userId == item.UserId)
                    return new TodoItem { Id = item.Id, Name = item.Name, Description = item.Description };
            return null;
        }

        public void Update(string userId, TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            var _item = _context.Items.SingleOrDefault(x => x.Id == item.Id);

            if (_item == null)
                throw new KeyNotFoundException("Item was not found.");
            if (userId != _item.UserId)
                throw new SecurityException("User does not own the item.");

            _item.Name = item.Name;
            _item.Description = item.Description;
            _context.SaveChanges();
        }
    }
}
