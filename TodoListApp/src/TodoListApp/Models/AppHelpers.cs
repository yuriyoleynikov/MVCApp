using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class AppHelpers
    {
        public static object ViewDataConvert(object obj)
        {
            var todoItemModel = obj as TodoItemModel;
            if (todoItemModel != null)
                return new TodoItem { Id = todoItemModel.Id, Name = todoItemModel.Name, Description = todoItemModel.Description };

            var todoItem = obj as TodoItem;
            if (todoItem != null)
                return new TodoItemModel { Id = todoItem.Id, Name = todoItem.Name, Description = todoItem.Description };

            throw new FormatException("Can not convert");
        }
    }
}
