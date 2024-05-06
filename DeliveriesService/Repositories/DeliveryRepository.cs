
using DeliveriesService.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DeliveriesService.Repositories;

public class DeliveryRepository(ILogger<DeliveryRepository> logger, IServiceScopeFactory scopeFactory) : IDeliveryRepository
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<DeliveryRepository> _logger = logger;

    private readonly string _billingService = "billing"; 
    private readonly string _storageService = "storage";

    public async Task<bool> CheckOrderReady(Guid userId, Guid orderId, string service)
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
        using var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

        try
        {
            var delivery = await context.Deliveries.FirstOrDefaultAsync(x => x.OrderId == orderId);

            //no entry yet
            if (delivery == null)
            {
                delivery = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OrderId = orderId,
                    IsBillingOk = (_billingService == service),
                    IsStorageOk = (_storageService == service),
                };

                await context.Deliveries.AddAsync(delivery);
                await context.SaveChangesAsync();
                transaction.Commit();
                
                return false;
            }

            //already cancelled
            if (!delivery.IsStorageOk || !delivery.IsBillingOk)
            {
                transaction.Rollback();
                return false;
            }

            //last confirmation received
            if(service == _billingService)
            {
                delivery.IsBillingOk = true;
            }
            else if(service == _storageService)
            {
                delivery.IsStorageOk = true;
            }

            await context.SaveChangesAsync();
            transaction.Commit();
            
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Check order ready error");

            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> CancelOrder(Guid userId, Guid orderId, string service)
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<DeliveryDbContext>();
        using var transaction = context.Database.BeginTransaction();

        try
        {
            var delivery = await context.Deliveries.FirstOrDefaultAsync(x => x.OrderId == orderId);

            //no entry yet
            if (delivery == null)
            {
                delivery = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OrderId = orderId,
                    IsBillingOk = (_billingService == service),
                    IsStorageOk = (_storageService == service),
                };

                await context.Deliveries.AddAsync(delivery);
                await context.SaveChangesAsync();
                transaction.Commit();

                //create and cancel
                return true;
            }

            //already cancelled
            if (!delivery.IsStorageOk || !delivery.IsBillingOk)
            {
                transaction.Rollback();
                return false;
            }

            //first cancellation received
            if (service == _billingService)
            {
                delivery.IsBillingOk = false;
            }
            else if (service == _storageService)
            {
                delivery.IsStorageOk = false;
            }

            await context.SaveChangesAsync();
            transaction.Commit();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Check order ready error");

            transaction.Rollback();
            return false;
        }
    }
}
