export interface SessionAttendance {
  id?: string;
  studentId: string;
  studentName?: string;
  isPresent: boolean;
  notes?: string | null;
}

export type SessionStatus = "Scheduled" | "Completed" | "Cancelled";

export interface Session {
  id: string;
  teacherId: string;
  teacherName: string;
  gradeId: string;
  gradeName: string;
  sessionDate: string;
  unitPrice: number;
  status: SessionStatus;
  attendances: SessionAttendance[];
}

export interface CreateSessionDto {
  teacherId: string;
  gradeId: string;
  sessionDate: string;
  attendances: SessionAttendance[];
}
