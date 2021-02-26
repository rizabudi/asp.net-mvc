using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Models
{
    public class ColumnModel
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Style { get; set; }
        public string Value { get; set; }
    }

    public class RowModel
    {
        public int ID { get; set; }
        public string IDString { get; set; }
        public string[] Value { get; set; }
    }
}
