import { autentificationClient, IAutentificationClient } from "../api/autenticationApi"
import { observable, action, makeObservable } from "mobx";
import { ResponseStatus } from "../core/api/responseBase"
import { apiConfiguration } from "../core/api/apiConfiguration"

export default class AutentificationService {
    private client: IAutentificationClient;
    @observable
    logged = false;
    constructor() {
        makeObservable(this);
        this.client = autentificationClient;
    }

    @action
    async login(username: string, password: string): Promise<boolean> {
        if (this.logged) return this.logged;
        this.logged = true;
        /*        let response = await this.client.login(username, password);
                this.logged = response.status == ResponseStatus.Success;
                if (this.logged) {
                    apiConfiguration.accessToken = response.accessToken;
                }
        */
        return this.logged;
    }

    @action
    logout() {
        if (!this.logged) return;
        //this.client.logout(apiConfiguration.accessToken);
        this.logged = false;
    }
}