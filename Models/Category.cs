using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Models
{
    public class Category
    {

        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(150, ErrorMessage = "Category name cannot exceed 150 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        [JsonIgnore]
        public ICollection<Test> Tests { get; set; } = new List<Test>();

    }
}
