using HttpServer.Users.Queries;
using Server.Core;

namespace Server.Users
{
    public class UsersQueryHandler
        : Define.IQuery<CheckAuthorizationQuery, GeneralResponses>
    {
        public UsersQueryHandler()
        {
        }

        public GeneralResponses Consume(IQueryContext consumeContext, CheckAuthorizationQuery request)
        {
            return GeneralResponses.Success;
        }
    }
}