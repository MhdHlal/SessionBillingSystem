import { apiClient } from "../../services/apiClient";
import type { Session, CreateSessionDto } from "./types";

export const sessionService = {
  getAll: async (): Promise<Session[]> => {
    const response = await apiClient.get<Session[]>("/sessions");
    return response.data;
  },

  getById: async (id: string): Promise<Session> => {
    const response = await apiClient.get<Session>(`/sessions/${id}`);
    return response.data;
  },

  create: async (dto: CreateSessionDto): Promise<string> => {
    const response = await apiClient.post<string>("/sessions", dto);
    return response.data;
  },

  complete: async (id: string): Promise<boolean> => {
    const response = await apiClient.put<boolean>(
      `/sessions/${id}/complete`,
      {},
    );
    return response.data;
  },

  cancel: async (id: string): Promise<boolean> => {
    const response = await apiClient.put<boolean>(`/sessions/${id}/cancel`, {});
    return response.data;
  },
};
