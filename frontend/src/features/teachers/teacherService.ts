import { apiClient } from "../../services/apiClient";
import type { Teacher, CreateTeacherDto, UpdateTeacherDto } from "./types";

export const teacherService = {
  getAll: async (): Promise<Teacher[]> => {
    const response = await apiClient.get<Teacher[]>("/teachers");
    return response.data;
  },

  getById: async (id: string): Promise<Teacher> => {
    const response = await apiClient.get<Teacher>(`/teachers/${id}`);
    return response.data;
  },

  create: async (dto: CreateTeacherDto): Promise<Teacher> => {
    const response = await apiClient.post<Teacher>("/teachers", dto);
    return response.data;
  },

  update: async (id: string, dto: UpdateTeacherDto): Promise<Teacher> => {
    const response = await apiClient.put<Teacher>(`/teachers/${id}`, dto);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/teachers/${id}`);
  },
};
