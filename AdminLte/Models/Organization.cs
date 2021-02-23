using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class Entity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public int Level { get; set; }

        [InverseProperty("SubEntities")]
        public Entity ParentEntity { get; set; }
        public virtual ICollection<Entity> SubEntities { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }

    public class Divition
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class Position
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class Department
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }

    public class CompanyFunction
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
