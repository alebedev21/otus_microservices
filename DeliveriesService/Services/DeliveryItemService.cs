using DeliveriesService.Repositories;
using System.Text.Json;

namespace DeliveriesService.Services;

public class DeliveryItemService(IKafkaService kafkaService, ILogger<DeliveryItemService> logger, IDeliveryRepository repository) : IDeliveryItemService
{
    private readonly IKafkaService _kafkaService = kafkaService;
    private readonly ILogger<DeliveryItemService> _logger = logger;
    private readonly IDeliveryRepository _repository = repository;

    private readonly string _orderCompletedTopic = "order-compleated";

    public async Task<bool> CheckOrderReady(Guid userId, Guid orderId, string service)
    {
        bool isReady = await _repository.CheckOrderReady(userId, orderId, service);

        if (!isReady)
        {
            return false;
        }

        string message = JsonSerializer.Serialize(new { UserId = userId, OrderId = orderId });
        return await _kafkaService.Publish(_orderCompletedTopic, message);
    }
}
