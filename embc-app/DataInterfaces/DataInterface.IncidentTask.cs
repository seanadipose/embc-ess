using Gov.Jag.Embc.Public.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Jag.Embc.Public.DataInterfaces
{
    public partial class DataInterface
    {
        public async Task<IEnumerable<IncidentTask>> GetIncidentTasks()
        {
            var all = await db.IncidentTasks
                .Include(t => t.Region)
                .Include(t => t.RegionalDistrict)
                    .ThenInclude(d => d.Region)
                .Include(t => t.Community)
                    .ThenInclude(c => c.RegionalDistrict)
                        .ThenInclude(d => d.Region)
                .ToArrayAsync();

            return all.Select(task => task.ToViewModel());
        }

        public Task<IncidentTask> GetIncidentTask(string id)
        {
            if (Guid.TryParse(id, out var guid))
            {
                var entity = db.IncidentTasks.FirstOrDefault(task => task.Id == guid);
                return Task.FromResult(entity?.ToViewModel());
            }
            return Task.FromResult<IncidentTask>(null);
        }

        public Task<IncidentTask> CreateIncidentTask(IncidentTask task)
        {
            var entity = task.ToModel();
            db.IncidentTasks.Add(entity);
            db.SaveChanges();
            return Task.FromResult(entity.ToViewModel());
        }

        public Task<IncidentTask> UpdateIncidentTask(IncidentTask task)
        {
            var entity = db.IncidentTasks.FirstOrDefault(item => item.Id == new Guid(task.Id));
            entity.PatchValues(task);
            db.IncidentTasks.Update(entity);
            db.SaveChanges();
            return Task.FromResult(entity.ToViewModel());
        }
    }
}
