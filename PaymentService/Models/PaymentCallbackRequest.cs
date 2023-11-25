namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

public class PaymentCallbackRequest
{
	public string PaymentReference { get; set; }
	public string InvoiceNo { get; set; }
	public decimal AmountPaid { get; set; }
}