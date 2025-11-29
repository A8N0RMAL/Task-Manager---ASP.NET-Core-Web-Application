using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tasks

        public async Task<IActionResult> Index(string searchString,
                                                string categoryFilter,
                                                string priorityFilter,
                                                string statusFilter)
        {
            var tasks = _context.Tasks.AsQueryable(); // no DB hit

            // Search by Title or Description
            if(!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchString) ||
                                         t.Description.Contains(searchString));
            }

            // Filter by category
            if(!string.IsNullOrEmpty(categoryFilter))
            {
                tasks = tasks.Where(t => t.Category == categoryFilter);
            }

            // Filter by priority
            if (!string.IsNullOrEmpty(priorityFilter))
            {
                tasks = priorityFilter switch
                {
                    "Low" => tasks.Where(t => t.Priority == Priority.Low),
                    "Medium" => tasks.Where(t => t.Priority == Priority.Medium),
                    "High" => tasks.Where(t => t.Priority == Priority.High),
                    "Urgent" => tasks.Where(t => t.Priority == Priority.Urgent),
                    _ => tasks
                };
            }

            // Filter by completion status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool isCompleted = statusFilter == "Completed"; // assume only "Completed" or "Incomplete"
                tasks = tasks.Where(t => t.IsCompleted == isCompleted);
            }

            // Get unique categories for dropdown
            ViewBag.Categories = await _context.Tasks
                                            .Where(t => !string.IsNullOrEmpty(t.Category))
                                            .Select(t => t.Category)
                                            .Distinct()
                                            .ToListAsync();

            var result = await tasks.OrderByDescending(t => t.Priority)
                                    .ThenBy(t => t.DueDate)
                                    .ToListAsync();

            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryFilter;
            ViewData["PriorityFilter"] = priorityFilter;
            ViewData["StatusFilter"] = statusFilter;

            return View(result);
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if(task is null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task)
        {
            if(ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if(task is null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem task)
        {
            if(id != task.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Task updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if(!TaskItemExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(task);
        }

        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if(task is null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = _context.Tasks.FindAsync(id);

            if(task != null)
            {
                _context.Tasks.Remove(task.Result);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
