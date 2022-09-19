using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankModel
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewId { get; set; }
        public string Title { get; set; }
        public string? ImageMain { get; set; }
        public string? ImageSlide { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Status { get; set; }
        [NotMapped]
        public string FormatDate { get { return CreatedDate.ToString("yyyy-MM-dd"); } }
    }
}
