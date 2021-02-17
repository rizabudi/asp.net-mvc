using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class Participant
    {
        [Key]
        public int ID { get; set; }
        public Schedule Schedule { get; set; }
        public ParticipantUser ParticipantUser { get; set; }
        public QuestionPackage QuestionPackage { get; set; }
        public DateTime FinishedAt { get; set; }
        public Entity Entity { get; set; }
        public Position Position { get; set; }
        public CompanyFunction CompanyFunction { get; set; }
        public Divition Divition { get; set; }
        public Department Department { get; set; }
        public bool IsCanRetake { get; set; }
        public int MaxRetake { get; set; }
    }

    public class ParticipantAnswerSheet
    {
        public Participant Participant { get; set; }
        public ParticipantUser ParticipantUser { get; set; }

    }
}
