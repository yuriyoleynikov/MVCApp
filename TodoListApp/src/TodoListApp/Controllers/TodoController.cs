using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;
using System.Security;
using Microsoft.AspNetCore.Authorization;

namespace TodoListApp.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private static ITodoListRepository memory = new InMemoryTodoListRepository();
        
        public IActionResult Index()
        {
            return View(new TodoList { Items = memory.GetTodoListByUser(User.Identity.Name) });
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
            memory.AddItem(User.Identity.Name, item);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Delete(Guid id)
        {
            try
            {
                memory.DeleteItem(User.Identity.Name, id);
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
            var editModel = memory.GetItemByUserAndId(User.Identity.Name, id);
            if (editModel == null)
                return NotFound();
            return View(editModel);
        }
        
        [HttpPost]
        public IActionResult Edit(TodoItem item)
        {
            try
            {
                memory.Update(User.Identity.Name, item);
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
            var detailsModel = memory.GetItemByUserAndId(User.Identity.Name, id);
            if (detailsModel == null)
                return NotFound();
            return View(detailsModel);
        }
    }
}