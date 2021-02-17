using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Entity ParentEntity { get; set; }
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
