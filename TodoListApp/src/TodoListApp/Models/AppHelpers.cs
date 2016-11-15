using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class AppHelpers
    {
        public static object FromViewToData(TodoItemModel item) =>
            new TodoItem { Id = item.Id, Name = item.Name, Description = item.Description };

        public static object FromDataToView(TodoItem item) =>
             new TodoItemModel { Id = item.Id, Name = item.Name, Description = item.Description };
    }
}
