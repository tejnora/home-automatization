using Server.Authentication;
using Server.Core;
using Share;

namespace Server.ImageGallery.Queries;

[WebGet]
[Session]

public class ListOfImageGroupsQuery
    : Define.IRequest
{
}