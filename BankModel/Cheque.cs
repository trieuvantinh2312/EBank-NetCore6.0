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
    public class Cheque
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double Amount { get; set; } = 0;
        public int CustomerId { get; set; }
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; } = "waiting";
        //public Account? Account { get; set; }
        [NotMapped]
        public string? FormatAmount { get { return FormatCurrency(Amount); } }
        [NotMapped]
        public string? FormatDate { get { return CreatedDate.ToString("yyyy-MM-dd"); } }
        public string FormatCurrency(double money)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-Us");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            string format = String.Format(culture, "{0:C0}", money);
            return format;
        }
    }
}
