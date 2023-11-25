using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers
{
	public class PaymentControllerTests
	{
		private PaymentCallbackRequest _request;

		[SetUp]
		public void Setup()
		{
			_request = default;
		}
		
		[TestCaseSource(nameof(GetCallbackThrowsArgumentExceptionWhenRequestIsNotValidCases))]
		public void CallbackBadRequestWhenRequestIsNotValid(
			PaymentCallbackRequest request)
		{
			_request = request;

			var result = CallCallback();

			result
				.Should()
				.BeOfType<BadRequestResult>();
		}

		private static IEnumerable<TestCaseData> GetCallbackThrowsArgumentExceptionWhenRequestIsNotValidCases()
		{
			yield return new TestCaseData(
					new PaymentCallbackRequest
					{
						PaymentReference = "xx",
						InvoiceNo = "yy",
						AmountPaid = -1
					}
				)
				.SetName("Amount is negative");

			yield return new TestCaseData(
					new PaymentCallbackRequest
					{
						PaymentReference = "xx",
						InvoiceNo = "",
						AmountPaid = 10
					}
				)
				.SetName("InvoiceNo is not set");

			yield return new TestCaseData(
					new PaymentCallbackRequest
					{
						PaymentReference = "",
						InvoiceNo = "yy",
						AmountPaid = 10
					}
				)
				.SetName("PaymentReference is not set");
		}

		private IActionResult CallCallback()
		{
			var sut = Create();

			return sut
				.Callback(
					_request
				);
		}

		private PaymentController Create()
		{
			return new PaymentController();
		}
	}
}