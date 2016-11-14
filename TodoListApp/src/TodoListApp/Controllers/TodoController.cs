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
        private ITodoListRepository _todoListRepository;
        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        public TodoController(ITodoListRepository todoListRepository)
        {
            _todoListRepository = todoListRepository;
        }

        public IActionResult Index()
        {
            return View(new TodoList { Items = _todoListRepository.GetTodoListByUser(UserId) });
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
            _todoListRepository.AddItem(UserId, item);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(Guid id)
        {
            try
            {
                _todoListRepository.DeleteItem(UserId, id);
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
            var editModel = _todoListRepository.GetItemByUserAndId(UserId, id);
            if (editModel == null)
                return NotFound();
            return View(AppHelpers.ViewDataConvert(editModel));            
        }

        [HttpPost]
        public IActionResult Edit(TodoItem item)
        {
            try
            {
                _todoListRepository.Update(UserId, item);
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
            var detailsModel = _todoListRepository.GetItemByUserAndId(UserId, id);
            if (detailsModel == null)
                return NotFound();
            return View(AppHelpers.ViewDataConvert(detailsModel));
        }
    }
}