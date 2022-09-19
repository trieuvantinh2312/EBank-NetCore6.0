using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public string TransactionCode { get; set; } = Guid.NewGuid().ToString();
        public DateTime TransactionDate { get; set; } = DateTime.Now.Date;
        public string Description { get; set; }
        public double Amount { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        [NotMapped]
        public string FormatAmount { get { return FormatCurrency(Amount); } }
        [NotMapped]
        public string FormatDate { get { return TransactionDate.ToString("yyyy-MM-dd HH:mm"); } }
        [NotMapped]
        public string FormatPdfDate { get { return TransactionDate.ToString("dd-MM-yyyy"); } }

        public string FormatCurrency(double money)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-Us");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            string format = String.Format(culture, "{0:C0}", money);
            return format;
        }
    }
}
