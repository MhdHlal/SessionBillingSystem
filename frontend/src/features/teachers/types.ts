export interface Teacher {
  teacherId: string;
  name: string;
}

export interface CreateTeacherDto {
  name: string;
}

export interface UpdateTeacherDto {
  teacherId: string;
  name: string;
}
