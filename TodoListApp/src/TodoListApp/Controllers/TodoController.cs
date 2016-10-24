using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;

namespace TodoListApp.Controllers
{
    public class TodoController : Controller
    {
        private static InMemoryTodoListRepository memory = new InMemoryTodoListRepository();

        public IActionResult Index()
        {
            return View(new TodoList { Items = memory.GetTodoListByUser("user") });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TodoItem item)
        {
            item.Id = Guid.NewGuid();
            memory.AddItem("user", item);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(Guid id)
        {
            memory.DeleteItem(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            throw new FormatException("Edit not work");
            /*var editModel = todoListModel.Items.Where(item => item.Id == id).SingleOrDefault();
            if (editModel == null)
                return NotFound();
            return View(editModel);*/
        }

        [HttpPost]
        public IActionResult Edit(TodoItem item)
        {
            throw new FormatException("Edit not work");
            /*foreach (var itemInList in todoListModel.Items)
            {
                if (itemInList.Id == item.Id)
                {
                    itemInList.Name = item.Name;
                    itemInList.Description = item.Description;
                    return RedirectToAction(nameof(Index));
                }
            }
            return NotFound();*/
        }
    }
}