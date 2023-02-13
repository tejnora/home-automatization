
export enum ResponseStatus {
    Success = 0,
    Failed
}

export interface IResponseBase {
    status: ResponseStatus;
}
