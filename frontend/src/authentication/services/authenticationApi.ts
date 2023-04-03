//Whole file is generated
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

export interface IAuthenticationClient {
    login(userName: string, password: string, rememberMe: boolean): Promise<ILoginResponse>;
    logout(user: string): Promise<IResponseBase>;
    permanentLogin(name: string, token: string): Promise<IPermanentLoginResponse>;
}

class AuthenticationClient implements IAuthenticationClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async login(userName: string, password: string, rememberMe: boolean): Promise<ILoginResponse>{
        return this.profileApiClient.post<ILoginResponse>("api/authentication/LoginCommand",{UserName:userName,Password:password,RememberMe:rememberMe});
    }

    async logout(user: string): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/authentication/LogoutCommand",{User:user});
    }

    async permanentLogin(name: string, token: string): Promise<IPermanentLoginResponse>{
        return this.profileApiClient.post<IPermanentLoginResponse>("api/authentication/PermanentLoginCommand",{Name:name,Token:token});
    }

}

export const authenticationClient = new AuthenticationClient(apiClient);
