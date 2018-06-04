using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Crud.EntityFramework.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Summary { get; set; }
        public int DirectorId { get; set; }
    }
}
