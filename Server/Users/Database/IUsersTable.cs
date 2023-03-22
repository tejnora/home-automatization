using BTDB.ODBLayer;

namespace Server.Users.Database;

public interface IUsersTable : IRelation<User>
{
    void Insert(User person);
    bool RemoveById(string name);
    User FindById(string name);
//    bool UpdateById(string name, User user);
    
    void Update(User user);
}