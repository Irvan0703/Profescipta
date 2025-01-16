using Profescipta.Models;

namespace Profescipta.ViewModel
{
    public class OrderViewModel
    {
        public SO_ORDER SO_ORDER { get; set; }
        public List<SO_ITEM> SO_ITEM { get; set; } = new List<SO_ITEM>();
    }
}
