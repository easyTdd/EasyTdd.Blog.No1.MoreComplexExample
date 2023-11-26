namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

public class UnexpectedPayment
{
	public UnexpectedPayment(
		string invoiceNo, 
		string message)
	{
		InvoiceNo = invoiceNo;
		Message = message;
	}

	public string InvoiceNo { get; }
	public string Message { get; }

}