import React, { useState, useEffect } from "react";
import { Eye, Check, Ban } from "lucide-react";
import type { Session, SessionAttendance } from "./types";
import type { Teacher } from "../teachers/types";
import type { Grade } from "../grades/types";
import type { Student } from "../students/types";
import { sessionService } from "./sessionService";
import { teacherService } from "../teachers/teacherService";
import { gradeService } from "../grades/gradeService";
import { studentService } from "../students/studentService";
import { Button } from "../../shared/components/Button";
import { DataTable } from "../../shared/components/DataTable";
import { Input } from "../../shared/components/FormField";
import { ConfirmationDialog } from "../../shared/components/ConfirmationDialog";

interface AxiosErrorResponse {
  response?: {
    data?: {
      errors?: Array<{ message: string }>;
      message?: string;
    };
  };
  message?: string;
}

export const SessionsScreen: React.FC = () => {
  const [sessions, setSessions] = useState<Session[]>([]);
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [students, setStudents] = useState<Student[]>([]);

  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");
  const [statusFilter, setStatusFilter] = useState<string>("All");

  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);
  const [selectedSession, setSelectedSession] = useState<Session | null>(null);

  const [teacherId, setTeacherId] = useState<string>("");
  const [gradeId, setGradeId] = useState<string>("");
  const [sessionDate, setSessionDate] = useState<string>("");
  const [attendanceForm, setAttendanceForm] = useState<SessionAttendance[]>([]);

  const [confirmCompleteId, setConfirmCompleteId] = useState<string | null>(
    null,
  );
  const [confirmCancelId, setConfirmCancelId] = useState<string | null>(null);

  const extractErrorMessage = (err: unknown): string => {
    if (err && typeof err === "object") {
      const errorObj = err as AxiosErrorResponse;
      if (
        errorObj.response?.data?.errors &&
        Array.isArray(errorObj.response.data.errors) &&
        errorObj.response.data.errors.length > 0
      ) {
        return errorObj.response.data.errors[0].message;
      }
      if (errorObj.response?.data?.message) {
        return errorObj.response.data.message;
      }
      if (errorObj.message) {
        return errorObj.message;
      }
    }
    return "An unexpected error occurred.";
  };

  useEffect(() => {
    let isMounted = true;

    const initData = async () => {
      try {
        setLoading(true);
        setError(null);

        const [sessionsRes, teachersRes, gradesRes, studentsRes] =
          await Promise.allSettled([
            sessionService.getAll(),
            teacherService.getAll(),
            gradeService.getAll(),
            studentService.getAll(),
          ]);

        if (isMounted) {
          setSessions(
            sessionsRes.status === "fulfilled" ? sessionsRes.value || [] : [],
          );
          setTeachers(
            teachersRes.status === "fulfilled" ? teachersRes.value || [] : [],
          );
          setGrades(
            gradesRes.status === "fulfilled" ? gradesRes.value || [] : [],
          );
          setStudents(
            studentsRes.status === "fulfilled" ? studentsRes.value || [] : [],
          );
        }
      } catch (err: unknown) {
        if (isMounted) {
          setError(extractErrorMessage(err));
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    initData();
    return () => {
      isMounted = false;
    };
  }, []);

  const refreshSessions = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await sessionService.getAll();
      setSessions(data || []);
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleGradeChange = (targetGradeId: string) => {
    setGradeId(targetGradeId);
    const filteredStudents = students.filter(
      (s) => s.gradeId === targetGradeId,
    );
    const initialAttendance = filteredStudents.map((s) => ({
      studentId: s.studentId,
      studentName: s.name,
      isPresent: true,
      notes: "",
    }));
    setAttendanceForm(initialAttendance);
  };

  const handleAttendanceChange = (
    index: number,
    field: keyof SessionAttendance,
    value: boolean | string | null,
  ) => {
    const updated = [...attendanceForm];
    updated[index] = { ...updated[index], [field]: value };
    setAttendanceForm(updated);
  };

  const handleOpenCreate = () => {
    setTeacherId(teachers.length > 0 ? teachers[0].teacherId : "");
    setSessionDate(new Date().toISOString().substring(0, 16));
    setIsFormOpen(true);
    if (grades.length > 0) {
      handleGradeChange(grades[0].gradeId);
    }
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setTeacherId("");
    setGradeId("");
    setSessionDate("");
    setAttendanceForm([]);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!teacherId || !gradeId || !sessionDate) {
      setError("Please fill in all required fields.");
      return;
    }

    try {
      setSubmitting(true);
      setError(null);

      const payload = {
        teacherId,
        gradeId,
        sessionDate: new Date(sessionDate).toISOString(),
        attendances: attendanceForm.map((a) => ({
          studentId: a.studentId,
          isPresent: a.isPresent,
          notes: a.notes || null,
        })),
      };

      await sessionService.create(payload);
      await refreshSessions();
      handleCloseForm();
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleCompleteConfirm = async () => {
    if (!confirmCompleteId) return;
    try {
      setLoading(true);
      setError(null);
      await sessionService.complete(confirmCompleteId);
      await refreshSessions();
      setConfirmCompleteId(null);
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleCancelConfirm = async () => {
    if (!confirmCancelId) return;
    try {
      setLoading(true);
      setError(null);
      await sessionService.cancel(confirmCancelId);
      await refreshSessions();
      setConfirmCancelId(null);
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const sortedSessions = [...sessions].sort(
    (a, b) =>
      new Date(b.sessionDate).getTime() - new Date(a.sessionDate).getTime(),
  );

  const filteredSessions = sortedSessions.filter((s) => {
    const teacherName =
      s.teacherName ||
      teachers.find((t) => t.teacherId === s.teacherId)?.name ||
      "";
    const gradeName =
      s.gradeName || grades.find((g) => g.gradeId === s.gradeId)?.name || "";

    const matchesSearch =
      teacherName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      gradeName.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus = statusFilter === "All" || s.status === statusFilter;

    return matchesSearch && matchesStatus;
  });

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Sessions & Attendance
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Manage student attendance logs and session statuses
          </p>
        </div>

        <div className="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
          <div className="w-full sm:w-64">
            <Input
              type="text"
              value={searchTerm}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                setSearchTerm(e.target.value)
              }
              placeholder="Search by teacher or grade..."
            />
          </div>
          <Button
            onClick={handleOpenCreate}
            variant="primary"
            disabled={teachers.length === 0 || grades.length === 0}
            className="whitespace-nowrap"
          >
            Record Session
          </Button>
        </div>
      </div>

      <div className="flex border-b border-gray-200">
        {["All", "Scheduled", "Completed", "Cancelled"].map((tab) => (
          <button
            key={tab}
            onClick={() => setStatusFilter(tab)}
            className={`px-4 py-2.5 text-sm font-semibold border-b-2 -mb-px transition-colors ${
              statusFilter === tab
                ? "border-indigo-600 text-indigo-600"
                : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300"
            }`}
          >
            {tab}
          </button>
        ))}
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm flex justify-between items-center">
          <span>{error}</span>
          <button onClick={() => setError(null)} className="font-bold">
            &times;
          </button>
        </div>
      )}

      {loading && !sessions.length ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-gray-100 overflow-hidden">
          <DataTable
            data={filteredSessions}
            columns={[
              {
                header: "Date & Time",
                accessor: (row) => {
                  const d = new Date(row.sessionDate);
                  const dateStr = d.toLocaleDateString(undefined, {
                    month: "short",
                    day: "numeric",
                  });
                  const timeStr = d.toLocaleTimeString(undefined, {
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: true,
                  });
                  return `${dateStr}, ${timeStr}`;
                },
                className: "font-medium text-gray-900 whitespace-nowrap",
              },
              {
                header: "Teacher",
                accessor: (row) =>
                  row.teacherName ||
                  teachers.find((t) => t.teacherId === row.teacherId)?.name ||
                  "Unknown",
                className: "text-gray-700",
              },
              {
                header: "Grade",
                accessor: (row) =>
                  row.gradeName ||
                  grades.find((g) => g.gradeId === row.gradeId)?.name ||
                  "Unknown",
                className: "text-gray-600",
              },
              {
                header: "Unit Price",
                accessor: (row) => `$${row.unitPrice.toFixed(2)}`,
                className: "font-semibold text-gray-900",
              },
              {
                header: "Status",
                accessor: (row) => (
                  <span
                    className={`px-2.5 py-1 rounded-full text-xs font-semibold ${
                      row.status === "Completed"
                        ? "bg-green-100 text-green-800"
                        : row.status === "Cancelled"
                          ? "bg-red-100 text-red-800"
                          : "bg-blue-100 text-blue-800"
                    }`}
                  >
                    {row.status}
                  </span>
                ),
              },
              {
                header: "Attendance",
                accessor: (row) => {
                  const presentCount = row.attendances.filter(
                    (a) => a.isPresent,
                  ).length;
                  return `${presentCount} / ${row.attendances.length} Present`;
                },
                className: "text-sm text-gray-500",
              },
              {
                header: "Actions",
                accessor: (row) => (
                  <div className="flex gap-1.5 justify-end">
                    <button
                      onClick={() => setSelectedSession(row)}
                      className="flex items-center justify-center w-8 h-8 rounded-lg border border-slate-200 bg-white text-slate-600 hover:bg-slate-50 transition-colors"
                      title="View Details"
                    >
                      <Eye className="w-4 h-4" />
                    </button>
                    {row.status === "Scheduled" && (
                      <>
                        <button
                          disabled={row.attendances.length === 0}
                          onClick={() => setConfirmCompleteId(row.id)}
                          className="flex items-center justify-center w-8 h-8 rounded-lg bg-indigo-600 text-white hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                          title="Complete Session"
                        >
                          <Check className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => setConfirmCancelId(row.id)}
                          className="flex items-center justify-center w-8 h-8 rounded-lg bg-rose-50 text-rose-600 hover:bg-rose-100 transition-colors"
                          title="Cancel Session"
                        >
                          <Ban className="w-4 h-4" />
                        </button>
                      </>
                    )}
                  </div>
                ),
                className: "text-right min-w-[120px]",
              },
            ]}
          />
        </div>
      )}

      {isFormOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg max-w-2xl w-full overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="text-lg font-semibold text-gray-900">
                Record New Session
              </h3>
              <button
                onClick={handleCloseForm}
                className="text-gray-400 hover:text-gray-600 text-xl"
              >
                &times;
              </button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="space-y-1">
                  <label className="block text-sm font-medium text-gray-700">
                    Teacher
                  </label>
                  <select
                    value={teacherId}
                    onChange={(e) => setTeacherId(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm text-sm"
                    required
                  >
                    {teachers.map((t) => (
                      <option key={t.teacherId} value={t.teacherId}>
                        {t.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="space-y-1">
                  <label className="block text-sm font-medium text-gray-700">
                    Grade Level
                  </label>
                  <select
                    value={gradeId}
                    onChange={(e) => handleGradeChange(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm text-sm"
                    required
                  >
                    {grades.map((g) => (
                      <option key={g.gradeId} value={g.gradeId}>
                        {g.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="space-y-1">
                  <label className="block text-sm font-medium text-gray-700">
                    Session Date
                  </label>
                  <input
                    type="datetime-local"
                    value={sessionDate}
                    onChange={(e) => setSessionDate(e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm text-sm"
                    required
                  />
                </div>
              </div>

              <div className="border-t border-gray-100 pt-4">
                <h4 className="text-sm font-semibold text-gray-900 mb-3">
                  Student Attendance Records
                </h4>
                {attendanceForm.length === 0 ? (
                  <p className="text-sm text-gray-500 text-center py-4">
                    No students enrolled in this grade level.
                  </p>
                ) : (
                  <div className="max-h-60 overflow-y-auto border border-gray-100 rounded-lg divide-y divide-gray-100">
                    {attendanceForm.map((item, index) => (
                      <div
                        key={item.studentId}
                        className="p-3 flex flex-col sm:flex-row sm:items-center justify-between gap-3 bg-white"
                      >
                        <span className="text-sm font-medium text-gray-800">
                          {item.studentName}
                        </span>
                        <div className="flex items-center gap-4">
                          <label className="inline-flex items-center gap-1.5 text-sm cursor-pointer">
                            <input
                              type="checkbox"
                              checked={item.isPresent}
                              onChange={(e) =>
                                handleAttendanceChange(
                                  index,
                                  "isPresent",
                                  e.target.checked,
                                )
                              }
                              className="rounded border-gray-300 text-indigo-600 focus:ring-indigo-500"
                            />
                            Present
                          </label>
                          <input
                            type="text"
                            placeholder="Add notes..."
                            value={item.notes || ""}
                            onChange={(e) =>
                              handleAttendanceChange(
                                index,
                                "notes",
                                e.target.value,
                              )
                            }
                            className="px-2.5 py-1 border border-gray-200 rounded text-xs focus:ring-1 focus:ring-indigo-500"
                          />
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div className="flex justify-end gap-3 pt-4 border-t border-gray-100">
                <Button
                  type="button"
                  variant="secondary"
                  onClick={handleCloseForm}
                  disabled={submitting}
                >
                  Cancel
                </Button>
                <Button
                  type="submit"
                  variant="primary"
                  disabled={submitting || attendanceForm.length === 0}
                >
                  {submitting ? "Saving..." : "Record Session"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {selectedSession && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg max-w-lg w-full overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">
                  Session Details
                </h3>
                <p className="text-xs text-gray-500 mt-0.5">
                  Held on{" "}
                  {new Date(selectedSession.sessionDate).toLocaleString()}
                </p>
              </div>
              <button
                onClick={() => setSelectedSession(null)}
                className="text-gray-400 hover:text-gray-600 text-xl"
              >
                &times;
              </button>
            </div>
            <div className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4 text-sm border-b border-gray-100 pb-4">
                <div>
                  <span className="text-gray-500 block">Teacher</span>
                  <span className="font-semibold text-gray-900">
                    {selectedSession.teacherName ||
                      teachers.find(
                        (t) => t.teacherId === selectedSession.teacherId,
                      )?.name ||
                      "Unknown"}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500 block">Grade Level</span>
                  <span className="font-semibold text-gray-900">
                    {selectedSession.gradeName ||
                      grades.find((g) => g.gradeId === selectedSession.gradeId)
                        ?.name ||
                      "Unknown"}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500 block">Unit Price</span>
                  <span className="font-semibold text-gray-900">
                    ${selectedSession.unitPrice.toFixed(2)}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500 block">Status</span>
                  <span
                    className={`inline-block px-2 py-0.5 mt-0.5 rounded-full text-xs font-semibold ${
                      selectedSession.status === "Completed"
                        ? "bg-green-100 text-green-800"
                        : selectedSession.status === "Cancelled"
                          ? "bg-red-100 text-red-800"
                          : "bg-blue-100 text-blue-800"
                    }`}
                  >
                    {selectedSession.status}
                  </span>
                </div>
              </div>

              <div>
                <h4 className="text-sm font-semibold text-gray-900 mb-3">
                  Attendance Summary
                </h4>
                <div className="max-h-60 overflow-y-auto border border-gray-100 rounded-lg divide-y divide-gray-100">
                  {selectedSession.attendances.map((item) => {
                    const studentName =
                      item.studentName ||
                      students.find((s) => s.studentId === item.studentId)
                        ?.name ||
                      "Unknown Student";
                    return (
                      <div
                        key={item.id || item.studentId}
                        className="p-3 flex items-center justify-between gap-3 bg-white"
                      >
                        <div>
                          <span className="text-sm font-medium text-gray-800 block">
                            {studentName}
                          </span>
                          {item.notes && (
                            <span className="text-xs text-gray-400">
                              {item.notes}
                            </span>
                          )}
                        </div>
                        <span
                          className={`px-2 py-0.5 rounded text-xs font-medium ${
                            item.isPresent
                              ? "bg-green-50 text-green-700 border border-green-200"
                              : "bg-red-50 text-red-700 border border-red-200"
                          }`}
                        >
                          {item.isPresent ? "Present" : "Absent"}
                        </span>
                      </div>
                    );
                  })}
                </div>
              </div>

              <div className="flex justify-end pt-2">
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => setSelectedSession(null)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {confirmCompleteId && (
        <ConfirmationDialog
          isOpen={!!confirmCompleteId}
          title="Complete Session"
          message="Are you sure you want to mark this session as completed? This action is irreversible and locks accounting records."
          onConfirm={handleCompleteConfirm}
          onCancel={() => setConfirmCompleteId(null)}
        />
      )}

      {confirmCancelId && (
        <ConfirmationDialog
          isOpen={!!confirmCancelId}
          title="Cancel Session"
          message="Are you sure you want to cancel this session? This action cannot be undone."
          onConfirm={handleCancelConfirm}
          onCancel={() => setConfirmCancelId(null)}
        />
      )}
    </div>
  );
};
