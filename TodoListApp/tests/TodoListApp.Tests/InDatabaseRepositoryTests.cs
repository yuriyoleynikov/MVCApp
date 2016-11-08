using System;
using Xunit;
using FluentAssertions;
using TodoListApp.Models;
using System.Security;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApp.Services.Tests
{
    public class InDatabaseRepositoryTests
    {
        private static DbContextOptions<MyDbContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseInMemoryDatabase()
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        [Fact]
        public void Add_writes_to_database()
        {
            // All contexts that share the same service provider will share the same InMemory database
            var options = CreateNewContextOptions();
            

            // Run the test against one instance of the context
            using (var repository = new MyDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);
                var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

                service.AddItem("user", item);
                service.GetTodoListByUser("user").Should().Equal(item);
            }            
        }
        /*
        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenItIsNew()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            repository.GetTodoListByUser("user").Should().BeEmpty("nothing was added yet");
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.AddItem(null, new TodoItem())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.AddItem("user", null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddItem_Throws_WhenItemIdIsEmpty()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.AddItem("user", new TodoItem()))
                .ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("item");
        }

        [Fact]
        public void AddItem_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
        }

        [Fact]
        public void AddItem_SucceedsTwice_WhenTwoItemsCreated()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" });
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyUserIdPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.DeleteItem(null, Guid.NewGuid()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyItemIdPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.DeleteItem("user", Guid.Empty))
                .ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("itemId");
        }

        [Fact]
        public void DeleteItem_Throws_WhenItemIsMissing()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.DeleteItem("user", Guid.NewGuid()))
                .ShouldThrow<KeyNotFoundException>();
        }

        [Fact]
        public void DeleteItem_Throws_WhenUserMismatches()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();
            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            new Action(() => repository.DeleteItem("user2", item.Id))
                .ShouldThrow<SecurityException>();
        }

        [Fact]
        public void DeleteItem_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();
            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            new Action(() => repository.DeleteItem("user1", item.Id))
                .ShouldNotThrow();
        }

        [Fact]
        public void GetTodoListByUser_Throws_WhenNullPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            new Action(() => repository.GetTodoListByUser(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAddedItem_WhenAdded()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item);

            repository.GetTodoListByUser("user").Should().Equal(item);
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAllAddedItemsInTheSameOrder()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository.GetTodoListByUser("user").Should().Equal(item1, item2, item3);
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenAddedItemsForOtherUser()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            repository.GetTodoListByUser("user2").Should().BeEmpty();
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenSingleItemAddedAndDeleted()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item);
            repository.DeleteItem("user", item.Id);

            repository.GetTodoListByUser("user").Should().BeEmpty();
        }

        [Fact]
        public void GetTodoListByUser_ReturnsWithoutDeletedItem_WhenItemAddedAndDeleted()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);
            repository.DeleteItem("user", item2.Id);

            repository.GetTodoListByUser("user").Should().Equal(item1, item3);
        }

        [Fact]
        public void GetItemByUserAndId_ReturnsEmpty_WhenItIsNew()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            repository.GetItemByUserAndId("user", Guid.NewGuid()).Should().Be(null);
        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository.GetItemByUserAndId("user", item1.Id).Should().Be(item1);

        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenUserNoHaveThisItemIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user2", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository.GetItemByUserAndId("user2", item1.Id).Should().Be(null);
        }

        [Fact]
        public void GetItemByUserAndId_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            new Action(() => repository.GetItemByUserAndId(null, item1.Id)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void GetItemByUserAndId_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            new Action(() => repository.GetItemByUserAndId("user", Guid.Empty)).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("itemId");
        }

        [Fact]
        public void Update_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoItem { Id = item1.Id, Name = "Item 2" };
            repository.Update("user", new TodoItem { Id = item1.Id, Description = item2.Description, Name = item2.Name });

            repository.GetItemByUserAndId("user", item1.Id).Name.Should().Be("Item 2");
        }

        [Fact]
        public void Update_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            new Action(() => repository.Update(null, item1)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void Update_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            new Action(() => repository.Update("user", null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("item");
        }

        [Fact]
        public void Update_Fails_WhenUserNoHaveThisItemIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user2", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            new Action(() =>
            repository.Update("user2", new TodoItem { Id = item1.Id, Description = item3.Description, Name = item3.Name }))
            .ShouldThrow<SecurityException>();
        }

        [Fact]
        public void Update_Fils_WhenImemIdNotFoundIsPassed()
        {
            ITodoListRepository repository = new InMemoryTodoListRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };

            new Action(() =>
            repository.Update("user", new TodoItem { Id = Guid.NewGuid(), Description = item2.Description, Name = item2.Name }))
            .ShouldThrow<KeyNotFoundException>();
        }
        */
    }
}
