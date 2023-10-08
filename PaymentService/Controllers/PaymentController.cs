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
			throw new NotImplementedException();
		}
	}
}
