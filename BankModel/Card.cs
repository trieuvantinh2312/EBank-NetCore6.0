using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankModel
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CardId { get; set; }
        public string CardName { get; set; } = "Visa";

        public string? CardNo { get; set; }
        public DateTime OpenCard { get; set; } = DateTime.Now.Date;
        public string ExpirationCard { get; set; }
        public string Cvv { get; set; }
        [ForeignKey("AccountId")]
        public int AccountId { get; set; }
        [NotMapped]
        public string cardFormat { get { 
                if (!string.IsNullOrEmpty(CardNo))
                    return  String.Format("{0:0000 0000 0000 0000}", (Int64.Parse(CardNo)));
                return CardNo;
            } }

    }
}   