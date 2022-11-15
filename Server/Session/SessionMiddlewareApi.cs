using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Storage;

namespace Server.Core.Session
{
    public class SessionMiddlewareApi
        : IRestApi
    {
        readonly ISessionManager _sessionManager;
        readonly IRestApi _dataStorage;

        public SessionMiddlewareApi(IDataStorage dataStorage, ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _dataStorage = dataStorage as IRestApi; ;
        }

        bool CanAccess(string sessionId, object commnad)
        {
            var attribute = Attribute.IsDefined(commnad.GetType(), typeof(SessionAttribute));
            if (!attribute) return true;
            Session session;
            if (_sessionManager.TryGetSession(sessionId, out session))
            {
                return true;
            }
            return false;
        }

        public Task<List<Define.IResponse>> Command(ICommandContext commadContext, Define.ICommand commnad)
        {
            if (!CanAccess(commadContext.SessionId, commnad))
            {
                return Task.Factory.StartNew(() => new List<Define.IResponse> { GeneralResponses.AccessDenied });
            }
            return _dataStorage.Command(commadContext, commnad);
        }

        public Task<Define.IResponse> Query(IQueryContext queryContext, Define.IRequest query)
        {
            if (!CanAccess(queryContext.SessionId, query))
            {
                return Task.Factory.StartNew(() => (Define.IResponse)GeneralResponses.AccessDenied);
            }
            return _dataStorage.Query(queryContext, query);
        }


    }
}