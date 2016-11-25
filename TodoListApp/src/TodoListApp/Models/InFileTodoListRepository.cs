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

        private bool NotEndOfStream(BinaryReader stream) => stream.BaseStream.Position < stream.BaseStream.Length;

        private TodoItem ReadTodoItem(BinaryReader stream)
        {
            var id = new Guid(stream.ReadBytes(16));
            var name = stream.ReadBoolean() ? stream.ReadString() : null;
            var description = stream.ReadBoolean() ? stream.ReadString() : null;

            return new TodoItem { Id = id, Name = name, Description = description };
        }

        private void WriteTodoItem(BinaryWriter stream, TodoItem item)
        {
            stream.Write(item.Id.ToByteArray());

            stream.Write(item.Name != null);
            if (item.Name != null)
                stream.Write(item.Name);

            stream.Write(item.Description != null);
            if (item.Description != null)
                stream.Write(item.Description);
        }

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException("item.Id must not be empty", nameof(item));

            using (var stream = new BinaryWriter(File.Open(GetFileName(userId), FileMode.Append, FileAccess.Write)))
                WriteTodoItem(stream, item);
        }

        public void DeleteItem(string userId, Guid itemId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (itemId == Guid.Empty)
                throw new ArgumentException("itemId must not be empty", nameof(itemId));

            if (!File.Exists(GetFileName(userId)))
                throw new KeyNotFoundException("Item was not found.");

            var tempFileName = Path.GetTempFileName();
            var deleted = false;

            using (var stream = new BinaryReader(File.Open(GetFileName(userId), FileMode.Open, FileAccess.Read)))
            using (var tempStream = new BinaryWriter(File.Open(tempFileName, FileMode.Create, FileAccess.Write)))
                while (NotEndOfStream(stream))
                {
                    var item = ReadTodoItem(stream);

                    if (item.Id != itemId)
                        WriteTodoItem(tempStream, item);
                    else
                        deleted = true;
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

            using (var stream = new BinaryReader(File.Open(GetFileName(userId), FileMode.Open, FileAccess.Read)))
                while (NotEndOfStream(stream))
                {
                    var item = ReadTodoItem(stream);

                    if (item.Id == itemId)
                        return item;
                }
            return null;
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

            using (var stream = new BinaryReader(File.Open(GetFileName(userId), FileMode.Open, FileAccess.Read)))
                while (NotEndOfStream(stream))
                    yield return ReadTodoItem(stream);
            yield break;
        }

        public void Update(string userId, TodoItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));

            if (!File.Exists(GetFileName(userId)))
                throw new KeyNotFoundException("Item was not found.");

            var tempFileName = Path.GetTempFileName();
            var willUpdate = false;

            using (var stream = new BinaryReader(File.Open(GetFileName(userId), FileMode.Open, FileAccess.Read)))
            using (var tempStream = new BinaryWriter(File.Open(tempFileName, FileMode.Create, FileAccess.Write)))
                while (NotEndOfStream(stream))
                {
                    var _item = ReadTodoItem(stream);

                    if (_item.Id != item.Id)
                        WriteTodoItem(tempStream, _item);
                    else
                        willUpdate = true;
                }

            if (!willUpdate)
                throw new KeyNotFoundException("Item was not found.");

            File.Delete(GetFileName(userId));
            File.Move(tempFileName, GetFileName(userId));

            using (var stream = new BinaryWriter(File.Open(GetFileName(userId), FileMode.Append, FileAccess.Write)))
                WriteTodoItem(stream, item);
        }
    }
}