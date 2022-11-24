using Server.Core;
using Server.Users.Queries;

namespace Server.Users;

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