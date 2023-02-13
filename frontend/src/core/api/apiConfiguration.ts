export class ApiConfiguration {
    private _accessToken = "";
    public get accessToken(): string {
        return this._accessToken;
    }
    public set accessToken(value: string) {
        this._accessToken = value;
    }
}

export const apiConfiguration = new ApiConfiguration();