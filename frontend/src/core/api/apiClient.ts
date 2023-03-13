import Axios, { AxiosInstance } from 'axios';
import { Logger } from "../logger"

export interface IApiClient {
  post<TResponse>(path: string, payload: any): Promise<TResponse>;
  patch<TRequest, TResponse>(path: string, object: TRequest): Promise<TResponse>;
  put<TRequest, TResponse>(path: string, object: TRequest): Promise<TResponse>;
  get<TResponse>(path: string): Promise<TResponse>;
}

class ApiClient implements IApiClient {
  private client: AxiosInstance;

  protected createAxiosClient(): AxiosInstance {
    return Axios.create({
      baseURL: /*AppConfig.baseURL*/"",
      responseType: 'json' as const,
      headers: {
        'Content-Type': 'application/json',
      },
      timeout: 10 * 1000,
    });
  }

  constructor() {
    this.client = this.createAxiosClient();
  }

  async post<TResponse>(path: string, payload: any): Promise<TResponse> {
    try {
      const response = await this.client.post<TResponse>(path, payload, {withCredentials: true});
      return response.data;
    } catch (error) {
      Logger.logServiceError(error);
    }
    return {} as TResponse;
  }

  async patch<TRequest, TResponse>(path: string, payload: TRequest): Promise<TResponse> {
    try {
      const response = await this.client.patch<TResponse>(path, payload);
      return response.data;
    } catch (error) {
      Logger.logServiceError(error);
    }
    return {} as TResponse;
  }

  async put<TRequest, TResponse>(path: string, payload: TRequest): Promise<TResponse> {
    try {
      const response = await this.client.put<TResponse>(path, payload);
      return response.data;
    } catch (error) {
      Logger.logServiceError(error);
    }
    return {} as TResponse;
  }

  async get<TResponse>(path: string): Promise<TResponse> {
    try {
      const response = await this.client.get<TResponse>(path);
      return response.data;
    } catch (error) {
      Logger.logServiceError(error);
    }
    return {} as TResponse;
  }
}

export const apiClient = new ApiClient();