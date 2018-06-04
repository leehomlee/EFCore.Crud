using EF.Crud.Models.Actors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Crud.Controllers
{
    [Route("actors")]
    public class ActorsController : Controller
    {
        private readonly EntityFramework.Database context;
        public ActorsController(EntityFramework.Database db)
        {
            context = db;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var entities = await context.Actors.ToListAsync();
            var outputModel = entities.Select(entity => new
            {
                entity.Id,
                entity.Name,
                entity.Timestamp
            });
            return Ok(outputModel);
        }

        [HttpGet("{id}", Name = "GetActor")]
        public IActionResult GetItem(int id)
        {
            var entity = context.Actors
                .Where(e => e.Id == id)
                .FirstOrDefault();
            if (entity == null)
                return NotFound();
            var outputModel = new
            {
                entity.Id,
                entity.Name,
                entity.Timestamp
            };
            return Ok(outputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]ActorCreateInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest();

            var entity = new EntityFramework.Entities.Actor
            {
                Name = inputModel.Name
            };

            context.Actors.Add(entity);
            await context.SaveChangesAsync();

            var outputModel = new
            {
                entity.Id,
                entity.Name
            };

            return CreatedAtRoute("GetActor", new { id = outputModel.Id }, outputModel);
        }

        [HttpPut]
        public IActionResult Update(int id, [FromBody]ActorUpdateInputModel inputModel)
        {
            if (inputModel == null || id != inputModel.Id)
                return BadRequest();

            var entity = new EntityFramework.Entities.Actor
            {
                Id = inputModel.Id,
                Name = inputModel.Name,
                Timestamp = inputModel.Timestamp
            };

            try
            {
                context.Actors.Update(entity);
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var inEntry = ex.Entries.Single();
                var dbEntry = inEntry.GetDatabaseValues();

                if (dbEntry == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Actor was deleted by another user");

                var inModel = inEntry.Entity as EntityFramework.Entities.Actor;
                var dbModel = dbEntry.ToObject() as EntityFramework.Entities.Actor;

                var conflicts = new Dictionary<string, string>();

                if (inModel.Name != dbModel.Name)
                    conflicts.Add("Actor", $"Changed from '{inModel.Name}' to '{dbModel.Name}'");

                if (inModel.Timestamp != dbModel.Timestamp)
                    conflicts.Add("Timestamp", $"Changed from '{Convert.ToBase64String(inModel.Timestamp)}' to '{Convert.ToBase64String(dbModel.Timestamp)}'");

                return StatusCode(StatusCodes.Status412PreconditionFailed, conflicts);
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = context.Actors
                .Where(e => e.Id == id)
                .FirstOrDefault();

            if (entity == null)
                return NotFound();

            context.Actors.Remove(entity);
            context.SaveChanges();

            return NoContent();
        }
    }
}
