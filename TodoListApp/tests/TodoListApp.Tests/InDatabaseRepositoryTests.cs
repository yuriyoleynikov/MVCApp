﻿using System;
using Xunit;
using FluentAssertions;
using TodoListApp.Models;
using System.Security;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace TodoApp.Services.Tests
{
    public class InDatabaseRepositoryTests
    {
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
        private static bool CompareTodoItem(TodoListApp.Models.TodoItem item1, TodoListApp.Models.TodoItem item2) =>
            item1 == item2 || (item1.Id == item2.Id && item1.Name == item2.Name && item1.Description == item2.Description);

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenItIsNew()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user").Should().BeEmpty("nothing was added yet");
            }
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsUserIsPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.AddItem(null, new TodoListApp.Models.TodoItem())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
            }
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsItemIsPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.AddItem("user", null)).ShouldThrow<ArgumentNullException>();
            }
        }

        [Fact]
        public void AddItem_Throws_WhenItemIdIsEmpty()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.AddItem("user", new TodoListApp.Models.TodoItem()))
                    .ShouldThrow<ArgumentException>()
                    .And.ParamName.Should().Be("item");
            }
        }

        [Fact]
        public void AddItem_Succeeds_WhenEverythingIsPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
            }
        }

        [Fact]
        public void AddItem_SucceedsTwice_WhenTwoItemsCreated()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
                service.AddItem("user", new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" });
            }
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyUserIdPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.DeleteItem(null, Guid.NewGuid()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
            }
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyItemIdPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.DeleteItem("user", Guid.Empty))
                .ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("itemId");
            }
        }

        [Fact]
        public void DeleteItem_Throws_WhenItemIsMissing()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.DeleteItem("user", Guid.NewGuid()))
                .ShouldThrow<KeyNotFoundException>();
            }
        }

        [Fact]
        public void DeleteItem_Throws_WhenUserMismatches()
        {
            var options = CreateNewContextOptions();
            var item = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user1", item);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.DeleteItem("user2", item.Id))
                .ShouldThrow<SecurityException>();
            }
        }

        [Fact]
        public void DeleteItem_Succeeds_WhenEverythingIsPassed()
        {
            var options = CreateNewContextOptions();
            var item = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user1", item);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.DeleteItem("user1", item.Id))
                .ShouldNotThrow();
            }
        }

        [Fact]
        public void GetTodoListByUser_Throws_WhenNullPassed()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.GetTodoListByUser(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAddedItem_WhenAdded()
        {
            var options = CreateNewContextOptions();
            var item = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user").Should().Equal(new[] { item }, CompareTodoItem);
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAllAddedItemsInTheSameOrder()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.AddItem("user", item2);
                service.AddItem("user", item3);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user").Should().Equal(new[] { item1, item2, item3 }, CompareTodoItem);
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenAddedItemsForOtherUser()
        {
            var options = CreateNewContextOptions();
            var item = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user1", item);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user2").Should().BeEmpty();
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenSingleItemAddedAndDeleted()
        {
            var options = CreateNewContextOptions();
            var item = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item);
                service.DeleteItem("user", item.Id);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user").Should().BeEmpty();
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsWithoutDeletedItem_WhenItemAddedAndDeleted()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.AddItem("user", item2);
                service.AddItem("user", item3);
                service.DeleteItem("user", item2.Id);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetTodoListByUser("user").Should().Equal(new[] { item1, item3 }, CompareTodoItem);
            }
        }

        [Fact]
        public void GetItemByUserAndId_ReturnsEmpty_WhenItIsNew()
        {
            var options = CreateNewContextOptions();

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetItemByUserAndId("user", Guid.NewGuid()).Should().Be(null);
            }
        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenEverythingIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.AddItem("user", item2);
                service.AddItem("user", item3);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetItemByUserAndId("user", item1.Id).Should().Match<TodoListApp.Models.TodoItem>(x => CompareTodoItem(x, item1));
            }
        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenUserNoHaveThisItemIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.AddItem("user2", item2);
                service.AddItem("user", item3);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetItemByUserAndId("user2", item1.Id).Should().Be(null);
            }
        }

        [Fact]
        public void GetItemByUserAndId_Fails_WhenNullAsUserIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.GetItemByUserAndId(null, item1.Id)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
            }
        }

        [Fact]
        public void GetItemByUserAndId_WhenNullAsItemIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.GetItemByUserAndId("user", Guid.Empty)).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("itemId");
            }
        }

        [Fact]
        public void Update_Succeeds_WhenEverythingIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = item1.Id, Name = "Item 2" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.Update("user", new TodoListApp.Models.TodoItem { Id = item1.Id, Description = item2.Description, Name = item2.Name });
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.GetItemByUserAndId("user", item1.Id).Name.Should().Be("Item 2");
            }
        }

        [Fact]
        public void Update_Fails_WhenNullAsUserIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.Update(null, item1)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
            }
        }

        [Fact]
        public void Update_WhenNullAsItemIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service.Update("user", null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("item");
            }
        }

        [Fact]
        public void Update_Fails_WhenUserNoHaveThisItemIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
                service.AddItem("user2", item2);
            }
            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() =>
            service.Update("user2", new TodoListApp.Models.TodoItem { Id = item1.Id, Description = item3.Description, Name = item3.Name }))
            .ShouldThrow<SecurityException>();
            }
        }

        [Fact]
        public void Update_Fils_WhenImemIdNotFoundIsPassed()
        {
            var options = CreateNewContextOptions();
            var item1 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            var item2 = new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                service.AddItem("user", item1);
            }

            using (var repository = new TodoListDbContext(options))
            {
                var service = new InDatebaseTodoListRepository(repository);

                new Action(() => service
                .Update("user", new TodoListApp.Models.TodoItem { Id = Guid.NewGuid(), Description = item2.Description, Name = item2.Name }))
                .ShouldThrow<KeyNotFoundException>();
            }
        }
    }
}