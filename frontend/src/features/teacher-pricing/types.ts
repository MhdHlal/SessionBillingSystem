export interface TeacherPricing {
  id: string;
  teacherId: string;
  teacherName: string;
  gradeId: string;
  gradeName: string;
  sessionPrice: number;
}

export interface CreateTeacherPricingDto {
  teacherId: string;
  gradeId: string;
  sessionPrice: number;
}

export interface UpdateTeacherPricingDto {
  id: string;
  newPrice: number;
}
