using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class Assesment
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Section> Sections { get; set; }
    }

    public class Period
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public virtual ICollection<SubPeriod> SubPeriods { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<QuestionPackagePeriod> QuestionPackagePeriods { get; set; }
    }

    public class SubPeriod
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Period Period { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }

    public class Schedule
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public Period Period { get; set; }
        public SubPeriod SubPeriod { get; set; }
        public Entity Entity { get; set; }
        public Assesment Assesment { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public virtual ICollection<Participant> Participants { get; set; }
    }
}
