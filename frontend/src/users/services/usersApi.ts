//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IUserListResponse{
   Name: string;
   Enabled: boolean;
   LastLogin: Date;
}

export interface IUsersListResponse extends IResponseBase {
   Users: Array<IUserListResponse>;
}

export interface IUsersClient {
    createUser(name: string, password: string, enabled: boolean): Promise<IResponseBase>;
    removeUser(name: string): Promise<IResponseBase>;
    updateUser(name: string, enabled: boolean): Promise<IResponseBase>;
    userChangePassword(user: string, newPassword: string, originPassword: string): Promise<IResponseBase>;
    usersList(): Promise<IUsersListResponse>;
}

class UsersClient implements IUsersClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async createUser(name: string, password: string, enabled: boolean): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/CreateUserCommand",{Name:name,Password:password,Enabled:enabled});
    }

    async removeUser(name: string): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/RemoveUserCommand",{Name:name});
    }

    async updateUser(name: string, enabled: boolean): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/UpdateUserCommand",{Name:name,Enabled:enabled});
    }

    async userChangePassword(user: string, newPassword: string, originPassword: string): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/UserChangePasswordCommand",{User:user,NewPassword:newPassword,OriginPassword:originPassword});
    }

    async usersList(): Promise<IUsersListResponse>{
        return this.profileApiClient.get<IUsersListResponse>("api/users/UsersListQuery");
    }

}

export const usersClient = new UsersClient(apiClient);
