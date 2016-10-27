using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;
using System.Security;

namespace TodoListApp.Controllers
{
    public class TodoController : Controller
    {
        private static ITodoListRepository memory = new InMemoryTodoListRepository();

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
            try
            {
                memory.DeleteItem("user", id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (SecurityException)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var editModel = memory.GetItemByUserAndId("user", id);
            if (editModel == null)
                return NotFound();
            return View(editModel);
        }

        [HttpPost]
        public IActionResult Edit(TodoItem item)
        {
            try
            {
                memory.Update("user", item);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (SecurityException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(Guid id)
        {
            var detailsModel = memory.GetItemByUserAndId("user", id);
            if (detailsModel == null)
                return NotFound();
            return View(detailsModel);
        }
    }
}