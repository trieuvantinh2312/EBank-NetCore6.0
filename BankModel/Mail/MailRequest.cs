using System;
namespace BankModel.Mail
{
	public class MailRequest
	{
        public string? UserName { get; set; }
        public string? UserContent { get; set; }
        public string? ToEmail { get; set; }
        public string? Body { get; set; }
    }
}

