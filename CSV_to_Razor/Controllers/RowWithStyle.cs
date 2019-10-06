using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSV_to_Razor.Controllers
{
    public class RowWithStyle
    {
        public string Style { get; set; } = "parent";
        public List<string> row;

        public RowWithStyle(List<string> row)
        {
            this.row = row;
        }
    }
}
