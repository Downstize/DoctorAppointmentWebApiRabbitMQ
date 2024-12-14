namespace DoctorAppointmentWebApi.Hub;

using Microsoft.AspNetCore.SignalR;

public class AppointmentHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveAppMessage", new {user, message});
        Console.WriteLine($"Message received: {message}");
    }
}
