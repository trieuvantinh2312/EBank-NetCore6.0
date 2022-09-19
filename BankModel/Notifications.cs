using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace BankModel
{
	public class Notifications
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
        public int FromCustomerId { get; set; }
        public int ToCustomerId { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public double Amount { get; set; }
        public DateTime DateNotification { get; set; } = DateTime.Now;
        public int TransactionId { get; set; }
        public Transaction Transactions { get; set; }
        [NotMapped]
        public string FormatAmount { get { return FormatCurrency(Amount); }  }
        [NotMapped]
        public string FormatDate { get { return DateNotification.ToString("yyyy-MM-dd HH:mm"); } }
        public string FormatCurrency(double money)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-Us");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            string format = String.Format(culture, "{0:C0}", money);
            return format;
        }
    }

}

