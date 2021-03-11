using System.ComponentModel;

namespace Pro.Services.Email
{
    using System;
    using System.Collections.Generic;
    using Pro.Models.Email;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using Models.Const;
    using Models.Enums;
    using Models.DB;
    using System.Diagnostics;

    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly SmtpClient _smtpClient;
        private readonly ApplicationSettings _applicationSettings;

        public EmailNotificationService(IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
            _smtpClient = GetSmtpClient();
        }

        private async Task<bool> CreateEmail(EmailTemplate emailTemplate, Dictionary<string, string> customData, List<Recipient> recipients)
        {
            try
            {
                var email = await GetEmailTemplate(emailTemplate);
                
                if (email is null) return false;

                //TODO
                // email.Recipients = recipients;

                email.Recipients = new List<Recipient>() {new() {Email = _applicationSettings.MailToTest}};

                FillCustomData(email, customData);

                SendMail(email);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        private void FillCustomData(Email email, Dictionary<string, string> customData)
        {
            Parallel.ForEach(customData, (item) =>
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

        private void SendMail(Email email)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_applicationSettings.Email),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            if(email.Recipients.Count == 1)
                mailMessage.To.Add(email.Recipients.First().Email);
            else
            {
                foreach (var item in email.Recipients)
                {
                    mailMessage.To.Add(item.Email);
                }
            }

            _smtpClient.SendCompleted += emailCallback;
            _smtpClient.SendAsync(mailMessage, email.Recipients);
        }

        private void emailCallback(object sender, AsyncCompletedEventArgs e)
        {
            var z = e;
        }

        private SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient("smtp.gmail.com");

            //client.UseDefaultCredentials = true;
            client.Port = 587;
            client.Credentials = new NetworkCredential(_applicationSettings.Email, _applicationSettings.Password);
            client.EnableSsl = true;

            return client;
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
                {EmailParameters.Login, user.Login}
            };

            return await CreateEmail(EmailTemplate.RemindPassword, customData,
                new List<Recipient>() {new() {Email = user.Email}});
        }
    }
}
