import { apiClient } from "../../services/apiClient";
import type { Student, CreateStudentDto, UpdateStudentDto } from "./types";

export const studentService = {
  getAll: async (): Promise<Student[]> => {
    const response = await apiClient.get<Student[]>("/students");
    return response.data;
  },

  getById: async (id: string): Promise<Student> => {
    const response = await apiClient.get<Student>(`/students/${id}`);
    return response.data;
  },

  create: async (dto: CreateStudentDto): Promise<Student> => {
    const response = await apiClient.post<Student>("/students", dto);
    return response.data;
  },

  update: async (id: string, dto: UpdateStudentDto): Promise<Student> => {
    const response = await apiClient.put<Student>(`/students/${id}`, dto);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/students/${id}`);
  },
};
