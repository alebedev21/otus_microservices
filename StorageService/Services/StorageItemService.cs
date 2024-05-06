using StorageService.Repositoreis;
using System.Text.Json;

namespace StorageService.Services;

public class StorageItemService(
    ILogger<StorageItemService> loger, 
    IStorageRepository repository, 
    IKafkaService kafkaService) : IStorageItemService
{
    private readonly ILogger<StorageItemService> _logger = loger;
    private readonly IStorageRepository _repository = repository;
    private readonly IKafkaService _kafkaService = kafkaService;

    private readonly string _readyTopic = "ready";
    private readonly string _cancelOrderTopic = "cancel-order";

    private readonly string _service = "storage";

    public async Task<bool> PrepareOrder(Guid userId, int quantity)
    {
        bool isReady = await _repository.ReserveItems(userId, quantity);

        if (isReady)
        {
            if (await GetReady(userId))
            {
                return true;
            }

            _logger.LogError("Can't publish 'ready' message. Order is cancelled.");
        }

        _logger.LogWarning("Insufficient goods");
        await CancelOrder(userId);
        return false;
    }

    private async Task<bool> GetReady(Guid userId)
    {
        string message = JsonSerializer.Serialize(new { UserId = userId, Service = _service });
        return await _kafkaService.Publish(_readyTopic, message);
    }

    private async Task<bool> CancelOrder(Guid userId)
    {
        string message = JsonSerializer.Serialize(new { UserId = userId, Service = _service });
        return await _kafkaService.Publish(_cancelOrderTopic, message);
    }
}
