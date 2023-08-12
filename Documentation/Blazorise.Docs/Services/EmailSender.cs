#region Using directives
using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Docs.Core;
using Blazorise.Docs.Domain;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
#endregion

namespace Blazorise.Docs.Services;

public class EmailSender
{
    #region Members

    private readonly IEmailOptions emailOptions;

    private readonly ILogger<EmailSender> logger;

    #endregion

    #region Constructors

    public EmailSender( IEmailOptions emailOptions,
        ILogger<EmailSender> logger )
    {
        this.emailOptions = emailOptions;
        this.logger = logger;
    }

    #endregion

    #region Methods

    public async Task<Result> Send( EmailMessage emailMessage, CancellationToken cancellationToken )
    {
        try
        {
            var mimeMessage = new MimeMessage();

            // Set From Address it was not set
            if ( emailMessage.From.Count == 0 )
            {
                emailMessage.From.Add( new EmailAddress( emailOptions.FromName, emailOptions.FromAddress ) );
            }

            mimeMessage.To.AddRange( emailMessage.To.Select( x => new MailboxAddress( x.Name, x.Address ) ) );
            mimeMessage.From.AddRange( emailMessage.From.Select( x => new MailboxAddress( x.Name, x.Address ) ) );
            mimeMessage.Cc.AddRange( emailMessage.Cc.Select( x => new MailboxAddress( x.Name, x.Address ) ) );
            mimeMessage.Bcc.AddRange( emailMessage.Bcc.Select( x => new MailboxAddress( x.Name, x.Address ) ) );

            mimeMessage.Subject = emailMessage.Subject;

            var body = emailMessage.IsHtml ?
                new BodyBuilder { HtmlBody = emailMessage.Body }.ToMessageBody() :
                new TextPart( "plain" ) { Text = emailMessage.Body };

            using ( var disposableContainer = new DisposableContainer() )
            {
                if ( emailMessage.Attachments != null )
                {
                    // when sending attachments we need to build email as an multipart with a content-disposition
                    var multipart = new Multipart( "mixed" );

                    multipart.Add( body );

                    foreach ( var attachment in emailMessage.Attachments )
                    {
                        var memoryStream = new MemoryStream( attachment.Data );

                        // we cannot dispose attachment just yet so put on hold for later dispose
                        disposableContainer.Add( memoryStream );

                        var mimeAttachment = new MimePart()
                        {
                            Content = new MimeContent( memoryStream, ContentEncoding.Default ),
                            ContentDisposition = new ContentDisposition( ContentDisposition.Attachment ),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = attachment.FileName,
                        };

                        multipart.Add( mimeAttachment );
                    }

                    // now set the multipart/ mixed as the message body
                    mimeMessage.Body = multipart;
                }
                else
                {
                    mimeMessage.Body = body;
                }

                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using ( var emailClient = new SmtpClient() )
                {
                    if ( !emailOptions.SmtpUseSSL )
                    {
                        emailClient.ServerCertificateValidationCallback = ( object sender2, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors ) => true;
                    }

                    await emailClient.ConnectAsync( emailOptions.SmtpServer, emailOptions.SmtpPort, emailOptions.SmtpUseSSL ).ConfigureAwait( false );

                    //Remove any OAuth functionality as we won't be using it.
                    emailClient.AuthenticationMechanisms.Remove( "XOAUTH2" );

                    if ( !string.IsNullOrWhiteSpace( emailOptions.SmtpUsername ) )
                    {
                        await emailClient.AuthenticateAsync( emailOptions.SmtpUsername, emailOptions.SmtpPassword ).ConfigureAwait( false );
                    }

                    await emailClient.SendAsync( mimeMessage ).ConfigureAwait( false );

                    await emailClient.DisconnectAsync( true ).ConfigureAwait( false );
                }
            }

            return new Result( ResultType.Ok );
        }
        catch ( Exception exc )
        {
            var errorMessage = "Failed to send email.";
            logger.LogCritical( exc, errorMessage );
            return new Result<string>( ErrorType.NetworkError, errorMessage, exc );
        }
    }

    #endregion
}
