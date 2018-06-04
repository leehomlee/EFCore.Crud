using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EF.Crud.Models.Movies
{
    public class MovieActorInputModel
    {
        public int ActorId { get; set; }
        public string Role { get; set; }
    }
}
