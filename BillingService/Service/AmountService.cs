using BillingService.Entities;
using BillingService.Repositories;
using System.Text.Json;

namespace BillingService.Service;

public class AmountService(IKafkaService kafkaService, ILogger<AmountService> logger, IAmountRepository repository) : IAmountService
{
    private readonly IKafkaService _kafkaService = kafkaService;
    private readonly ILogger<AmountService> _logger = logger;
    private readonly IAmountRepository _repository = repository;

    private readonly string _readyTopic = "ready";
    private readonly string _cancelOrderTopic = "cancel-order";

    private readonly string _service = "billing";

    public async Task<bool> PrepareOrder(Guid userId, Guid orderId, decimal funds)
    {
        bool isReady = await _repository.ReserveMoney(userId, funds);

        if (isReady)
        {
            if(await SendOrderMessage(userId, orderId, _readyTopic))
            {
                return true;
            }

            _logger.LogError("Can't publish 'ready' message. Order is cancelled.");
        }

        _logger.LogWarning("Insufficient funds");
        await SendOrderMessage(userId, orderId, _cancelOrderTopic);
        return false;
    }

    private async Task<bool> SendOrderMessage(Guid userId, Guid orderId, string topic)
    {
        string message = JsonSerializer.Serialize(new { UserId = userId, OrderId = orderId, Service = _service });
        return await _kafkaService.Publish(topic, message);
    }
}
