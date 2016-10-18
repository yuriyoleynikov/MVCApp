using System;
using Xunit;
using FluentAssertions;
using TodoListApp.Models;

namespace TodoApp.Services.Tests
{
	public class InMemoryRepositoryTests
	{
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
			repository.DeleteItem(item.Id);

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
			repository.DeleteItem(item2.Id);

			repository.GetTodoListByUser("user").Should().Equal(item1, item3);
		}
	}
}
