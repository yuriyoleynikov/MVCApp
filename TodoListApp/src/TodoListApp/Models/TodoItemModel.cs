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
        [Display(Name = "Task Name")]
        public string Name { get; set; }
        [Display(Name = "Task Description")]
        public string Description { get; set; }
    }    
}