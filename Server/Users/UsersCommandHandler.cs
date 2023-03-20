using Server.Core;
using Server.Storage;
using Server.Users.Commands;
using Server.Users.Database;
using Server.Tools;

namespace Server.Users;

public class UsersCommandHandler
    : Define.IConsumer<InitCommand, GeneralResponses>
    , Define.IConsumer<CreateUserCommand, GeneralResponses>
    , Define.IConsumer<UpdateUserCommand, GeneralResponses>
    , Define.IConsumer<RemoveUserCommand, GeneralResponses>
{

    public GeneralResponses Consume(ICommandContext context, CreateUserCommand command)
    {
        var password = PasswordHasher.HashPassword(command.Password, out var salt);
        context.Table<IUsersTable>("Users").Insert(new User { Name = command.Name, Password = password, Salt = salt });
        return GeneralResponses.Success;
    }

    public GeneralResponses Consume(ICommandContext context, UpdateUserCommand command)
    {
        var password = PasswordHasher.HashPassword(command.Password, out var salt);
        return context.Table<IUsersTable>("Users").UpdateById(command.Name, new User { Name = command.Name, Password = password, Salt = salt })
            ? GeneralResponses.Success : GeneralResponses.Failed;
    }

    public GeneralResponses Consume(ICommandContext context, RemoveUserCommand command)
    {
        return context.Table<IUsersTable>("Users").RemoveById(command.Name) ? GeneralResponses.Success : GeneralResponses.Failed;
    }

    public GeneralResponses Consume(ICommandContext context, InitCommand command)
    {
        throw new System.NotImplementedException();
    }
}using Server.Core;
using Server.Storage;
using Server.Users.Commands;
using Server.Users.Database;
using Server.Tools;

namespace Server.Users;

public class UsersCommandHandler
    : Define.IConsumer<InitCommand, GeneralResponses>
    , Define.IConsumer<CreateUserCommand, GeneralResponses>
    , Define.IConsumer<UpdateUserCommand, GeneralResponses>
    , Define.IConsumer<RemoveUserCommand, GeneralResponses>
{

    public GeneralResponses Consume(ICommandContext context, CreateUserCommand command)
    {
        var password = PasswordHasher.HashPassword(command.Password, out var salt);
        context.Table<IUsersTable>("Users").Insert(new User { Name = command.Name, Password = password, Salt = salt });
        return GeneralResponses.Success;
    }

    public GeneralResponses Consume(ICommandContext context, UpdateUserCommand command)
    {
        var password = PasswordHasher.HashPassword(command.Password, out var salt);
        return context.Table<IUsersTable>("Users").Update(new User { Name = command.Name, Password = password, Salt = salt })
            ? GeneralResponses.Success : GeneralResponses.Failed;
    }

    public GeneralResponses Consume(ICommandContext context, RemoveUserCommand command)
    {
        return context.Table<IUsersTable>("Users").RemoveById(command.Name) ? GeneralResponses.Success : GeneralResponses.Failed;
    }

    public GeneralResponses Consume(ICommandContext context, InitCommand command)
    {
        var table = context.Table<IUsersTable>("Users");
        if(table.)
    }
}