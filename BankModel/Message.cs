using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankModel
{
    
	public class Message
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMessage { get; set; }
        public int Sender { get; set; }
        public int Receiver { get; set; }
        public string? MessageChat { get; set; }
        public DateTime DateChat { get; set; } = DateTime.Now;
        [NotMapped]
        public string FormatDate { get { return DateChat.ToString("HH:mm"); } }
    }
}

