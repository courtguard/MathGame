using Microsoft.AspNetCore.SignalR;

namespace MathGame_Server;

public class GameHub : Hub<IGameClient>
{
    private static int userCount = 0;
    public override async Task OnConnectedAsync()
    {
        userCount++;

        await Clients.All.UserCountUpdated(
           userCount);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        userCount--;
        await Clients.All.UserCountUpdated(
                   userCount);
        await base.OnDisconnectedAsync(exception);

    }
}

public interface IGameClient
{
    Task ReceiveMathProblem(string problem);
    Task UserCountUpdated(int userCount);
}
