using Microsoft.AspNetCore.Html;

namespace TrojWebApp.Models
{
    public class InvoiceLink
    {
        public IHtmlContent Link { get; set; }
        public double Sum { get; set; }
    }
}
