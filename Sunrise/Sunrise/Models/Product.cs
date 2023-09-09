using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sunrise.Models
{
    public class Product
    {
        /// <summary>
        /// a unique auto-generated identifier
        /// </summary>
        public int Id { get; set; }


        [Required(ErrorMessage ="You have to Provide a valid name.")]
        [MinLength(3, ErrorMessage ="Name cannot be less than 3 characters")]
        [MaxLength(30 , ErrorMessage = "Name cannot be more than 30 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "You have to Provide a valid Description.")]
        [MinLength(5, ErrorMessage = "Name cannot be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Name cannot be more than 50 characters")]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public DateTime ProductDate { get; set;}

        [ValidateNever]
        public DateTime CreatedAt { get; set; }

        [ValidateNever]
        public DateTime LastUpdatedAt { get; set; }

        public int CategoryId { get; set; }



        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string ImagePath { get; set; }

        [ValidateNever]
        [NotMapped]
        public IFormFile Image { get; set; }
    }
}
