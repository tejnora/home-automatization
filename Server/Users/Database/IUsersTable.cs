using BTDB.ODBLayer;

namespace Server.Users.Database;

public interface IUsersTable : IRelation<User>
{
    void Insert(User person);
    bool RemoveById(string name);
    User FindByIdOrDefault(string name);
    void Update(User user);
}