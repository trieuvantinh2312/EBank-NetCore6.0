using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Pls enter full name")]
        [Display(Name ="Full name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Pls enter IdentityCard Number")]
        [Display(Name = "IdentityCard Number")]
        public string IdentityCardNumber { get; set; } = "";

        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; } = "Other";

        [Required(ErrorMessage ="Pls enter email.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Avatar { get; set; }

        [Required(ErrorMessage = "Pls enter phone.")]
        public string Phone { get; set; }

        public string? UserName { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string? Password { get; set; }
        //[Required(ErrorMessage = "Pls enter confirm password.")]
        public string Status { get; set; } = "normal";
        public int Attempt { get; set; } = 0;
        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<Contact> Contact { get; set; } = new List<Contact>();

        [NotMapped]

        public string DateFormat { get { return Dob.ToString("dd/MM/yyyy"); } }



    }
}
