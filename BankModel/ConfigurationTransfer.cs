using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace BankModel
{
	public class ConfigurationTransfer
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        [NotMapped]
        public string FormatMin { get { return FormatCurrency(MinValue); } }
        [NotMapped]
        public string FormatMax { get { return FormatCurrency(MaxValue); } }
        public string FormatCurrency(double money)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-Us");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            string format = String.Format(culture, "{0:C0}", money);
            return format;
        }
    }
}

