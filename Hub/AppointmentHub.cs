using DoctorAppointmentWebApi.Messages;

namespace DoctorAppointmentWebApi.Hub;

using Microsoft.AspNetCore.SignalR;

public class AppointmentHub : Hub
{
    public async Task SendAppointmentNotification(AppointmentMessage message)
    {
        var notificationMessage = $"{message.PatientFullName}, {message.Status}";
        await Clients.All.SendAsync("ReceiveAppMessage", new { user = "Система", message = notificationMessage });
        Console.WriteLine($"Notification sent: {notificationMessage}");
    }
    
    // public async Task SendMessage(string user, string message)
    // {
    //     await Clients.All.SendAsync("ReceiveAppMessage", new {user, message});
    //     Console.WriteLine($"Message received: {message}");
    // }
}
