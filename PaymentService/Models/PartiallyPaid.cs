namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

public class PartiallyPaid
{
	public PartiallyPaid(string invoiceNo)
	{
		InvoiceNo = invoiceNo;
	}

	public string InvoiceNo { get; }

}