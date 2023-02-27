using Server.Authentication.Commands;
using Server.Authentication.Responses;
using Server.Core;

namespace Server.Authentication
{
    public class AuthenticationCommandHandler
        : Define.IConsumer<LogoutCommand, GeneralResponses>
        , Define.IConsumer<LoginCommand, LoginResponse>
        , Define.IConsumer<PermanentLoginCommand, PermanentLoginResponse>

    {
        readonly ISessionManager _sessionManager;

        public AuthenticationCommandHandler(ISessionManager sessionManager)
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
            if (command.UserName != "david" || command.Password != "pass2")
                return new LoginResponse { Result = ResponseType.IncorrectLoginData };
            var session = _sessionManager.CreateSession(command.UserName);
            var response = new LoginResponse { SessionId = session.SessionId, Result = ResponseType.Success };
            if (command.RememberMe)
            {
                response.PermanentSessionId = "123dieyfsajfi30854j54092ddf4";
            }
            return response;
        }
        public PermanentLoginResponse Consume(ICommandContext context, PermanentLoginCommand command)
        {
            if (command.Token != "123dieyfsajfi30854j54092ddf4")
                return new PermanentLoginResponse { Result = ResponseType.IncorrectLoginData };
            var session = _sessionManager.CreateSession("david");
            return new PermanentLoginResponse
            {
                SessionId = session.SessionId,
                Result = ResponseType.Success,
                UserName = "david"
            };
        }

    }
}
