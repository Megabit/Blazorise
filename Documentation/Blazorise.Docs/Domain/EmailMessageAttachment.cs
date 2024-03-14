namespace Blazorise.Docs.Domain
{
    public class EmailMessageAttachment
    {
        public EmailMessageAttachment( string fileName, byte[] data )
        {
            FileName = fileName;
            Data = data;
        }

        public string FileName { get; set; }

        public byte[] Data { get; set; }
    }
}
