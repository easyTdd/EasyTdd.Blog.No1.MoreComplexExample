using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IBus _bus;

		public PaymentController(IInvoiceRepository invoiceRepository, IBus bus)
		{
			_invoiceRepository = invoiceRepository;
			_bus = bus;
		}

		[HttpPost("callback")]
		public async Task<IActionResult> Callback(PaymentCallbackRequest request)
		{
			if (!IsRequestValid(request))
			{
				return BadRequest();
			}

			var invoice = await _invoiceRepository
				.GetByInvoiceNoAsync(request.InvoiceNo);

			if (invoice == null)
			{
				await _bus
					.PublishAsync(
						new UnexpectedPayment(
							request.InvoiceNo,
							"Payment received for unknown invoice."
						)
					);

				return null;
			}

			if (invoice.TotalAmount < invoice.TotalAmount + request.AmountPaid)
			{
				await _bus
					.PublishAsync(
						new UnexpectedPayment(
							request.InvoiceNo,
							"Invoice is overpaid."
						)
					);
			}

			await _invoiceRepository
				.RegisterPaymentAsync(
					request.InvoiceNo,
					request.AmountPaid
				);

			if (invoice.TotalAmount > request.AmountPaid)
			{
				await _bus
					.PublishAsync(
						new PartiallyPaid(request.InvoiceNo)
					);
			}
			else
			{
				await _bus
					.PublishAsync(
						new Paid(request.InvoiceNo)
					);
			}

			return Ok();
		}

		private static bool IsRequestValid(PaymentCallbackRequest request)
		{
			if (request.AmountPaid < 0)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(request.InvoiceNo))
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(request.PaymentReference))
			{
				return false;
			}

			return true;
		}
	}
}
