using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Company
    {
        [Column("CompanyId")]
        public Guid Id { get; set; }

        [Required(ErrorMessage ="Company is a required filed")]
        [MaxLength(60,ErrorMessage ="maximum of 60 characters are allowed")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company Address is a required filed")]
        [MaxLength(60, ErrorMessage = "maximum of 60 characters are allowed")]
        public string Address { get; set; }
        public string Country { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }
}
