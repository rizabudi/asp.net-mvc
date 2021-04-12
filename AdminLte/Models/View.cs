﻿using System;
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
        public int VerticalDimentionID { get; set; }
        public int SubVerticalDimentionID { get; set; }
        public float urutan { get; set; }
        public float bobot { get; set; }
        public float nilai { get; set; }
        public float bobotxnilai { get; set; }
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
}
