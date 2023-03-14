//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IDoorSettingResponse extends IResponseBase {
   Enable: boolean;
   Password: string;
}

export interface IDoorClient {
    DoorSettings(): Promise<IDoorSettingResponse>;
    OpenDoor(): Promise<IResponseBase>;
    UpdateDoorSettings(enable: boolean, password: string,): Promise<IResponseBase>;
}

class DoorClient implements IDoorClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async DoorSettings(): Promise<IDoorSettingResponse>{
        return this.profileApiClient.post<IDoorSettingResponse>("api/door/DoorSettingsQuery",{});
    }

    async OpenDoor(): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/door/OpenDoorCommand",{});
    }

    async UpdateDoorSettings(enable: boolean, password: string,): Promise<IResponseBase>{
        return this.profileApiClient.post<IResponseBase>("api/door/UpdateDoorSettingsCommand",{Enable:enable,Password:password});
    }

}

export const doorClient = new DoorClient(apiClient);
