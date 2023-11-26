using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using NUnit.Framework;
using System.Collections;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;

public class PaymentIsRegisteredWhenPaymentIsReceivedCases : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		yield return new TestCaseData(
				new Invoice("EASY0001", 1000, 0)
			)
			.SetName("Invoice is fully paid");

		yield return new TestCaseData(
				new Invoice("EASY0001", 1500, 0)
			)
			.SetName("Invoice is partially paid");

		yield return new TestCaseData(
				new Invoice("EASY0001", 1500, 500)
			)
			.SetName("Partially paid invoice is fully paid");

		yield return new TestCaseData(
				new Invoice("EASY0001", 1500, 100)
			)
			.SetName("Partially paid invoice is partially paid");
	}
}