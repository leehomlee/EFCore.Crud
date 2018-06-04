using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Crud.EntityFramework.Entities
{
    public class MovieActor
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public string Role { get; set; }
    }
}
