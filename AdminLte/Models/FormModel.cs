using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public enum InputType
    {
        TEXT,
        TEXTAREA,
        EMAIL,
        PASSWORD,
        WYSIWYG,
        NUMBER,
        HIDDEN,
        DROPDOWN,
        YESNO,
        IMAGE,
    }

    public enum FormPosition
    {
        LEFT,
        RIGHT,
        FULL
    }

    public class FormModel
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public InputType InputType { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public FormPosition FormPosition { get; set; } = FormPosition.LEFT;
        public bool IsRequired { get; set; }
        public bool IsDisable { get; set; }
    }
}
