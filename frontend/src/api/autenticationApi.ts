import { IApiClient, apiClient } from '../core/api/apiClient';
import { IResponseBase } from "../core/api/responseBase"

export interface ILoginResponse extends IResponseBase {
    accessToken: string;
}

export interface ILogoutResponse extends IResponseBase {

}

export interface IAutentificationClient {
    login(userName: string, password: string): Promise<ILoginResponse>;
    logout(token: string): Promise<ILogoutResponse>;
}


class AutentificationClient implements IAutentificationClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
        this.profileApiClient = apiClient;
    }

    async login(userName: string, password: string): Promise<ILoginResponse> {
        return this.profileApiClient.post<ILoginResponse>("api/login", { userName: userName, password: password });
    }

    async logout(token: string): Promise<ILogoutResponse> {
        return this.profileApiClient.post<ILogoutResponse>("api/logout", { token: token });
    }
}

export const autentificationClient = new AutentificationClient(apiClient);