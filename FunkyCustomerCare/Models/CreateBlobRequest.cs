namespace FunkyCustomerCare.Models
{
    public class CreateBlobRequest
    {
        public string Container { get; }
        public string FileName { get; }
        public string Content { get; }

        public CreateBlobRequest(string container, string fileName, string content)
        {
            Container = container;
            FileName = fileName;
            Content = content;
        }
    }
}