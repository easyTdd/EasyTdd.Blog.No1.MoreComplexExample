using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;

public interface IInvoiceRepository
{
	Task RegisterPaymentAsync(
		string invoiceNo,
		decimal amount
	);

	Task<Invoice> GetByInvoiceNoAsync(string invoiceNo);
}