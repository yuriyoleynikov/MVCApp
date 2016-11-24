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
        private string _dataDirectory;

        public InFileTodoListRepository(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        private string GetFileName(string userId) => Path.Combine(_dataDirectory, userId + ".yodat");

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException("item.Id must not be empty", nameof(item));

            using (var file = new FileStream(GetFileName(userId), FileMode.Append, FileAccess.Write))
            using (var stream = new BinaryWriter(file))
            {
                stream.Write(item.Id.ToByteArray());

                stream.Write(item.Name != null);
                if (item.Name != null)
                    stream.Write(item.Name);

                stream.Write(item.Description != null);
                if (item.Description != null)
                    stream.Write(item.Description);
            }
        }

        public void DeleteItem(string userId, Guid itemId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            var tempFileName = Path.GetTempFileName();
            var deleted = false;

            if (!File.Exists(GetFileName(userId)))
                throw new KeyNotFoundException("Item was not found.");

            using (var temp = new FileStream(tempFileName, FileMode.Create, FileAccess.Write))
            using (var file = new FileStream(GetFileName(userId), FileMode.Open, FileAccess.Read))
            using (var stream = new BinaryReader(file))
            using (var temp_stream = new BinaryWriter(temp))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    var id = new Guid(stream.ReadBytes(16));
                    var name = stream.ReadBoolean() ? stream.ReadString() : null;
                    var description = stream.ReadBoolean() ? stream.ReadString() : null;

                    if (id != itemId)
                    {
                        temp_stream.Write(id.ToByteArray());

                        temp_stream.Write(name != null);
                        if (name != null)
                            temp_stream.Write(name);

                        temp_stream.Write(description != null);
                        if (description != null)
                            temp_stream.Write(description);
                    }
                    else
                        deleted = true;
                }
            }
            if (!deleted)
                throw new KeyNotFoundException("Item was not found.");
            File.Delete(GetFileName(userId));
            File.Move(tempFileName, GetFileName(userId));
        }

        public TodoItem GetItemByUserAndId(string userId, Guid itemId)
        {
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            if (!File.Exists(GetFileName(userId)))
                return null;

            using (var file = new FileStream(GetFileName(userId), FileMode.Open, FileAccess.Read))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    var id = new Guid(stream.ReadBytes(16));
                    var name = stream.ReadBoolean() ? stream.ReadString() : null;
                    var description = stream.ReadBoolean() ? stream.ReadString() : null;

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
            return GetTodoListByUserInner(userId);
        }

        public IEnumerable<TodoItem> GetTodoListByUserInner(string userId)
        {
            if (!File.Exists(GetFileName(userId)))
                yield break;

            using (var file = new FileStream(GetFileName(userId), FileMode.Open, FileAccess.Read))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    var id = new Guid(stream.ReadBytes(16));
                    var name = stream.ReadBoolean() ? stream.ReadString() : null;
                    var description = stream.ReadBoolean() ? stream.ReadString() : null;

                    yield return new TodoItem { Id = id, Name = name, Description = description };
                }
                yield break;
            }
        }

        public void Update(string userId, TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            if (!File.Exists(GetFileName(userId)))
                throw new KeyNotFoundException("Item was not found.");

            long position = -1;

            using (var file = new FileStream(GetFileName(userId), FileMode.Open, FileAccess.Read))
            using (var stream = new BinaryReader(file))
            {
                while (stream.BaseStream.Position < stream.BaseStream.Length)
                {
                    var id = new Guid(stream.ReadBytes(16));

                    var _position = stream.BaseStream.Position;

                    var name = stream.ReadBoolean() ? stream.ReadString() : null;
                    var description = stream.ReadBoolean() ? stream.ReadString() : null;

                    if (id == item.Id)
                    {
                        position = _position;
                        break;
                    }
                }
            }

            if (position != -1)
                using (var file = new FileStream(GetFileName(userId), FileMode.Open, FileAccess.Write))
                using (var stream = new BinaryWriter(file))
                {
                    stream.BaseStream.Seek(position, SeekOrigin.Begin);

                    stream.Write(item.Name != null);
                    if (item.Name != null)
                        stream.Write(item.Name);

                    stream.Write(item.Description != null);
                    if (item.Description != null)
                        stream.Write(item.Description);
                }
            else
                throw new KeyNotFoundException("Item was not found.");
        }
    }
}
