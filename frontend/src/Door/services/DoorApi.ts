//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IDoorSettingResponse extends IResponseBase {
   Enable: boolean;
   Password: string;
}

export interface IDoorClient {
    doorSettings(): Promise<IDoorSettingResponse>;
    openDoor(): Promise<IResponseBase>;
    updateDoorSettings(enable: boolean, password: string): Promise<IResponseBase>;
}

class DoorClient implements IDoorClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async doorSettings(): Promise<IDoorSettingResponse>{
        return this.profileApiClient.post<IDoorSettingResponse>("api/door/DoorSettingsQuery",{});
    }

    async openDoor(): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/door/OpenDoorCommand",{});
    }

    async updateDoorSettings(enable: boolean, password: string): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/door/UpdateDoorSettingsCommand",{Enable:enable,Password:password});
    }

}

export const doorClient = new DoorClient(apiClient);
