using System.Collections.Generic;
using Server.Core;

namespace Server.Users.Responses;

public class UsersListResponse : Define.IResponse
{
    public ResponseType Result => ResponseType.Success;
    public IList<UserListResponse> Users { get; init; }  
}