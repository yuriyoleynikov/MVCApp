﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        private static IDictionary<string, TodoList> _todoListByUser = new Dictionary<string, TodoList>();

        public void AddItem(string userId, TodoItem item)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Id == default(Guid))
                throw new ArgumentException(nameof(item));
            TodoList todoList;
            if (!_todoListByUser.TryGetValue(userId, out todoList))
            {
                todoList = new TodoList { Items = new List<TodoItem>() };
                _todoListByUser.Add(userId, todoList);
            }
            ((List<TodoItem>)todoList.Items).Add(item);
        }

        public void DeleteItem(Guid itemId)
        {
            if (itemId== Guid.Empty)
                throw new ArgumentException(nameof(itemId));
            foreach (var user in _todoListByUser)
                foreach (var item in user.Value.Items)
                    if (item.Id == itemId)
                    {
                        ((List<TodoItem>)user.Value.Items).Remove(item);
                        return;
                    }
        }

        public IEnumerable<TodoItem> GetTodoListByUser(string userId)
        {
            foreach (var user in _todoListByUser)
                if (user.Key == userId)
                    foreach (var item in user.Value.Items)
                        yield return item;
        }
    }
}
