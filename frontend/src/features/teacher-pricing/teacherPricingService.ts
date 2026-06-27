import { apiClient } from "../../services/apiClient";
import type {
  TeacherPricing,
  CreateTeacherPricingDto,
  UpdateTeacherPricingDto,
} from "./types";

export const teacherPricingService = {
  // جلب كافة الأسعار من السيرفر
  getAll: async (): Promise<TeacherPricing[]> => {
    const response = await apiClient.get<TeacherPricing[]>("/teacherpricing");
    return response.data;
  },

  // جلب تسعيرة محددة بواسطة المعرف
  getById: async (id: string): Promise<TeacherPricing> => {
    const response = await apiClient.get<TeacherPricing>(
      `/teacherpricing/${id}`,
    );
    return response.data;
  },

  // إضافة تسعيرة جديدة للمدرس
  create: async (dto: CreateTeacherPricingDto): Promise<string> => {
    const response = await apiClient.post<string>("/teacherpricing", dto);
    return response.data;
  },

  // تحديث سعر الحصة
  update: async (
    id: string,
    dto: UpdateTeacherPricingDto,
  ): Promise<boolean> => {
    const response = await apiClient.put<boolean>(`/teacherpricing/${id}`, dto);
    return response.data;
  },

  // حذف تسعيرة محددة
  delete: async (id: string): Promise<void> => {
    await apiClient.delete(`/teacherpricing/${id}`);
  },
};
