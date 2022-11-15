using HttpServer.Users.Commands;
using Server.Core;
using Server.Core.Session;
using Server.Users.Commands;
using Server.Users.Responses;

namespace Server.Users
{
    public class UsersCommandHandler
        : Define.IConsumer<LoginCommand, LoginResponse>
        , Define.IConsumer<LoginByPernamentSessionIdCommand, LoginResponse>
        , Define.IConsumer<LogoutCommand, GeneralResponses>
    {
        ISessionManager _sessionManager;

        public UsersCommandHandler(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public GeneralResponses Consume(ICommandContext context, LogoutCommand command)
        {
            _sessionManager.RemoveSession(context.SessionId);
            return GeneralResponses.Success;
        }

        public LoginResponse Consume(ICommandContext context, LoginCommand command)
        {
            if (command.User == "david" && command.Password == "pass2")
            {
                var session = _sessionManager.CreateSession(command.User);
                var response = new LoginResponse { SessionId = session.SessionId, Result = ResponseType.Success };
                if (command.RememberMe)
                {
                    response.PernamentSessionId = "123dieyfsajfi30854j54092ddf4";
                }
                return response;
            }
            return new LoginResponse { Result = ResponseType.IncorrectLoginData };
        }

        public LoginResponse Consume(ICommandContext context, LoginByPernamentSessionIdCommand command)
        {
            if (command.PernamentSessionId == "123dieyfsajfi30854j54092ddf4")
            {
                var session = _sessionManager.CreateSession("david");
                return new LoginResponse
                {
                    SessionId = session.SessionId,
                    Result = ResponseType.Success,
                    PernamentSessionId = command.PernamentSessionId
                };
            }
            return new LoginResponse { Result = ResponseType.IncorrectLoginData };
        }
    }

}