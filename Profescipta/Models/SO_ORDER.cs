using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Profescipta.Models
{
    public class SO_ORDER
    {
        [Key]
        public long SO_ORDER_ID { get; set; }

        [Required, StringLength(20)]
        public string ORDER_NO { get; set; } = string.Empty;

        [Required]
        public DateTime ORDER_DATE { get; set; } = new DateTime(1900, 1, 1);

        [Required]
        public int COM_CUSTOMER_ID { get; set; }

        [Required, StringLength(100)]
        public string ADDRESS { get; set; } = string.Empty;

        // Navigasi ke COM_CUSTOMER (Many-to-One)
        [ForeignKey("COM_CUSTOMER_ID")]
        public COM_CUSTOMER? COM_CUSTOMER { get; set; }

        // Navigasi ke SO_ITEM (One-to-Many)
        public ICollection<SO_ITEM>? SO_ITEM { get; set; }
    }
}
