using System.ComponentModel.DataAnnotations;
namespace FPTBook.Models;
public class Order
{
        public int Id { get; set; }       
        [DataType(DataType.Date)]
        public DateTime OrderTime { get; set; }
        public decimal Total { get; set; }
        public string State{ get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public ICollection<OrderItem> OrderItem { get; set; }
}
