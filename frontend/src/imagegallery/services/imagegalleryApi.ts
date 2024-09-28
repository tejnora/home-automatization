//Whole file is generated
import { IApiClient, apiClient } from '../../core/api/apiClient';
import { IResponseBase } from "../../core/api/responseBase"

export interface IImageInfo{
   Name: string;
   Src: string;
}

export interface IImagesListResponse extends IResponseBase {
   Images: Array<IImageInfo>;
}

export interface IImageGroupsResponse extends IResponseBase {
   ImagesGroups: Array<string>;
}

export interface IImageGalleryClient {
    imagesList(imagesGroup: string): Promise<IImagesListResponse>;
    listOfImageGroups(): Promise<IImageGroupsResponse>;
}

class ImageGalleryClient implements IImageGalleryClient {
    private profileApiClient: IApiClient;

    constructor(profileApiClient: IApiClient) {
       this.profileApiClient = apiClient;
    }

    async imagesList(imagesGroup: string): Promise<IImagesListResponse>{
        return this.profileApiClient.getWithParams<IImagesListResponse>("api/imagegallery/ImagesListQuery", {ImagesGroup:imagesGroup});
    }

    async listOfImageGroups(): Promise<IImageGroupsResponse>{
        return this.profileApiClient.get<IImageGroupsResponse>("api/imagegallery/ListOfImageGroupsQuery");
    }

}

export const imagegalleryClient = new ImageGalleryClient(apiClient);
