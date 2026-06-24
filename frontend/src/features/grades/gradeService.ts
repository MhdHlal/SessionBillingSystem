import { apiClient } from "../../services/apiClient";
import type { Grade, CreateGradeDto, UpdateGradeDto } from "./types";

export const gradeService = {
  getAll: async (): Promise<Grade[]> => {
    const response = await apiClient.get<Grade[]>("/grades");
    return response.data;
  },

  getById: async (id: string): Promise<Grade> => {
    const response = await apiClient.get<Grade>(`/grades/${id}`);
    return response.data;
  },

  create: async (dto: CreateGradeDto): Promise<Grade> => {
    const response = await apiClient.post<Grade>("/grades", dto);
    return response.data;
  },

  update: async (id: string, dto: UpdateGradeDto): Promise<Grade> => {
    const response = await apiClient.put<Grade>(`/grades/${id}`, dto);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/grades/${id}`);
  },
};
