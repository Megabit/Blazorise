using Blazorise.Docs.Core;

namespace Blazorise.Docs.Server.Infrastructure;

public class EmailOptions : IEmailOptions
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public bool SmtpUseSSL { get; set; }

    public string FromName { get; set; }
    public string FromAddress { get; set; }
    public string ReplyToAddress { get; set; }

    public string ThemeReviewName { get; set; }
    public string ThemeReviewAddress { get; set; }
}
