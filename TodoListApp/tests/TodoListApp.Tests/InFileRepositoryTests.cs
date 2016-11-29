using System;
using System.IO;
using TodoListApp.Models;
using TodoListApp.Tests;

namespace TodoApp.Services.Tests
{
    public class InFileRepositoryTests : RepositoryTests, IDisposable
    {
        private string _path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        public void Dispose()
        {
            Directory.Delete(_path, true);
        }

        public override ITodoListRepository GetRepository()
        {
            Directory.CreateDirectory(_path);
            return new InFileTodoListRepository(_path);
        }
    }
}
