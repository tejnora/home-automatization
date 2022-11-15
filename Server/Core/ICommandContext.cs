namespace Server.Core
{
    public interface ICommandContext
    {
        string SessionId { get; }
    }
}