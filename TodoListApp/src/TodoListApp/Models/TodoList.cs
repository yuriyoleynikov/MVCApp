﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class TodoList
    {
        public IEnumerable<TodoItem> Items { get; set; }
    }
}
