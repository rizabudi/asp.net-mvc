using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Models
{
    public class VwCulturePerRow
    {
        [Key]
        public string id { get; set; }
        public int QuestionID { get; set; }
        public int MatrixRowAnswerID { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public double urutan { get; set; }
        public double bobot { get; set; }
        public double nilai { get; set; }
        public double bobotxnilai { get; set; }
    }

    public class VwCulturePerSubVerticalDimention
    {
        [Key]
        public string id { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public SubVerticalDimention SubVerticalDimention { get; set; }
        public double skorsituasi { get; set; }
        public double indexsituasi { get; set; }
    }

    public class VwCulturePerVerticalDimention
    {
        [Key]
        public string id { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
        public double indexvaluesubject { get; set; }
    }

    public class VwCulturePerSheet
    {
        [Key]
        public int ParticipantID { get; set; }
        public double indexakhlaksubjek { get; set; }
    }

    public class VwPerformancePerRow
    {
        [Key]
        public string id { get; set; }
        public int QuestionID { get; set; }
        public int MatrixRowAnswerID { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public float nilai { get; set; }
    }

    public class VwPerformancePerVerticalDimention
    {
        [Key]
        public string id { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
        public double indexvaluesubject { get; set; }
    }

    public class VwEngagementPerRow
    {
        [Key]
        public string id { get; set; }
        public int QuestionID { get; set; }
        public int MatrixRowAnswerID { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int VerticalDimentionID { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public int HorizontalDimentionID { get; set; }
        public float nilai { get; set; }
    }

    public class VwEngagementPerHorizontalDimention
    {
        [Key]
        public string id { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int HorizontalDimentionID { get; set; }
        public HorizontalDimention HorizontalDimention { get; set; }
        public double indexsituasi { get; set; }
    }

    public class VwEngagementPerSubVerticalDimention
    {
        [Key]
        public string id { get; set; }
        public int ParticipantID { get; set; }
        public Participant Participant { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public SubVerticalDimention SubVerticalDimention { get; set; }
        public double indexsituasi { get; set; }
    }

    public class VwParticipant
    {
        [Key]
        public int ParticipantID { get; set; }
        public int EntityID { get; set; }
        public Entity Entity { get; set; }
        public int? SubEntityID { get; set; }
        public Entity SubEntity { get; set; }
        public int AssesmentID { get; set; }
        public int QuestionPackageID { get; set; }
        public string UserId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
    }
}
