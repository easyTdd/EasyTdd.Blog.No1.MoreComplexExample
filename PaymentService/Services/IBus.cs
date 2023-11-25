namespace EasyTdd.Blog.No1.MoreComplexExample.PaymentService.Services;

public interface IBus
{
	Task PublishAsync<TMessage>(TMessage message);
}