using System;
using Server.Authentication.Commands;
using Server.Authentication.Responses;
using Server.Core;
using Server.Tools;
using Server.Users.Database;

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
            var table = context.Table<IUsersTable>();
            var user = table.FindByIdOrDefault(command.User);
            if (user == null)
                return GeneralResponses.Failed;
            if (!string.IsNullOrEmpty(user.PermanentSessionId))
            {
                user.PermanentSessionId = "";
                table.Update(user);
            }
            _sessionManager.RemoveSession(context.SessionId);
            return GeneralResponses.Success;
        }

        public LoginResponse Consume(ICommandContext context, LoginCommand command)
        {
            var usersTable = context.Table<IUsersTable>();
            var user = usersTable.FindByIdOrDefault(command.UserName);
            if (user == null || !user.Enabled || !PasswordHasher.VerifyPassword(command.Password, user.Password, user.Salt))
                return new LoginResponse { Result = ResponseType.IncorrectLoginData };
            user.LastLogin = DateTime.UtcNow;
            usersTable.Update(user);
            var session = _sessionManager.CreateSession(command.UserName);
            var response = new LoginResponse { SessionId = session.SessionId, Result = ResponseType.Success };
            if (command.RememberMe)
            {
                response.PermanentSessionId = PasswordHasher.GeneratePermanentSessionId();
                user.PermanentSessionId = response.PermanentSessionId;
                usersTable.Update(user);
            }
            return response;
        }
        public PermanentLoginResponse Consume(ICommandContext context, PermanentLoginCommand command)
        {
            var usersTable = context.Table<IUsersTable>();
            var user = usersTable.FindByIdOrDefault(command.Name);
            if (user == null || !user.Enabled || user.PermanentSessionId != command.Token)
                return new PermanentLoginResponse { Result = ResponseType.IncorrectLoginData };
            user.LastLogin = DateTime.UtcNow;
            usersTable.Update(user);
            var session = _sessionManager.CreateSession(user.Name);
            return new PermanentLoginResponse
            {
                SessionId = session.SessionId,
                Result = ResponseType.Success,
                UserName = user.Name
            };
        }

    }
}
