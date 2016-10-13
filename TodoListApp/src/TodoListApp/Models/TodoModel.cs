using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListApp.Models
{
    public class TodoItemModel
    {
        public int Id { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
    public class TodoListModel
    {
        public IEnumerable<TodoItemModel> Items { get; set; }
        public string Name { get; set; }
    }
}