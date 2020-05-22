using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PMS.Models
{    
    public partial class item
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public item()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int Id { get; set; }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(".{3,50}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*Required")]
        public double Price { get; set; }

        [Required(ErrorMessage = "*Required")]
        public System.DateTime Expiration_Date { get; set; }

        [Required(ErrorMessage = "*Required")]
        public int Quentity { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
        public string Image { get; set; }
        public Nullable<int> Discount { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
