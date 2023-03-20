using BTDB.ODBLayer;

namespace Server.Users.Database;

public interface IUsersTable : IRelation<User>
{
    void Insert(User person);
    bool RemoveById(string name);
    User FindById(string id);
    bool UpdateById(string id, User user);
    
    bool Update(User user);
}