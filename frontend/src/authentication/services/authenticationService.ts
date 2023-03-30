import { authenticationClient, IAuthenticationClient } from "./authenticationApi"
import { observable, action, makeObservable } from "mobx";
import { ResponseStatus } from "../../core/api/responseBase"
import { getCookie, setCookie, usetCookie } from "../../core/cookie"

export default class AuthenticationService {
    private client: IAuthenticationClient;
    @observable
    logged = false;
    @observable
    userName = "";
    @observable
    logging = false;

    constructor() {
        makeObservable(this);
        this.client = authenticationClient;
    }

    @action
    async login(username: string, password: string, rememberMe: boolean): Promise<boolean> {
        if (this.logged || this.logging) return this.logged;
        this.logging = true;
        let response = await this.client.login(username, password, rememberMe);
        this.logged = response.Result === ResponseStatus.Success;
        if (this.logged) {
            if (rememberMe) {
                this.setPernamentSession(username, response.PermanentSessionId);
            }
            this.userName = username;
            this.setSessionId(response.SessionId);
        }
        this.logging = false;
        return this.logged;
    }
    
    @action
    async tryPernamentLogin(): Promise<boolean> {
        if (this.logged || this.logging) return this.logged;
        let pertnamentToken = this.getPernamentSessionId();
        if (pertnamentToken === undefined) return false;
        this.logging = true;
        let response = await this.client.permanentLogin(pertnamentToken[0], pertnamentToken[1]);
        this.logged = response.Result === ResponseStatus.Success;
        if (this.logged) {
            this.userName = response.UserName;
            this.setSessionId(response.SessionId);
        }
        else{
            this.unsetPernamentSession();
        }
        this.logging = false;
        return this.logged;
    }

    @action
    async logout() {
        if (!this.logged) return;
        await this.client.logout(this.userName);
        this.unsetSessionId();
        this.unsetPernamentSession();
        this.userName = ""
        this.logged = false;
    }

    private setPernamentSession(user: string, token: string) {
        setCookie("pernament-session-id", `${user};${token}`);
    }

    private getPernamentSessionId(): [string, string] | undefined {
        let value = getCookie("pernament-session-id");
        if (value === undefined) return;
        let values = value.split(';');
        if (values.length != 2) {
            this.unsetPernamentSession();
            return;
        }
        return [values[0], values[1]];
    };

    private unsetPernamentSession() {
        usetCookie("pernament-session-id");
    }

    private getSessionId(): string | undefined {
        let value = getCookie("session-id");
        return value;
    };

    private setSessionId(sessingId: string) {
        setCookie("session-id", sessingId);
    };

    private unsetSessionId() {
        usetCookie("session-id");
    };

}