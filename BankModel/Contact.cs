using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        public string UserContact { get; set; }
        public string UserPhone { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "waiting";
        public DateTime DateContact { get; set; } = DateTime.Now.Date;
        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customers { get; set; }

        public string FormatDate  {
            get {
                return DateContact.ToShortDateString();
                }
            }

    }
}