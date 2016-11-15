using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using TodoListApp.Models;
using Xunit;

namespace TodoListApp.Tests
{
    public abstract class RepositoryTests
    {
        public abstract ITodoListRepository GetRepository();

        private static bool CompareTodoItem(TodoListApp.Models.TodoItem item1, TodoListApp.Models.TodoItem item2) =>
            item1 == item2 || (item1.Id == item2.Id && item1.Name == item2.Name && item1.Description == item2.Description);

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenItIsNew()
        {
            ITodoListRepository repository = GetRepository();

            repository.GetTodoListByUser("user").Should().BeEmpty("nothing was added yet");
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.AddItem(null, new TodoItem())).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void AddItem_Fails_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.AddItem("user", null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddItem_Throws_WhenItemIdIsEmpty()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.AddItem("user", new TodoItem()))
                .ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("item");
        }

        [Fact]
        public void AddItem_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
        }

        [Fact]
        public void AddItem_SucceedsTwice_WhenTwoItemsCreated()
        {
            ITodoListRepository repository = GetRepository();

            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" });
            repository.AddItem("user", new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" });
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyUserIdPassed()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.DeleteItem(null, Guid.NewGuid()))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void DeleteItem_Throws_WhenEmptyItemIdPassed()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.DeleteItem("user", Guid.Empty))
                .ShouldThrow<ArgumentException>()
                .And.ParamName.Should().Be("itemId");
        }

        [Fact]
        public void DeleteItem_Throws_WhenItemIsMissing()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.DeleteItem("user", Guid.NewGuid()))
                .ShouldThrow<KeyNotFoundException>();
        }

        [Fact]
        public void DeleteItem_Throws_WhenUserMismatches()
        {
            ITodoListRepository repository = GetRepository();
            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            repository = GetRepository();
            new Action(() => repository.DeleteItem("user2", item.Id))
                .ShouldThrow<SecurityException>();
        }

        [Fact]
        public void DeleteItem_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = GetRepository();
            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            repository = GetRepository();
            new Action(() => repository.DeleteItem("user1", item.Id))
                .ShouldNotThrow();
        }

        [Fact]
        public void GetTodoListByUser_Throws_WhenNullPassed()
        {
            ITodoListRepository repository = GetRepository();

            new Action(() => repository.GetTodoListByUser(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAddedItem_WhenAdded()
        {
            ITodoListRepository repository = GetRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item);

            repository = GetRepository();
            repository.GetTodoListByUser("user").Should().Equal(new[] { item }, CompareTodoItem);
        }

        [Fact]
        public void GetTodoListByUser_ReturnsAllAddedItemsInTheSameOrder()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository = GetRepository();
            repository.GetTodoListByUser("user").Should().Equal(new[] { item1, item2, item3 }, CompareTodoItem);
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenAddedItemsForOtherUser()
        {
            ITodoListRepository repository = GetRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user1", item);

            repository = GetRepository();
            repository.GetTodoListByUser("user2").Should().BeEmpty();
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmpty_WhenSingleItemAddedAndDeleted()
        {
            ITodoListRepository repository = GetRepository();

            var item = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item);
            repository.DeleteItem("user", item.Id);

            repository = GetRepository();
            repository.GetTodoListByUser("user").Should().BeEmpty();
        }

        [Fact]
        public void GetTodoListByUser_ReturnsWithoutDeletedItem_WhenItemAddedAndDeleted()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);
            repository.DeleteItem("user", item2.Id);

            repository = GetRepository();
            repository.GetTodoListByUser("user").Should().Equal(new[] { item1, item3 }, CompareTodoItem);
        }

        [Fact]
        public void GetItemByUserAndId_ReturnsEmpty_WhenItIsNew()
        {
            ITodoListRepository repository = GetRepository();

            repository.GetItemByUserAndId("user", Guid.NewGuid()).Should().Be(null);
        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository = GetRepository();
            repository.GetItemByUserAndId("user", item1.Id).Should().Match<TodoListApp.Models.TodoItem>(x => CompareTodoItem(x, item1));
        }

        [Fact]
        public void GetItemByUserAndId_Succeeds_WhenUserNoHaveThisItemIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user2", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };
            repository.AddItem("user", item3);

            repository = GetRepository();
            repository.GetItemByUserAndId("user2", item1.Id).Should().Be(null);
        }

        [Fact]
        public void GetItemByUserAndId_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            repository = GetRepository();
            new Action(() => repository.GetItemByUserAndId(null, item1.Id)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void GetItemByUserAndId_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            repository = GetRepository();
            new Action(() => repository.GetItemByUserAndId("user", Guid.Empty)).ShouldThrow<ArgumentException>().And.ParamName.Should().Be("itemId");
        }

        [Fact]
        public void Update_Succeeds_WhenEverythingIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            var item3 = new TodoItem { Id = item1.Id, Name = "Item 2" };
            repository.Update("user", new TodoItem { Id = item1.Id, Description = item2.Description, Name = item2.Name });

            repository = GetRepository();
            repository.GetItemByUserAndId("user", item1.Id).Name.Should().Be("Item 2");
        }

        [Fact]
        public void Update_Fails_WhenNullAsUserIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            repository = GetRepository();
            new Action(() => repository.Update(null, item1)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("userId");
        }

        [Fact]
        public void Update_WhenNullAsItemIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);

            repository = GetRepository();
            new Action(() => repository.Update("user", null)).ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("item");
        }

        [Fact]
        public void Update_Fails_WhenUserNoHaveThisItemIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };
            repository.AddItem("user2", item2);
            var item3 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 3" };

            repository = GetRepository();
            new Action(() => repository.Update("user2", new TodoItem { Id = item1.Id, Description = item3.Description, Name = item3.Name }))
                .ShouldThrow<SecurityException>();
        }

        [Fact]
        public void Update_Fils_WhenImemIdNotFoundIsPassed()
        {
            ITodoListRepository repository = GetRepository();

            var item1 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 1" };
            repository.AddItem("user", item1);
            var item2 = new TodoItem { Id = Guid.NewGuid(), Name = "Item 2" };

            repository = GetRepository();
            new Action(() => repository.Update("user", new TodoItem { Id = Guid.NewGuid(), Description = item2.Description, Name = item2.Name }))
                .ShouldThrow<KeyNotFoundException>();
        }
    }
}
