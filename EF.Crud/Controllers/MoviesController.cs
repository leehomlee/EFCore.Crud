﻿using EF.Crud.Models.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Crud.Controllers
{
    [Route("movies")]
    public class MoviesController : Controller
    {
        private readonly EntityFramework.Database context;
        public MoviesController(EntityFramework.Database db)
        {
            context = db;
        }

        [HttpGet]
        public IActionResult GetList()
        {
            var entities = from movie in context.Movies
                           join director in context.Directors
                           on movie.DirectorId equals director.Id
                           select new
                           {
                               movie.Id,
                               movie.Title,
                               movie.ReleaseYear,
                               movie.Summary,
                               Director = director.Name,
                               Actors = (
                                    from actor in context.Actors
                                    join movieActor in context.MovieActors
                                    on actor.Id equals movieActor.ActorId
                                    where movieActor.MovieId == movie.Id
                                    select actor.Name + " as " + movieActor.Role
                               )
                           };
            var outputModel = entities.Select(entity => new
            {
                entity.Id,
                entity.Title,
                entity.ReleaseYear,
                entity.Summary,
                entity.Director,
                entity.Actors
            });
            return Ok(outputModel);
        }

        [HttpGet("{id}", Name = "GetMovie")]
        public IActionResult GetItem(int id)
        {
            var entity = (from movie in this.context.Movies
                          join director in this.context.Directors
                             on movie.DirectorId equals director.Id
                          where movie.Id == id
                          select new
                          {
                              movie.Id,
                              movie.Title,
                              movie.ReleaseYear,
                              movie.Summary,
                              Director = director.Name,
                              Actors = (
                                             from actor in this.context.Actors
                                             join movieActor in this.context.MovieActors
                                                 on actor.Id equals movieActor.ActorId
                                             where movieActor.MovieId == movie.Id
                                             select actor.Name
                                         )
                          }).FirstOrDefault();

            if (entity == null)
                return NotFound();

            var outputModel = new
            {
                entity.Id,
                entity.Title,
                entity.ReleaseYear,
                entity.Summary,
                entity.Director,
                entity.Actors
            };

            return Ok(outputModel);
        }

        [HttpPost]
        public IActionResult Create([FromBody]MovieCreateInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest();

            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var entity = new EntityFramework.Entities.Movie
                    {
                        Title = inputModel.Title,
                        ReleaseYear = inputModel.ReleaseYear,
                        Summary = inputModel.Summary,
                        DirectorId = inputModel.DirectorId
                    };
                    this.context.Movies.Add(entity);
                    this.context.SaveChanges();

                    foreach (var actor in inputModel.Actors)
                    {
                        this.context.MovieActors.Add(new EntityFramework.Entities.MovieActor
                        {
                            MovieId = entity.Id,
                            ActorId = actor.ActorId,
                            Role = actor.Role
                        });
                    }
                    this.context.SaveChanges();

                    transaction.Commit();

                    var outputModel = new
                    {
                        entity.Id,
                        entity.Title,
                        entity.ReleaseYear,
                        entity.Summary,
                        entity.DirectorId
                    };

                    return CreatedAtRoute("GetMovie", new { id = outputModel.Id }, outputModel);
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]MovieUpdateInputModel inputModel)
        {
            if (inputModel == null || id != inputModel.Id)
                return BadRequest();

            var entity = new EntityFramework.Entities.Movie
            {
                Id = inputModel.Id,
                Title = inputModel.Title,
                ReleaseYear = inputModel.ReleaseYear,
                Summary = inputModel.Summary,
                DirectorId = inputModel.DirectorId
            };

            this.context.Movies.Update(entity);
            this.context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{movieId}/actors")]
        public IActionResult AddMovieActor(int movieId, [FromBody]MovieActorInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest();

            this.context.MovieActors.Add(new EntityFramework.Entities.MovieActor
            {
                MovieId = movieId,
                ActorId = inputModel.ActorId,
                Role = inputModel.Role
            });

            this.context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = context.Movies
                                .Where(e => e.Id == id)
                                .FirstOrDefault();

            if (entity == null)
                return NotFound();

            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    this.context.Movies.Remove(entity);

                    this.context.MovieActors.RemoveRange(
                        this.context.MovieActors.Where(e => e.MovieId == id).ToList());

                    this.context.SaveChanges();

                    transaction.Commit();
                }
                catch (System.Exception ex)
                {
                    transaction.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            return NoContent();
        }
    }
}
