using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Spiciness { get; set; }

        public enum ESpicy { 
            [Display(Name="Not Set")] 
            NA=0, 
            [Display(Name="Not Spicy")] 
            NotSpicy=1, 
            Spicy=2, 
            [Display(Name="Very Spicy")]
            VerySpicy=3 }

        public string Image { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] 
        public virtual  Category Category { get; set; }

        [Display(Name = "Subcategory")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage="Price should be greater than ${1}.")]
        public double Price { get; set; }


    }

   
}
