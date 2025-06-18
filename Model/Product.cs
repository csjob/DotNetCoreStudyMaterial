using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreWebAPI.Model
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
