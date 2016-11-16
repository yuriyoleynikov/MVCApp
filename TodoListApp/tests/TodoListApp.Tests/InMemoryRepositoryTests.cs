using System;
using TodoListApp.Models;
using TodoListApp.Tests;

namespace TodoApp.Services.Tests
{
    public class InMemoryRepositoryTests : RepositoryTests
    {
        private InMemoryTodoListRepository _repository = new InMemoryTodoListRepository();

        public override ITodoListRepository GetRepository() => _repository;
    }
}
