﻿using Server.Core;
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
    , Define.IConsumer<UserChangePasswordCommand, GeneralResponses>
{

    public GeneralResponses Consume(ICommandContext context, CreateUserCommand command)
    {
        var usersTable = context.Table<IUsersTable>();
        var user = usersTable.FindByIdOrDefault(command.Name);
        if (user != null)
        {
            return GeneralResponses.Failed;
        }
        var password = PasswordHasher.HashPassword(command.Password, out var salt);
        context.Table<IUsersTable>().Insert(new User { Name = command.Name, Password = password, Salt = salt, Enabled = command.Enabled});
        return GeneralResponses.Success;
    }

    public GeneralResponses Consume(ICommandContext context, UpdateUserCommand command)
    {
        var usersTable = context.Table<IUsersTable>();
        var user = usersTable.FindByIdOrDefault(command.Name);
        if (user == null)
        {
            return GeneralResponses.Failed;
        }
        user.Enabled = command.Enabled;
        context.Table<IUsersTable>().Update(user);
        return GeneralResponses.Success;
    }

    public GeneralResponses Consume(ICommandContext context, RemoveUserCommand command)
    {
        return context.Table<IUsersTable>().RemoveById(command.Name) ? GeneralResponses.Success : GeneralResponses.Failed;
    }

    public GeneralResponses Consume(ICommandContext context, InitCommand command)
    {
        if (context.Table<IUsersTable>().Count != 0)
            return GeneralResponses.Success;
        Consume(context, new CreateUserCommand { Name = "admin", Password = "pass", Enabled = true });
        return GeneralResponses.Success;
    }

    public GeneralResponses Consume(ICommandContext context, UserChangePasswordCommand command)
    {
        var usersTable = context.Table<IUsersTable>();
        var user = usersTable.FindByIdOrDefault(command.User);
        if (user == null || !PasswordHasher.VerifyPassword(command.OriginPassword, user.Password, user.Salt)) return GeneralResponses.Failed;
        user.Password = PasswordHasher.HashPassword(command.NewPassword, out var salt);
        user.Salt = salt;
        usersTable.Update(user);
        return GeneralResponses.Success;
    }
}