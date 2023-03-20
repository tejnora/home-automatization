//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IUsersClient {
    createUser(name: string, password: string,): Promise<IResponseBase>;
    removeUser(name: string,): Promise<IResponseBase>;
    updateUser(name: string, password: string,): Promise<IResponseBase>;
}

class UsersClient implements IUsersClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async createUser(name: string, password: string,): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/CreateUserCommand",{Name:name,Password:password});
    }

    async removeUser(name: string,): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/RemoveUserCommand",{Name:name});
    }

    async updateUser(name: string, password: string,): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/users/UpdateUserCommand",{Name:name,Password:password});
    }

}

export const usersClient = new UsersClient(apiClient);
