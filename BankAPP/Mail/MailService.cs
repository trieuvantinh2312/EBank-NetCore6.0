using System;
using BankModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BankAPP.Mail
{
    public class MailService : IMailService
    {
        private readonly BankModel.Mail.MailSettings _mailSettings;
        public MailService(IOptions<BankModel.Mail.MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<string> SendEmailAsync(BankModel.Mail.MailRequest mailRequest)
        {
            try
            {

                string FilePath = Directory.GetCurrentDirectory() + "/wwwroot/Template/Replay_contact.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[username]", mailRequest.UserName)
                    .Replace("[body]", mailRequest.Body)
                    .Replace("[usercontent]", mailRequest.UserContent);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse("eBanking");
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = $"Dear, {mailRequest.UserName}";
                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return "success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> SendEmailAsyncTransaction(BankModel.Mail.MailRequest mailRequest,Transaction sac)
        {
            try
            {

                string FilePath = Directory.GetCurrentDirectory() + "/wwwroot/Template/Transaction_details.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[transactionCode]", sac.TransactionCode)
                    .Replace("[date]", sac.FormatDate)
                    .Replace("[fromAccount]", sac.FromAccount)
                    .Replace("[toAccount]", sac.ToAccount)
                    .Replace("[amount]", sac.Amount.ToString())
                    .Replace("[description]", sac.Description)
                    .Replace("[userName]", mailRequest.UserName);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse("eBanking");
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = $"Dear, {mailRequest.UserName}";
                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return "success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> SendEmailAsyncCustomer(BankModel.Mail.MailRequest mailRequest, Customer cus)
        {
            try
            {

                string FilePath = Directory.GetCurrentDirectory() + "/wwwroot/Template/CustomerInformation.html";
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                MailText = MailText.Replace("[cusName]", cus.CustomerName)
                    .Replace("[phone]", cus.Phone)
                    .Replace("[address]", cus.Address)
                    .Replace("[email]", cus.Email)
                    .Replace("[userName]", cus.UserName)
                    .Replace("[password]", cus.Password);
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse("eBanking");
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = $"Dear, {mailRequest.UserName}";
                var builder = new BodyBuilder();
                builder.HtmlBody = MailText;
                email.Body = builder.ToMessageBody();
                using var smtp = new SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return "success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

