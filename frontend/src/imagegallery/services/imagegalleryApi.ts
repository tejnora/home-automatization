//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IImagesListResponse extends IResponseBase {
   Images: Array<string>;
}

export interface IImageGroupsResponse extends IResponseBase {
   ImagesGroups: Array<string>;
}

export interface IImageGalleryClient {
    imagesList(nameOfGroup: string): Promise<IImagesListResponse>;
    listOfImageGroups(): Promise<IImageGroupsResponse>;
}

class ImageGalleryClient implements IImageGalleryClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async imagesList(nameOfGroup: string): Promise<IImagesListResponse>{
        return this.profileApiClient.get<IImagesListResponse>("api/imagegallery/ImagesListQuery");
    }

    async listOfImageGroups(): Promise<IImageGroupsResponse>{
        return this.profileApiClient.get<IImageGroupsResponse>("api/imagegallery/ListOfImageGroupsQuery");
    }

}

export const imagegalleryClient = new ImageGalleryClient(apiClient);
