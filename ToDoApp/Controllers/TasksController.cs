using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;
using ToDoApp.Filter;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

[AuthenAttribute]
public class TasksController : Controller
{
    private readonly ToDoAppContext _context;
    private readonly IDistributedCache _cache;

    public TasksController(ToDoAppContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: TASKS
    public async Task<IActionResult> Index(int? userId)    
    {
        userId = HttpContext.Session.GetInt32("curUserId");
        //List<TaskModel> task = task = await _context.Tasks.Where(m => m.UserId == userId).OrderBy(m => m.Date).ToListAsync();
        List<TaskModel>? task;
        string cacheKey = $"task_user_{userId}";
        string? cacheData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cacheData))
        {
            task = JsonSerializer.Deserialize<List<TaskModel>>(cacheData);
        }
        else
        {
            task = await _context.Tasks.Where(m => m.UserId == userId).OrderBy(m => m.Date).ToListAsync();
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            string dataToCache = JsonSerializer.Serialize(task);
            await _cache.SetStringAsync(cacheKey, dataToCache, cacheOptions);
        }
        return View(task);
        //List<TaskModel> tasks = await _context.Tasks.FromSqlRaw("""
        //    select * 
        //    from "Tasks"
        //    order by "Date"
        //    """).ToListAsync();
        //return View(tasks);
    }

    // GET: TASKS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TASKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Date,Status")] TaskModel task)
    {
        var userId = HttpContext.Session.GetInt32("curUserId");
        if (task.Date < DateTime.UtcNow)
        {
            ModelState.AddModelError("Date", "Can not choose a date from the past");
        }
        if (ModelState.IsValid)
        {
            task.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
            task.UserId = userId.Value;
            _context.Add(task);
            await _context.SaveChangesAsync();
            string cacheKey = $"task_user_{userId}";
            await _cache.RemoveAsync(cacheKey);
            //var name = task.Name;
            //var date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
            //var status = task.Status;
            //await _context.Database.ExecuteSqlInterpolatedAsync($"""
            //    insert into "Tasks" ("Name", "Date", "Status")
            //    values ({name}, {date}, {status})
            //    """);
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: TASKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var userId = HttpContext.Session.GetInt32("curUserId");
        string cacheKey = $"task_detail_{id}_user_{userId}";
        TaskModel? task;
        string? cacheData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cacheData))
        {
            task = JsonSerializer.Deserialize<TaskModel>(cacheData);
        }
        else
        {
            task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if(task == null)
            {
                return NotFound();
            }
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            string dataToCache = JsonSerializer.Serialize(task);
            await _cache.SetStringAsync(cacheKey, dataToCache, cacheOptions);
        }
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();
        return View(task);
    }

    // POST: TASKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Date,Status")] TaskModel task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }
        var userId = HttpContext.Session.GetInt32("curUserId");
        if (ModelState.IsValid)
        {
            var exisTask = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if(exisTask == null)
            {
                return NotFound();
            }
            exisTask.Name = task.Name;
            exisTask.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"task_user_{userId}");
            await _cache.RemoveAsync($"task_detail_{id}_user_{userId}");
            //var name = task.Name;
            //var date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
            //var status = task.Status;
            //await _context.Database.ExecuteSqlInterpolatedAsync($"""
            //    update "Tasks"
            //    set "Name" = {name}, "Date" = {date}, "Status" = {status}
            //    where "Id" = {id}
            //    """);
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: TASKS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var userId = HttpContext.Session.GetInt32("curUserId");
        string cacheKey = $"task_detail_{id}_user_{userId}";
        TaskModel? task;
        string? cacheData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cacheData))
        {
            task = JsonSerializer.Deserialize<TaskModel>(cacheData);
        }
        else
        {
            task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
            if (task == null)
            {
                return NotFound();
            }
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            string dataToCache = JsonSerializer.Serialize(task);
            await _cache.SetStringAsync(cacheKey, dataToCache, cacheOptions);
        }
        
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();

        return View(task);
    }

    // POST: TASKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var userId = HttpContext.Session.GetInt32("curUserId");
        var task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync($"task_user_{userId}");
            await _cache.RemoveAsync($"task_detail_{id}_user_{userId}");
            //await _context.Database.ExecuteSqlInterpolatedAsync($"""
            //    delete
            //    from "Tasks"
            //    where "Id" = {id}
            //    """);
        }
        else
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UpdateTaskStatus(int? id, bool status)
    {
        var userId = HttpContext.Session.GetInt32("curUserId");
        var task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if(task == null)
        {
            return NotFound();
        }
        task.Status = status;
        DateTime? completedDate = null;
        if (task.Status)
        {
            task.CompletedDate = DateTime.UtcNow;
            completedDate = task.CompletedDate;
        }
        await _context.SaveChangesAsync();
        await _cache.RemoveAsync($"task_user_{userId}");
        await _cache.RemoveAsync($"task_detail_{id}_user_{userId}");
        return Json(new
        {
            success = true,
            completedDate = completedDate?.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
        });
        //await _context.Database.ExecuteSqlInterpolatedAsync($"""
        //    update "Tasks"
        //    set "Status" = {status}
        //    where "Id" = {id}
        //    """);
    }

    private bool TaskExists(int? id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }
}
