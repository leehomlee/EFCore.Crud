using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Crud.Models.Movies
{
    public class MovieCreateInputModel
    {
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Summary { get; set; }
        public int DirectorId { get; set; }
        public List<MovieActorInputModel> Actors { get; set; }
    }
}
