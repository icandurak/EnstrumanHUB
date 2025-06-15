using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnstrümanHub.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        // Navigation property for Orders
        public virtual ICollection<Order> Orders { get; set; }
    }
} 