namespace Pro.Services.Email
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Options;
    using Models.Const;
    using Models.DB;
    using Models.Email;
    using Models.Enums;
    using MimeKit;
    using Helpers;

    public class MailKitNotificationService : IEmailNotificationService
    {
        private readonly SmtpClient _smtpClient;

        private readonly ApplicationSettings _applicationSettings;

        public MailKitNotificationService(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
            _smtpClient =  GetSmtpClient().GetAwaiter().GetResult();
        }

        private async Task<SmtpClient> GetSmtpClient()
        {
            var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com",587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_applicationSettings.Email, _applicationSettings.Password);
            return client;
        }
        private void FillCustomData(Email email, Dictionary<string, string> customData)
        {
            Parallel.ForEach(customData, item =>
            {
                email.Body = email.Body.Replace(item.Key, item.Value);
            });
        }

        private async Task<Email> GetEmailTemplate(EmailTemplate emailTemplate)
        {
            var email = new Email();

            switch (emailTemplate)
            {
                case EmailTemplate.RegisterEmailTemplate:

                    email.Body = await File.ReadAllTextAsync(Path.Join(Directory.GetCurrentDirectory(),
                        Definition.Resources,
                        Definition.EmailTemplates,
                        EmailTemplates.EmailRegisterTemplate));
                    email.Subject = EmailSubject.Register;
                    break;
                case EmailTemplate.RemindPassword:

                    email.Body = await File.ReadAllTextAsync(Path.Join(Directory.GetCurrentDirectory(),
                        Definition.Resources,
                        Definition.EmailTemplates,
                        EmailTemplates.EmailRemindTemplate));
                    email.Subject = EmailSubject.RemindPassword;
                    break;
            }

            return email;
        }

        private async void PrepareAndSendMail(Email email)
        {

            var mailMessage = new MimeMessage()
            {
                Body = new BodyBuilder()
                {
                    HtmlBody = email.Body
                }.ToMessageBody(),
                Subject = email.Subject
            };

            
            AddStandardEmailName(mailMessage);

            AddRecipient(mailMessage, email.Recipients);

            await _smtpClient.SendAsync(mailMessage);
            await _smtpClient.DisconnectAsync(true);
        }

        private void AddRecipient(MimeMessage mailMessage, List<Recipient> emailRecipients)
        {
            mailMessage.To.Add(MailboxAddress.Parse(_applicationSettings.TestEmail1));
            mailMessage.To.Add(MailboxAddress.Parse(_applicationSettings.TestEmail2));
        }

        private void AddStandardEmailName(MimeMessage mailMessage)
        {
            mailMessage.From.Add(new MailboxAddress(_applicationSettings.EmailName, _applicationSettings.Email));
        }

        private async Task<bool> CreateEmail(EmailTemplate emailTemplate, Dictionary<string, string> customData, List<Recipient> recipients)
        {
            try
            {
                var email = await GetEmailTemplate(emailTemplate);

                if (email is null) return false;

                // todo test
                email.Recipients = new List<Recipient> { new() { Email = _applicationSettings.MailToTest } };

                FillCustomData(email, customData);

                PrepareAndSendMail(email);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }


        public async Task<bool> SendRegisterEmail(User user)
        {
            var customData = new Dictionary<string, string>
            {
                {EmailParameters.Login, user.Login},
                {EmailParameters.Email, user.Email},
                {EmailParameters.ControlHash, user.ControlHash},
                {EmailParameters.UserHash, user.UserHash}
            };

            return await CreateEmail(EmailTemplate.RegisterEmailTemplate, customData, new List<Recipient>() { new() { Email = user.Email } });
        }

        public async Task<bool> SendRemindPasswordEmail(User user)
        {
            var customData = new Dictionary<string, string>
            {
                {EmailParameters.UserHash, user.UserHash},
                {EmailParameters.ControlHash, user.ControlHash},
                {EmailParameters.Login, user.Login},
                {EmailParameters.RandomString, String.Empty.RandomString()}
            };

            return await CreateEmail(EmailTemplate.RemindPassword, customData,
                new List<Recipient>() { new() { Email = user.Email } });
        }
    }
}
