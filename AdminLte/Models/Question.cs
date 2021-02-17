using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public enum Construct
    {
        CULTURE,
        PERFORMANCE,
        ENGAGEMENT
    }

    public class Section
    {
        [Key]
        public int ID { get; set; }
        public Assesment Assesment { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool IsRandom { get; set; }
        public Construct Construct { get; set; }
    }

    public enum QuestionType
    {
        SIMPLE_CHOICE,
        MATRIX
    }

    public enum MatrixSubType
    {
        SIMPLE,
        MULTIPLE,
        CUSTOM
    }

    public class Question
    {
        [Key]
        public int ID { get; set; }
        public Assesment Assesment { get; set; }
        public Section Section { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public bool IsRandom { get; set; }
        public QuestionType QuestionType { get; set; }
        public MatrixSubType MatrixSubType { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsRandomAnswer { get; set; }
        public Construct Construct { get; set; }
    }

    public enum ValueDriverDimention
    {
        AMANAH,
        KOMPETEN,
        HARMONIS,
        LOYAL,
        ADAPTIF,
        KOLABORATIF,
        REPUTASI_ORGANISASI,
        KEPEMIMPINAN,
        KARIR_DAN_PENGEMBANGAN_DIRI,
        PEKERJAAN,
        KEBUTUHAN_DASAR,
        LEARNING,
        GROWING,
        CONTRIBUTING
    }

    public class VerticalDimention
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public ValueDriverDimention ValueDriverDimention { get; set; }
        public Section Section { get; set; }
    }

    public class SubVerticalDimention
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public ValueDriverDimention ValueDriverDimention { get; set; }
        public Section Section { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
    }

    public enum SituationEvpDimention
    {
        URUTAN,
        NILAI,
        SAY,
        STAY_LEARNING,
        STAY_GROWING,
        STRIVE_CONTRIBUTING,
    }

    public class HorizontalDimention
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public SituationEvpDimention SituationEvpDimention { get; set; }
        public Section Section { get; set; }
    }

    public enum MatrixValueType
    {
        CHAR_BOX,
        FREE_TEXT,
        NUMERICAL_BOX
    }

    public class QuestionAnswer
    {
        [Key]
        public int ID { get; set; }
        public Question Question { get; set; }
        public Question MatrixQuestion { get; set; }
        public MatrixValueType MatrixValue { get; set; }
        public int Sequence { get; set; }
        public string Value { get; set; }
        public float Weight { get; set; }
        public float AnswerScore { get; set; }
        public bool IsCorrect { get; set; }
        public VerticalDimention VerticalDimention { get; set; }
        public SubVerticalDimention SubVerticalDimention { get; set; }
        public HorizontalDimention HorizontalDimention { get; set; }
        public Section Section { get; set; }
    }

    public class QuestionPackage
    {
        [Key]
        public int ID { get; set; }
        public Assesment Assesment { get; set; }
        public string Name { get; set; }
    }

    public class QuestionPackageLine
    {
        [Key]
        public int ID { get; set; }
        public QuestionPackage QuestionPackage { get; set; }
        public Question Question { get; set; }
    }

    public class QuestionPackagePeriod
    {
        [Key]
        public int ID { get; set; }
        public QuestionPackage QuestionPackage { get; set; }
        public Period Period { get; set; }
    }

}
