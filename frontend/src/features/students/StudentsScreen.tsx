import React, { useState, useEffect } from "react";
import type { Student } from "./types";
import type { Grade } from "../grades/types";
import { studentService } from "./studentService";
import { gradeService } from "../grades/gradeService";
import { Button } from "../../shared/components/Button";
import { DataTable } from "../../shared/components/DataTable";
import { Input } from "../../shared/components/FormField";
import { ConfirmationDialog } from "../../shared/components/ConfirmationDialog";

export const StudentsScreen: React.FC = () => {
  const [students, setStudents] = useState<Student[]>([]);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [gradeId, setGradeId] = useState<string>("");
  const [editingStudent, setEditingStudent] = useState<Student | null>(null);
  const [deletingStudentId, setDeletingStudentId] = useState<string | null>(
    null,
  );
  const [submitting, setSubmitting] = useState<boolean>(false);

  useEffect(() => {
    let isMounted = true;

    const initData = async () => {
      try {
        setLoading(true);
        setError(null);
        const [studentsData, gradesData] = await Promise.all([
          studentService.getAll(),
          gradeService.getAll(),
        ]);
        if (isMounted) {
          setStudents(studentsData);
          setGrades(gradesData);
        }
      } catch (err: unknown) {
        if (isMounted) {
          setError(err instanceof Error ? err.message : "Failed to load data.");
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

  const refreshStudents = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await studentService.getAll();
      setStudents(data);
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Failed to fetch students.",
      );
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingStudent(null);
    setName("");
    setGradeId(grades.length > 0 ? grades[0].gradeId : "");
    setIsFormOpen(true);
  };

  const handleOpenEdit = (student: Student) => {
    setEditingStudent(student);
    setName(student.name);
    setGradeId(student.gradeId);
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setName("");
    setGradeId("");
    setEditingStudent(null);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!name.trim() || !gradeId) return;

    try {
      setSubmitting(true);
      setError(null);
      if (editingStudent) {
        await studentService.update(editingStudent.studentId, {
          studentId: editingStudent.studentId,
          name: name.trim(),
          gradeId,
        });
      } else {
        await studentService.create({
          name: name.trim(),
          gradeId,
        });
      }
      await refreshStudents();
      handleCloseForm();
    } catch (err: unknown) {
      setError(
        err instanceof Error
          ? err.message
          : "An error occurred while saving the student.",
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deletingStudentId) return;

    try {
      setLoading(true);
      setError(null);
      await studentService.delete(deletingStudentId);
      await refreshStudents();
      setDeletingStudentId(null);
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Failed to delete student.",
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Students Management
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Configure and manage the students and their grade assignments
          </p>
        </div>
        <Button
          onClick={handleOpenCreate}
          variant="primary"
          disabled={grades.length === 0}
        >
          Add Student
        </Button>
      </div>

      {grades.length === 0 && !loading && (
        <div className="bg-yellow-50 border border-yellow-200 text-yellow-800 px-4 py-3 rounded-lg text-sm">
          Warning: You must create at least one Grade before you can manage or
          add students.
        </div>
      )}

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm flex justify-between items-center">
          <span>{error}</span>
          <button onClick={() => setError(null)} className="font-bold">
            &times;
          </button>
        </div>
      )}

      {loading && !students.length ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-gray-100 overflow-hidden">
          <DataTable
            data={students}
            columns={[
              {
                header: "Student Name",
                accessor: (row) => row.name,
                className: "font-medium text-gray-900",
              },
              {
                header: "Assigned Grade",
                accessor: (row) =>
                  grades.find((g) => g.gradeId === row.gradeId)?.name ||
                  row.gradeName ||
                  "Unassigned",
                className: "text-gray-600",
              },
              {
                header: "Actions",
                accessor: (row) => (
                  <div className="flex gap-2 justify-end">
                    <Button
                      variant="secondary"
                      size="sm"
                      onClick={() => handleOpenEdit(row)}
                    >
                      Edit
                    </Button>
                    <Button
                      variant="danger"
                      size="sm"
                      onClick={() => setDeletingStudentId(row.studentId)}
                    >
                      Delete
                    </Button>
                  </div>
                ),
                className: "text-right",
              },
            ]}
          />
        </div>
      )}

      {isFormOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg max-w-md w-full overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center">
              <h3 className="text-lg font-semibold text-gray-900">
                {editingStudent ? "Edit Student" : "Add New Student"}
              </h3>
              <button
                onClick={handleCloseForm}
                className="text-gray-400 hover:text-gray-600 text-xl"
              >
                &times;
              </button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <Input
                label="Student Name"
                type="text"
                value={name}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setName(e.target.value)
                }
                placeholder="e.g. Khaled Al-Ahmad"
                required
              />

              <div className="space-y-1">
                <label className="block text-sm font-medium text-gray-700">
                  Assign Grade
                </label>
                <select
                  value={gradeId}
                  onChange={(e: React.ChangeEvent<HTMLSelectElement>) =>
                    setGradeId(e.target.value)
                  }
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 text-sm"
                  required
                >
                  <option value="" disabled>
                    Select a Grade
                  </option>
                  {grades.map((grade) => (
                    <option key={grade.gradeId} value={grade.gradeId}>
                      {grade.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex justify-end gap-3 pt-2">
                <Button
                  type="button"
                  variant="secondary"
                  onClick={handleCloseForm}
                  disabled={submitting}
                >
                  Cancel
                </Button>
                <Button type="submit" variant="primary" disabled={submitting}>
                  {submitting ? "Saving..." : "Save"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}

      {deletingStudentId && (
        <ConfirmationDialog
          isOpen={!!deletingStudentId}
          title="Delete Student"
          message="Are you sure you want to delete this student? This action cannot be undone."
          onConfirm={handleDeleteConfirm}
          onCancel={() => setDeletingStudentId(null)}
        />
      )}
    </div>
  );
};
