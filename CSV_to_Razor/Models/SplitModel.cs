using System.Collections.Generic;

namespace CSV_to_Razor.Controllers
{
    public class SplitModel
    {
        public List<string> header { get; set; }

        public SplitModel(List<string> header, List<RowWithStyle> table)
        {
            this.header = header;
            this.RequestId = table;
        }

        public List<RowWithStyle> RequestId { get; set; }

        //public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}