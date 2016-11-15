using TodoListApp.Models;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using Microsoft.Extensions.DependencyInjection;
using TodoListApp.Tests;

namespace TodoApp.Services.Tests
{
    public class InDatabaseRepositoryTests : RepositoryTests
    {   
        private TodoListDbContext _context = new TodoListDbContext(CreateNewContextOptions());

        public override ITodoListRepository GetRepository() => new InDatebaseTodoListRepository(_context);
        private static DbContextOptions<TodoListDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<TodoListDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        } 
    }
}
