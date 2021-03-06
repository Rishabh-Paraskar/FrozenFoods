//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FrozenFoods.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Order : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }


        [Required]
        [StringLength(20, ErrorMessage = "The characters must be less than 20 characters.")]
        [RegularExpression(@"^[a-zA-Z]*", ErrorMessage = "Only Alphabets are allowed.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The characters must be less than 20 characters.")]
        [RegularExpression(@"^[a-zA-Z]*", ErrorMessage = "Only Alphabets are allowed.")]
        public string SurName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The characters must be less than 100 characters.")]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^([0-9]{6})", ErrorMessage = "Invalid Zip Code.")]
        public string ZipCode { get; set; }
        public int OrderNumber { get; set; }
        public string OrderStatus { get; set; }
        public bool AcceptOrReject { get; set; }
        public bool flag { get; set; }
        public string CustomerId { get; set; }
        public string Remark { get; set; }
       
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
