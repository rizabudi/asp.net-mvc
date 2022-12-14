using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class Participant
    {
        [Key]
        public int ID { get; set; }
        public Schedule Schedule { get; set; }
        public QuestionPackage QuestionPackage { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public bool IsCanRetake { get; set; }
        public int MaxRetake { get; set; }
        [ForeignKey("ParticipantUser")]
        public string ParticipantUserUserId { get; set; }
        [ForeignKey("ParticipantUserUserId")]
        public virtual ParticipantUser? ParticipantUser { get; set; }
        public virtual ICollection<ParticipantAnswerSheet> ParticipantAnswerSheets { get; set; }
    }

    public enum ParticipantAnswerSheetEnum
    {
        NEW, INPROGRESS, FINISH
    }

    public class ParticipantAnswerSheet
    {
        [Key]
        public int ID { get; set; }
        public Participant Participant { get; set; }
        public ParticipantAnswerSheetEnum State { get; set; }
        public string Data { get; set; }
        public bool IsFinish { get; set; }
        public bool IsLast { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public virtual ICollection<ParticipantAnswerSheetSection> ParticipantAnswerSheetSections { get; set; }
        public virtual ICollection<ParticipantAnswerSheetLine> ParticipantAnswerSheetLines { get; set; }

    }

    public class ParticipantAnswerSheetSection
    {
        [Key]
        public int ID { get; set; }
        public ParticipantAnswerSheet ParticipantAnswerSheet { get; set; }
        public Section Section { get; set; }
        public bool IsFinish { get; set; }

    }

    public class ParticipantAnswerSheetLine
    {
        [Key]
        public int ID { get; set; }
        public ParticipantAnswerSheet ParticipantAnswerSheet { get; set; }
        public int SuggestedAnswerID { get; set; }
        public int MatrixRowAnswerID { get; set; }
        public int QuestionSquence { get; set; }
        public bool IsSkipped { get; set; }
        public MatrixValueType AnswerType { get; set; }
        public string CharBoxValue { get; set; }
        public string FreeTextValue { get; set; }
        public float NumericalBoxValue { get; set; }
        public float AnswerWeight { get; set; }
        public float AnswerScore { get; set; }
        public Question Question { get; set; }
        public DateTime? SubmitAt { get; set; }
    }

    public class ParticipantSectionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public ParticipantAnswerSheetLine ParticipantAnswerSheetLine { get; set; }
        public Section Section { get; set; }
    }

    public class ParticipantHorizontalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public ParticipantAnswerSheetLine ParticipantAnswerSheetLine { get; set; }
        public HorizontalDimention HorizontalDimention { get; set; }
    }

    public class ParticipantVerticalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public ParticipantAnswerSheetLine ParticipantAnswerSheetLine { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
    }

    public class ParticipantSubVerticalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public float ZScore { get; set; }
        public float TScore { get; set; }
        public ParticipantAnswerSheetLine ParticipantAnswerSheetLine { get; set; }
        public SubVerticalDimention SubVerticalDimention { get; set; }
    }

    public class EntitySectionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public Entity Entity { get; set; }
        public Schedule Schedule { get; set; }
        public Section Section { get; set; }
    }

    public class EntityHorizontalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public Entity Entity { get; set; }
        public Schedule Schedule { get; set; }
        public HorizontalDimention HorizontalDimention { get; set; }
    }

    public class EntityVerticalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public Entity Entity { get; set; }
        public Schedule Schedule { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
    }

    public class EntitySubVerticalDimentionScore
    {
        [Key]
        public int ID { get; set; }
        public float Score { get; set; }
        public float ZScore { get; set; }
        public float TScore { get; set; }
        public Entity Entity { get; set; }
        public Schedule Schedule { get; set; }
        public SubVerticalDimention SubVerticalDimention { get; set; }
    }
}
