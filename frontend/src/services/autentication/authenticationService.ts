import { authenticationClient, IAuthenticationClient } from "./authenticationApi"
import { observable, action, makeObservable } from "mobx";
import { ResponseStatus } from "../../core/api/responseBase"
import { setSessionId, unsetSessionId, unsetPernametSessionId } from "../../core/cookie"

export default class AuthenticationService {
    private client: IAuthenticationClient;
    @observable
    logged = false;
    @observable
    userName = "";

    constructor() {
        makeObservable(this);
        this.client = authenticationClient;
    }

    @action
    async login(username: string, password: string): Promise<boolean> {
        if (this.logged) return this.logged;
        let response = await this.client.login(username, password);
        this.logged = response.Result == ResponseStatus.Success;
        if (this.logged) {
            this.userName = username;
            setSessionId(response.SessionId);
        }
        return this.logged;
    }

    async tryPernamentLogin(token: string): Promise<boolean> {
        if (this.logged) return this.logged;
        let response = await this.client.pernamentLogin(token);
        this.logged = response.Result == ResponseStatus.Success;
        if (this.logged) {
            this.userName = response.UserName;
            setSessionId(response.SessionId);
        }
        return this.logged;
    }

    @action
    async logout() {
        if (!this.logged) return;
        await this.client.logout(this.userName);
        unsetSessionId();
        unsetPernametSessionId();
        this.userName = ""
        this.logged = false;
    }

}