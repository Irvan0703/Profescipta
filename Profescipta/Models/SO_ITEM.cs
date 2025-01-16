using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Profescipta.Models
{
    public class SO_ITEM
    {
        [Key]
        public long SO_ITEM_ID { get; set; }

        [Required]
        public long SO_ORDER_ID { get; set; }

        [Required, StringLength(100)]
        public string ITEM_NAME { get; set; } = string.Empty;

        [Required]
        public int QUANTITY { get; set; } = -99;

        [Required]
        public double PRICE { get; set; } = 0.0;

        // Navigasi ke SO_ORDER (Many-to-One)
        [ForeignKey("SO_ORDER_ID")]
        public SO_ORDER? SO_ORDER { get; set; }
    }
}
