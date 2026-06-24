export interface Student {
  studentId: string;
  name: string;
  gradeId: string;
  gradeName?: string;
}

export interface CreateStudentDto {
  name: string;
  gradeId: string;
}

export interface UpdateStudentDto {
  studentId: string;
  name: string;
  gradeId: string;
}
