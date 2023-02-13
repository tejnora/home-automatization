import { makeAutoObservable } from "mobx";
import AutentificationService from "./autenticationService"

export class ApplicationServices {
    Authentification: AutentificationService;
    constructor() {
        makeAutoObservable(this);
        this.Authentification = new AutentificationService();
    }
}
