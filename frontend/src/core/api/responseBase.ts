
export enum ResponseStatus {
    Success = "Success",
    Failed = "Failed"
}

export interface IResponseBase {
    Result: ResponseStatus;
}
