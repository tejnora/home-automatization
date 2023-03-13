import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface ILoginResponse extends IResponseBase {
    SessionId: string;
    PermanentSessionId: string;
}

export interface IPermanentLoginResponse extends IResponseBase {
    SessionId: string;
    UserName: string;
}

export interface ILogoutResponse extends IResponseBase {

}

export interface IAuthenticationClient {
    login(userName: string, password: string): Promise<ILoginResponse>;
    pernamentLogin(token: string): Promise<IPermanentLoginResponse>;
    logout(token: string): Promise<ILogoutResponse>;
}


class AuthenticationClient implements IAuthenticationClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
        this.profileApiClient = apiClient;
    }

    async login(userName: string, password: string): Promise<ILoginResponse> {
        return this.profileApiClient.post<ILoginResponse>("api/authentication/LoginCommand", { UserName: userName, Password: password });
    }

    async pernamentLogin(token: string): Promise<IPermanentLoginResponse>{
        return this.profileApiClient.post<IPermanentLoginResponse>("api/authentication/PermanentLoginCommand", { Token: token});
    }

    async logout(userName: string): Promise<ILogoutResponse> {
        return this.profileApiClient.post<ILogoutResponse>("api/authentication/LogoutCommand", { UserName: userName });
    }
}

export const authenticationClient = new AuthenticationClient(apiClient);