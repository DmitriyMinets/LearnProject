using System.ComponentModel.DataAnnotations;

namespace Rocky.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
