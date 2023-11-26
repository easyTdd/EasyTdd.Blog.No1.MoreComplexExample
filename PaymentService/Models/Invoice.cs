namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

public class Invoice
{
	public Invoice(
		string invoiceNo,
		decimal totalAmount,
		decimal paidAmount)
	{
		InvoiceNo = invoiceNo;
		TotalAmount = totalAmount;
		PaidAmount = paidAmount;
	}

	public string InvoiceNo { get; }
	public decimal TotalAmount { get; }
	public decimal PaidAmount { get; }
}