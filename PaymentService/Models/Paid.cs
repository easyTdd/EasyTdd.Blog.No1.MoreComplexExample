namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

public class Paid
{
	public Paid(string invoiceNo)
	{
		InvoiceNo = invoiceNo;
	}

	public string InvoiceNo { get; }
}