using System.Linq;
using Server.Core;
using Server.Users.Database;
using Server.Users.Queries;
using Server.Users.Responses;

namespace Server.Users;

public class UsersQueryHandler
: Define.IQuery<UsersListQuery, UsersListResponse>
{
    public UsersListResponse Consume(IQueryContext consumeContext, UsersListQuery request)
    {
        var users = consumeContext.Table<IUsersTable>("Users");
        return new UsersListResponse { Users = users.Select((n) => n.Name).ToList() };
    }
}
