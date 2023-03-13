import { makeAutoObservable } from "mobx";
import AuthenticationService from "./autentication/services/authenticationService"

export class ApplicationServices {
    Authentification: AuthenticationService;
    constructor() {
        makeAutoObservable(this);
        this.Authentification = new AuthenticationService();
    }
}
