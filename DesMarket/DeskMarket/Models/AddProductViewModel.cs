using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Common.ValidationConstants;

namespace DeskMarket.Models
{
    public class AddProductViewModel
    {
        [Required]
        [MinLength(ProductMinNameLength)]
        [MaxLength(ProductMaxNameLength)]
        public string ProductName { get; set; } = null!;

        [Required]
        [MinLength(ProductDescriptionMinLength)]
        [MaxLength(ProductDescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(1.00, 3000.00)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Please select a category.")]

        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();


        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedOn { get; set; }
       
    }
}
