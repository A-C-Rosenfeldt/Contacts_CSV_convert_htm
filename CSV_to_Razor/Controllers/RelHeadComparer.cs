using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CSV_to_Razor.Controllers
{
    public class RelHeadComparer : IComparer<RelationHeader>
    {
        public int Compare([AllowNull] RelationHeader x, [AllowNull] RelationHeader y)
        {
            var r = string.Compare(x.lfdNo, y.lfdNo);
            if (r != 0)
            {
                r = string.Compare(x.colType, y.colType);
            }

            return r;
        }
    }
}
