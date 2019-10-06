namespace CSV_to_Razor.Controllers
{
    public class MainStreamModel
    {
        //private string csv;

        public MainStreamModel(string csv)
        {
            this.RequestId = csv;
        }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}