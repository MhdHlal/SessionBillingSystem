import axios from "axios";
import type { AxiosInstance, AxiosResponse } from "axios";

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  errors: {
    code?: string;
    message?: string;
  }[];
}

const baseURL = import.meta.env.VITE_API_URL || "http://localhost:5135/api/v1";

const axiosInstance: AxiosInstance = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

const responseBody = <T>(
  response: AxiosResponse<ApiResponse<T>>,
): ApiResponse<T> => response.data;

export const apiClient = {
  get: <T>(url: string) =>
    axiosInstance.get<ApiResponse<T>>(url).then(responseBody),
  post: <T>(url: string, body: unknown) =>
    axiosInstance.post<ApiResponse<T>>(url, body).then(responseBody),
  put: <T>(url: string, body: unknown) =>
    axiosInstance.put<ApiResponse<T>>(url, body).then(responseBody),
  delete: <T>(url: string) =>
    axiosInstance.delete<ApiResponse<T>>(url).then(responseBody),
};
