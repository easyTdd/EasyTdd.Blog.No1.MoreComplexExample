namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;

public interface IInvoiceRepository
{
	Task RegisterPaymentAsync(
		string invoiceNo,
		decimal amount
	);
}