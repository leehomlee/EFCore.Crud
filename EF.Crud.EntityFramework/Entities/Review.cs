using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Crud.EntityFramework.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime Dated { get; set; }
        public string Summary { get; set; }
        public int MovieId { get; set; }
    }
}
