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
        NUMBER,
        HIDDEN,
        DROPDOWN,
        YESNO
    }

    public class FormModel
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public InputType InputType { get; set; }
        public Dictionary<string, string> Options { get; set; }
    }
}
