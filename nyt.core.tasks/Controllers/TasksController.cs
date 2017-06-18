using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nyt.core.tasks.Models;

namespace nyt.core.tasks.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private Models.TaskContext context;

        public TasksController(Models.TaskContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public Task<task[]> Get()
        {
            return this.context.tasks.OrderByDescending(o => o.updated).Take(10).ToArrayAsync();
        }

        [HttpGet("{id}")]
        public Task<task> Get(Guid id)
        {
            return this.context.tasks.Where(t => t.id == id).SingleAsync();
        }

        [HttpPost]
        public Task Post([FromBody]task task)
        {
            if (task.id == Guid.Empty)
                task.id = Guid.NewGuid();
            task.created = DateTime.Now;
            task.updated = DateTime.Now;
            this.context.tasks.Add(task);
            return this.context.SaveChangesAsync();
        }

        [HttpPut("{id}")]
        public Task Put(Guid id, [FromBody]task task)
        {
            task.id = id;
            task.updated = DateTime.Now;
            var entry = this.context.Entry(task);
            entry.State = EntityState.Modified;
            entry.Property(o => o.created).IsModified = false;
            entry.Property(o => o.creator).IsModified = false;
            return this.context.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public Task Delete(Guid id)
        {
            this.context.Entry(new task() { id = id }).State = EntityState.Deleted;
            return this.context.SaveChangesAsync();
        }
    }
}
