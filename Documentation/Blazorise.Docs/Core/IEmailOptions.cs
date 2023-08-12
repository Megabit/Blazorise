namespace Blazorise.Docs.Core;

public interface IEmailOptions
{
    string SmtpServer { get; }
    int SmtpPort { get; }
    string SmtpUsername { get; set; }
    string SmtpPassword { get; set; }
    bool SmtpUseSSL { get; set; }

    string FromName { get; set; }
    string FromAddress { get; set; }
    string ReplyToAddress { get; set; }

    string ThemeReviewName { get; set; }
    string ThemeReviewAddress { get; set; }
}
