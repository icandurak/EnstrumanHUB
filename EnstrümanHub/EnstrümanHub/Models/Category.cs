using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EnstrümanHub.Models
{
    public class Category
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
} 