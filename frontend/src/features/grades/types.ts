export interface Grade {
  gradeId: string;
  name: string;
}

export interface CreateGradeDto {
  name: string;
}

export interface UpdateGradeDto {
  gradeId: string;
  name: string;
}
