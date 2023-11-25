using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers
{
	public class PaymentControllerTests
	{
		private PaymentCallbackRequest _request;

		private Mock<IInvoiceRepository> _invoiceRepositoryMock;

		[SetUp]
		public void Setup()
		{
			_request = new PaymentCallbackRequest
			{
				PaymentReference = "xx",
				InvoiceNo = "EASY0001",
				AmountPaid = 1000
			};

			_invoiceRepositoryMock = new Mock<IInvoiceRepository>(MockBehavior.Strict);

			_invoiceRepositoryMock
				.Setup(
					x => x.RegisterPaymentAsync(
						"EASY0001",
						1000
					)
				)
				.Returns(Task.FromResult(0))
				.Verifiable(Times.Once);
		}

		[TestCaseSource(nameof(GetCallbackThrowsArgumentExceptionWhenRequestIsNotValidCases))]
		public async Task CallbackBadRequestWhenRequestIsNotValid(
			PaymentCallbackRequest request)
		{
			_request = request;

			var result = await CallCallback();

			result
				.Should()
				.BeOfType<BadRequestResult>();
		}

		[Test]
		public async Task ReturnsOKWhenInvoiceIsFullyPaid()
		{
			var result = await CallCallback();

			result
				.Should()
				.BeOfType<OkResult>();
		}

		[Test]
		public async Task PaymentIsRegisteredWhenInvoiceIsFullyPaid()
		{
			await CallCallback();

			_invoiceRepositoryMock.Verify();
		}

		private async Task<IActionResult> CallCallback()
		{
			var sut = Create();

			return await sut
				.Callback(
					_request
				);
		}

		private PaymentController Create()
		{
			return new PaymentController(
				_invoiceRepositoryMock.Object
			);
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
	}
}