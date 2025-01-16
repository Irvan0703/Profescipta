using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Profescipta.Models
{
    public class COM_CUSTOMER
    {
        [Key]
        public int COM_CUSTOMER_ID { get; set; }

        [StringLength(100)]
        public string? CUSTOMER_NAME { get; set; }

        // Navigasi ke SO_ORDER (One-to-Many)
        public ICollection<SO_ORDER>? SO_ORDERS { get; set; }
    }
}
