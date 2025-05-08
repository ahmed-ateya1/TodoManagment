namespace TodoManagment.Core.ServiceContract
{
    public interface INotificationService
    {
        Task SendAsync(string subject, string message);
    }
}
