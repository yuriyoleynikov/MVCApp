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
        private static TodoListModel todoListModel = new TodoListModel { Items = new List<TodoItemModel>() };
        private static int lastId = 0;
        
        public IActionResult Index()
        {
            return View(todoListModel);
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(TodoItemModel item)
        {
            item.Id = ++lastId;
            ((List<TodoItemModel>)todoListModel.Items).Add(item);            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            ((List<TodoItemModel>)todoListModel.Items).RemoveAll(item => item.Id == id);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var editModel = todoListModel.Items.Where(item => item.Id == id).SingleOrDefault();
            if (editModel == null)
                return NotFound();
            return View(editModel);
        }
        [HttpPost]
        public IActionResult Edit(TodoItemModel item)
        {
            foreach (var itemInList in todoListModel.Items)
            {
                if (itemInList.Id == item.Id)
                {
                    itemInList.Name = item.Name;
                    itemInList.Description = item.Description;
                    return RedirectToAction(nameof(Index));
                }
            }
            return NotFound();
        }
    }
}