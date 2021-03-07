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

        public static Dictionary<string, string> getEntities(List<Entity> entities, int parent, int level)
        {
            string prefix = "";
            for (int i = 0; i < level; i++)
            {
                prefix += "-- ";
            }
            var result = new Dictionary<string, string>();
            if (parent == 0)
            {
                result = entities
                    .Where(x => x.ParentEntity == null)
                    .ToDictionary(x => x.ID.ToString(), y => prefix + " " + y.Name);
            }
            else
            {
                result = entities
                    .Where(x => x.ParentEntity != null && x.ParentEntity.ID == parent)
                    .ToDictionary(x => x.ID.ToString(), y => prefix + " " + y.Name);
            }

            var fixResult = new Dictionary<string, string>();
            foreach (string key in result.Keys)
            {
                fixResult.Add(key, result[key]);
                var subEntities = getEntities(entities, int.Parse(key), level + 1);
                if (subEntities.Count > 0)
                {
                    foreach (string subKey in subEntities.Keys)
                    {
                        if (!fixResult.ContainsKey(subKey))
                        {
                            fixResult.Add(subKey, subEntities[subKey]);
                        }
                    }
                }
            }

            return fixResult;
        }
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
