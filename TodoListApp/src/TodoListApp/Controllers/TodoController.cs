using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Models;
using System.Security;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TodoListApp.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {

        private static ITodoListRepository memory = new InDatebaseTodoListRepository();

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public IActionResult Index()
        {
            return View(new TodoList { Items = memory.GetTodoListByUser(UserId) });
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
            memory.AddItem(UserId, item);
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Delete(Guid id)
        {
            try
            {
                memory.DeleteItem(UserId, id);
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
            var editModel = memory.GetItemByUserAndId(UserId, id);
            if (editModel == null)
                return NotFound();
            return View(editModel);
        }
        
        [HttpPost]
        public IActionResult Edit(TodoItem item)
        {
            try
            {
                memory.Update(UserId, item);
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
            var detailsModel = memory.GetItemByUserAndId(UserId, id);
            if (detailsModel == null)
                return NotFound();
            return View(detailsModel);
        }
    }
}