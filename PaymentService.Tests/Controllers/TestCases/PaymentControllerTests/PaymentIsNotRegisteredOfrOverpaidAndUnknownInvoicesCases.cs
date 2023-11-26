using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using NUnit.Framework;
using System.Collections;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;

public class PaymentIsNotRegisteredOfrOverpaidAndUnknownInvoicesCases : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		yield return new TestCaseData(
				null //Set value for invoice
			)
			.SetName("Payment for unknown invoice was received.");

		yield return new TestCaseData(
				new Invoice("EASY0001", 500, 0) //Set value for invoice
			)
			.SetName("Invoice was overpaid.");


		yield return new TestCaseData(
				new Invoice("EASY0001", 500, 400) //Set value for invoice
			)
			.SetName("Partially paid invoice was overpaid.");
	}
}