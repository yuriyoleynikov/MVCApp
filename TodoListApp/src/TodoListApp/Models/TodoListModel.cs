using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class TodoListModel
    {
        public IEnumerable<TodoItemModel> Items { get; set; }
        public string Name { get; set; }
    }
}
