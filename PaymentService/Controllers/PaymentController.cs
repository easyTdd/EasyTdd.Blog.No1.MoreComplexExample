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
			if (request.AmountPaid < 0)
			{
				return BadRequest();
			}

			if (string.IsNullOrWhiteSpace(request.InvoiceNo))
			{
				return BadRequest();
			}

			if (string.IsNullOrWhiteSpace(request.PaymentReference))
			{
				return BadRequest();
			}

			throw new NotImplementedException();
		}
	}
}
