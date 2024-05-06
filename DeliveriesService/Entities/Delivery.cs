using Microsoft.EntityFrameworkCore;

namespace DeliveriesService.Entities;

[PrimaryKey(nameof(Id))]
public class Delivery
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public bool IsBillingOk { get; set; }
    public bool IsStorageOk { get; set; }
}
