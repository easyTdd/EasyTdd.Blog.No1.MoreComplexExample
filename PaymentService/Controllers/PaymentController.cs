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

		public PaymentController(IInvoiceRepository invoiceRepository)
		{
			_invoiceRepository = invoiceRepository;
		}

		[HttpPost("callback")]
		public IActionResult Callback(PaymentCallbackRequest request)
		{
			if (!IsRequestValid(request))
			{
				return BadRequest();
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
