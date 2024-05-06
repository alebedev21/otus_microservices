using Microsoft.EntityFrameworkCore;

namespace NotificationService.Entities;

[PrimaryKey(nameof(UserId))]
public class Notification
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public bool IsOrderCancelled { get; set; }
    public bool IsOrderCompleted { get; set; }
}
