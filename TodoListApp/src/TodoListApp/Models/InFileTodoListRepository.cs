using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InFileTodoListRepository : ITodoListRepository
    {
        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException("item.Id must not be empty", nameof(item));
            
            using (var file = new FileStream(userId.ToString(), FileMode.OpenOrCreate))
            using (var stream = new BinaryWriter(file))
            {
                stream.Write(item.Id.ToByteArray());
                stream.Write(item.Name);
                stream.Write(item.Description);
            }
        }

        public void DeleteItem(string userId, Guid itemId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            long position = 0;

            using (var file = new FileStream(userId.ToString(), FileMode.Open))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    position = stream.BaseStream.Position;
                    var id = Guid.Parse(stream.ReadBytes(16).ToString());
                    
                    if (id == itemId)
                        break;

                    var name = stream.ReadString();
                    var description = stream.ReadString();
                }
            }
            using (var file = new FileStream(userId.ToString(), FileMode.Open))
            using (var stream = new BinaryWriter(file))
            {
                stream.BaseStream.Seek(position, SeekOrigin.Current);
            }


                /*var item = _context.Items.SingleOrDefault(x => x.Id == itemId);

                if (item == null)
                    throw new KeyNotFoundException("Item was not found.");
                if (item.UserId != userId)
                    throw new SecurityException("User does not own the item.");

                _context.Items.Remove(item);
                _context.SaveChanges();*/
            }

        public TodoItem GetItemByUserAndId(string userId, Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            using (var file = new FileStream(userId.ToString(), FileMode.Open))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    var id = Guid.Parse(stream.ReadBytes(16).ToString());
                    var name = stream.ReadString();
                    var description = stream.ReadString();

                    if (id == itemId)
                        return new TodoItem { Id = id, Name = name, Description = description };
                }
                return null;
            }            
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            using (var file = new FileStream(userId.ToString(), FileMode.Open))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.CanRead)
                {
                    var id = Guid.Parse(stream.ReadBytes(16).ToString());
                    var name = stream.Read().ToString();
                    var description = stream.Read().ToString();

                    yield return new TodoItem { Id = id, Name = name, Description = description };
                }
            }
        }

        public void Update(string userId, TodoItem item)
        {
            throw new NotImplementedException();
        }

        /*
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
         */
    }
}
