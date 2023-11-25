using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		[HttpPost("callback")]
		public IActionResult Callback(PaymentCallbackRequest request)
		{
			if (!IsRequestValid(request))
			{
				return BadRequest();
			}

			throw new NotImplementedException();
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
