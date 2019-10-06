using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSV_to_Razor.Controllers
{
    public class Header
    {
        
        public string lfdNo { get; set; }
        public string colType { get; set; }
    }
    public class RelationHeader : Header
    {
        public int index { get; set; }

    }
    public class RelationHeaderPre : Header
    {
        public string word { get; set; }

    }
}

