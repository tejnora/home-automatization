import { makeAutoObservable } from "mobx";
import AuthenticationService from "./authentication/services/authenticationService"

export class ApplicationServices {
    Authentification: AuthenticationService;
    constructor() {
        makeAutoObservable(this);
        this.Authentification = new AuthenticationService();
    }
}
