using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Core
{
    public interface IRestApi
    {
        Task<List<Define.IResponse>> Command(ICommandContext commadContext, Define.ICommand commnad);
        Task<Define.IResponse> Query(IQueryContext queryContext, Define.IRequest query);
    }
}