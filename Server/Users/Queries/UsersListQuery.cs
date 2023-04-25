using Server.Authentication;
using Server.Core;
using Share;

namespace Server.Users.Queries;

[WebGet]
[Session]
public class UsersListQuery : Define.IRequest
{
}