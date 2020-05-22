using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int Id { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(".{3,50}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(".{3,50}")]
        public string User_Name { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(".{8,50}")]
        public string Password { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
