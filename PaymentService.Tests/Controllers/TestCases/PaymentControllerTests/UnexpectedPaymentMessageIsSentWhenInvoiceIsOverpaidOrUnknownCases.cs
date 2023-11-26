using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using NUnit.Framework;
using System;
using System.Collections;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;

public class UnexpectedPaymentMessageIsSentWhenInvoiceIsOverpaidOrUnknownCases : IEnumerable
{
	public IEnumerator GetEnumerator()
	{
		yield return new TestCaseData(
				new Invoice("EASY0002", 1000, 0), //Set value for invoice
				"Payment received for unknown invoice." //Set value for message
			)
			.SetName("Payment for unknown invoice was received.");

		yield return new TestCaseData(
				new Invoice("EASY0001", 500, 0), //Set value for invoice
				"Invoice is overpaid." //Set value for message
			)
			.SetName("Invoice was overpaid.");


		yield return new TestCaseData(
				new Invoice("EASY0001", 500, 400), //Set value for invoice
				"Invoice is overpaid." //Set value for message
			)
			.SetName("Partially paid invoice was overpaid.");
	}
}