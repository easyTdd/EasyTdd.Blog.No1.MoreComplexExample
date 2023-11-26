using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Controllers;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;
using EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers.TestCases.PaymentControllerTests;

namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Tests.Controllers
{
	public class PaymentControllerTests
	{
		private PaymentCallbackRequest _request;

		private Invoice _invoiceRepositoryResult;

		private Mock<IInvoiceRepository> _invoiceRepositoryMock;
		private Mock<IBus> _busMock;

		[SetUp]
		public void Setup()
		{
			_request = new PaymentCallbackRequest
			{
				PaymentReference = "xx",
				InvoiceNo = "EASY0001",
				AmountPaid = 1000
			};

			_invoiceRepositoryResult = new Invoice("EASY0001", 1000, 0);

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

			_invoiceRepositoryMock
				.Setup(x => x.GetByInvoiceNoAsync("EASY0001"))
				.ReturnsAsync(
					() => _invoiceRepositoryResult
				);

			_busMock = new Mock<IBus>();

			_busMock
				.Setup(
					x => x.PublishAsync<Paid>(
						It.IsAny<Paid>()
					)
				);
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

		[TestCaseSource(typeof(PaymentIsRegisteredWhenPaymentIsReceivedCases))]
		public async Task PaymentIsRegisteredWhenPaymentIsReceived(
			Invoice invoice)
		{
			_invoiceRepositoryResult = invoice;

			await CallCallback();

			_invoiceRepositoryMock.Verify();
		}

		[TestCaseSource(typeof(PaidMessageIsSentWhenInvoiceIsFullyPaidCases))]
		public async Task PaidMessageIsSentWhenInvoiceIsFullyPaid(
			Invoice invoice)
		{
			await CallCallback();

			_busMock
				.Verify(
					x => x.PublishAsync(
						It.Is<Paid>(x => x.InvoiceNo == "EASY0001")
					)
				);
		}

		[TestCaseSource(typeof(PartiallyPaidMessageIsSentWhenInvoiceIsPartiallyPaidCases))]
		public async Task PartiallyPaidMessageIsSentWhenInvoiceIsPartiallyPaid(
			Invoice invoice)
		{
			_invoiceRepositoryResult = invoice;

			await CallCallback();

			_busMock
				.Verify(
					x => x.PublishAsync(
						It.Is<PartiallyPaid>(x => x.InvoiceNo == "EASY0001")
					)
				);
		}

		[Test]
		[TestCaseSource(typeof(UnexpectedPaymentMessageIsSentWhenInvoiceIsOverpaidOrUnknownCases))]
		public async Task UnexpectedPaymentMessageIsSentWhenInvoiceIsOverpaidOrUnknown(
			Invoice invoice,
			string message)
		{
			_invoiceRepositoryResult = invoice;

			await CallCallback();

			_busMock
				.Verify(
					x => x.PublishAsync(
						It.Is<UnexpectedPayment>(
							x => x.InvoiceNo == invoice.InvoiceNo
							&& x.Message == message
						)
					)
				);
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
				_invoiceRepositoryMock.Object,
				_busMock.Object
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