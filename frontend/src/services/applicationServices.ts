import { makeAutoObservable } from "mobx";
import AuthenticationService from "./autentication/authenticationService"

export class ApplicationServices {
    Authentification: AuthenticationService;
    constructor() {
        makeAutoObservable(this);
        this.Authentification = new AuthenticationService();
    }
}
