using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;
using ToDoApp.Filter;

[AuthenAttribute]
public class TasksController : Controller
{
    private readonly ToDoAppContext _context;

    public TasksController(ToDoAppContext context)
    {
        _context = context;
    }

    // GET: TASKS
    public async Task<IActionResult> Index(int? userId)    
    {
        userId = HttpContext.Session.GetInt32("curUserId");
        return View(await _context.Tasks.Where(m => m.UserId == userId).OrderBy(m => m.Date).ToListAsync());
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
    public async Task<IActionResult> Create([Bind("Id,Name,Date,Status, UserId")] TaskModel task)
    {
        if(task.Date < DateTime.UtcNow)
        {
            ModelState.AddModelError("Date", "Can not choose a date from the past");
        }
        if (ModelState.IsValid)
        {
            task.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
            task.UserId = HttpContext.Session.GetInt32("curUserId").Value;
            _context.Add(task);
            await _context.SaveChangesAsync();
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

        var task = await _context.Tasks.FindAsync(id);
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();
        if (task == null)
        {
            return NotFound();
        }
        return View(task);
    }

    // POST: TASKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Date,Status, UserId")] TaskModel task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                task.Date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
                task.UserId = HttpContext.Session.GetInt32("curUserId").Value;
                _context.Update(task);
                await _context.SaveChangesAsync();
                //var name = task.Name;
                //var date = DateTime.SpecifyKind(task.Date, DateTimeKind.Utc);
                //var status = task.Status;
                //await _context.Database.ExecuteSqlInterpolatedAsync($"""
                //    update "Tasks"
                //    set "Name" = {name}, "Date" = {date}, "Status" = {status}
                //    where "Id" = {id}
                //    """);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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

        var task = await _context.Tasks
            .FirstOrDefaultAsync(m => m.Id == id);
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // POST: TASKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var task = await _context.Tasks.FindAsync(id);
        //var task = await _context.Tasks.FromSqlInterpolated($"""
        //    select *
        //    from "Tasks"
        //    where "Id" = {id}
        //    """).FirstOrDefaultAsync();
        if (task != null)
        {
            _context.Tasks.Remove(task);
            //await _context.Database.ExecuteSqlInterpolatedAsync($"""
            //    delete
            //    from "Tasks"
            //    where "Id" = {id}
            //    """);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UpdateTaskStatus(int? id, bool status)
    {
        var task = await _context.Tasks.FindAsync(id);
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
