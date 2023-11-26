using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		enum CaseName
		{
			Paid,
			PartiallyPaid,
			UnexpectedPayment
		};

		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IBus _bus;

		private readonly Dictionary<
			CaseName,
			Func<Invoice?, PaymentCallbackRequest, Task>
		> _caseHandlers;

		public PaymentController(IInvoiceRepository invoiceRepository, IBus bus)
		{
			_invoiceRepository = invoiceRepository;
			_bus = bus;

			_caseHandlers = new Dictionary<CaseName, Func<Invoice?, PaymentCallbackRequest, Task>>
			{
				[CaseName.Paid] = HandlePaid,
				[CaseName.PartiallyPaid] = HandlePartiallyPaid,
				[CaseName.UnexpectedPayment] = HandlerUnexpectedPayment
			};
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

			var caseName = ResolveCase(invoice, request);

			await _caseHandlers[caseName](invoice, request);

			return Ok();
		}

		private async Task HandlePaid(
			Invoice? invoice,
			PaymentCallbackRequest request)
		{
			await HandleExpectedPayment(
				request,
				new Paid(request.InvoiceNo)
			);
		}

		private async Task HandlePartiallyPaid(
			Invoice? invoice,
			PaymentCallbackRequest request)
		{
			await HandleExpectedPayment(
				request,
				new PartiallyPaid(request.InvoiceNo)
			);
		}

		private async Task HandleExpectedPayment<TMessage>(
			PaymentCallbackRequest request,
			TMessage message)
		{
			await _invoiceRepository
				.RegisterPaymentAsync(
					request.InvoiceNo,
					request.AmountPaid
				);

			await _bus
				.PublishAsync(message);
		}

		private async Task HandlerUnexpectedPayment(
			Invoice? invoice,
			PaymentCallbackRequest request)
		{
			await _bus
				.PublishAsync(
					new UnexpectedPayment(
						request.InvoiceNo,
						invoice == null
							? "Payment received for unknown invoice."
							: "Invoice is overpaid."
					)
				);
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

		private CaseName ResolveCase(
			Invoice? invoice,
			PaymentCallbackRequest request)
		{
			if (invoice == null)
			{
				return CaseName.UnexpectedPayment;
			}

			if (invoice.TotalAmount < invoice.PaidAmount + request.AmountPaid)
			{
				return CaseName.UnexpectedPayment;
			}

			if (invoice.TotalAmount > request.AmountPaid)
			{
				return CaseName.PartiallyPaid;
			}

			return CaseName.Paid;
		}
	}
}
