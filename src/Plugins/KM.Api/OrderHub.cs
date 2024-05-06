namespace Km.Api;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var req = Context.GetHttpContext().Request;
        if (req.Query.TryGetValue("access_token", out var at))
        {
            var uid = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(at);
            if (uid != null)
            {
                await base.OnConnectedAsync();
                await Groups.AddToGroupAsync(Context.ConnectionId, uid.Uid);
            }
        }
    }
}
