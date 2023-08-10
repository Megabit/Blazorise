using System.Collections.Generic;

namespace Blazorise.Docs.Domain
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            To = new List<EmailAddress>();
            From = new List<EmailAddress>();
            Bcc = new List<EmailAddress>();
            Cc = new List<EmailAddress>();
        }

        public EmailMessage( User user )
            : this()
        {
            To.Add( new EmailAddress( $"{user.FirstName} {user.LastName}", user.Email ) );
        }

        public EmailMessage( string toName, string toAddress )
            : this()
        {
            To.Add( new EmailAddress( toName, toAddress ) );
        }

        public EmailMessage( string toFirstName, string toLastName, string toAddress )
            : this( $"{toFirstName} {toLastName}", toAddress )
        {
        }

        public List<EmailAddress> To { get; set; }

        public List<EmailAddress> From { get; set; }

        public List<EmailAddress> Bcc { get; set; }

        public List<EmailAddress> Cc { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; } = true;

        public List<EmailMessageAttachment> Attachments { get; set; }
    }
}
