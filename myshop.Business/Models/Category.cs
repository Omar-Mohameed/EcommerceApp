using System.ComponentModel.DataAnnotations;

namespace myshop.Business.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(100),MinLength(5)]
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
}
