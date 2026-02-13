using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class SessionHub : Hub
{
    public async Task JoinSession(string sessionCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionCode);
        await Clients.Group(sessionCode).SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveSession(string sessionCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionCode);
        await Clients.Group(sessionCode).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task PushQuestion(string sessionCode, string questionId)
    {
        await Clients.Group(sessionCode).SendAsync("QuestionPushed", questionId);
    }

    public async Task SubmitAnswer(string sessionCode, object answerData)
    {
        await Clients.Group(sessionCode).SendAsync("AnswerReceived", answerData);
    }

    public async Task JoinAsPresenter(string sessionCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"{sessionCode}-presenter");
    }
}
