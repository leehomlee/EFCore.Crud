using System;
using System.Collections.Generic;
using System.Text;

namespace EF.Crud.EntityFramework.Entities
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        //[Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
