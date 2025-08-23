using ExampleAbpApplicationLayer.OrderItems;

using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace ExampleAbpApplicationLayer.Orders;

public class OrderDeletedEventHandler : ILocalEventHandler<EntityDeletedEventData<Order>>, ITransientDependency
{
    private readonly IOrderItemRepository _orderItemRepository;

    public OrderDeletedEventHandler(IOrderItemRepository orderItemRepository)
    {
        _orderItemRepository = orderItemRepository;

    }

    public async Task HandleEventAsync(EntityDeletedEventData<Order> eventData)
    {
        if (eventData.Entity is not ISoftDelete softDeletedEntity)
        {
            return;
        }

        if (!softDeletedEntity.IsDeleted)
        {
            return;
        }

        try
        {
            await _orderItemRepository.DeleteManyAsync(await _orderItemRepository.GetListByOrderIdAsync(eventData.Entity.Id));

        }
        catch
        {
            //...
        }
    }
}