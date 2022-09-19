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
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        public string? AccountNo { get; set; }
        public double Balance { get; set; }
        public string? PinCode { get; set; }
        public DateTime OpenDate { get; set; } = DateTime.Now.Date;
        public int Attempt { get; set; } = 0;
        public string? Status { get; set; } = "normal";
        [ForeignKey("CustomerId")]
        public int CustomerId { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Cheque> Cheque { get; } = new List<Cheque>();

        [NotMapped]
        public string FormatAmount { get { return FormatCurrency(Balance); } }
        public string FormatDate { get { return OpenDate.ToString("yyyy-MM-dd"); } }
        public string FormatCurrency(double money)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-Us");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            string format = String.Format(culture, "{0:C0}", money);
            return format;
        }
    }
}
