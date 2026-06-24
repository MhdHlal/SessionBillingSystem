import React, { useState, useEffect } from "react";
import type { Grade } from "./types";
import type { Student } from "../students/types";
import { gradeService } from "./gradeService";
import { studentService } from "../students/studentService";
import { Button } from "../../shared/components/Button";
import { DataTable } from "../../shared/components/DataTable";
import { Input } from "../../shared/components/FormField";
import { ConfirmationDialog } from "../../shared/components/ConfirmationDialog";

export const GradesScreen: React.FC = () => {
  const [grades, setGrades] = useState<Grade[]>([]);
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [editingGrade, setEditingGrade] = useState<Grade | null>(null);
  const [deletingGradeId, setDeletingGradeId] = useState<string | null>(null);
  const [viewingGradeStudents, setViewingGradeStudents] =
    useState<Grade | null>(null);
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");

  useEffect(() => {
    let isMounted = true;

    const initData = async () => {
      try {
        setLoading(true);
        setError(null);
        const [gradesData, studentsData] = await Promise.all([
          gradeService.getAll(),
          studentService.getAll(),
        ]);
        if (isMounted) {
          setGrades(gradesData);
          setStudents(studentsData);
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

  const refreshGrades = async () => {
    try {
      setLoading(true);
      setError(null);
      const [gradesData, studentsData] = await Promise.all([
        gradeService.getAll(),
        studentService.getAll(),
      ]);
      setGrades(gradesData);
      setStudents(studentsData);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Failed to refresh data.");
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingGrade(null);
    setName("");
    setIsFormOpen(true);
  };

  const handleOpenEdit = (grade: Grade) => {
    setEditingGrade(grade);
    setName(grade.name);
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setName("");
    setEditingGrade(null);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!name.trim()) return;

    try {
      setSubmitting(true);
      setError(null);
      if (editingGrade) {
        await gradeService.update(editingGrade.gradeId, {
          gradeId: editingGrade.gradeId,
          name: name.trim(),
        });
      } else {
        await gradeService.create({ name: name.trim() });
      }
      await refreshGrades();
      handleCloseForm();
    } catch (err: unknown) {
      setError(
        err instanceof Error
          ? err.message
          : "An error occurred while saving the grade.",
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deletingGradeId) return;

    try {
      setLoading(true);
      setError(null);
      await gradeService.delete(deletingGradeId);
      await refreshGrades();
      setDeletingGradeId(null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Failed to delete grade.");
    } finally {
      setLoading(false);
    }
  };

  const filteredGrades = grades.filter((grade) =>
    grade.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  const activeGradeStudents = viewingGradeStudents
    ? students.filter((s) => s.gradeId === viewingGradeStudents.gradeId)
    : [];

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Grades Management
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Configure and manage the academic grade levels
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
              placeholder="Search by grade name..."
            />
          </div>
          <Button
            onClick={handleOpenCreate}
            variant="primary"
            className="whitespace-nowrap"
          >
            Add Grade
          </Button>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm flex justify-between items-center">
          <span>{error}</span>
          <button onClick={() => setError(null)} className="font-bold">
            &times;
          </button>
        </div>
      )}

      {loading && !grades.length ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-gray-100 overflow-hidden">
          <DataTable
            data={filteredGrades}
            columns={[
              {
                header: "Grade Name",
                accessor: (row) => {
                  const count = students.filter(
                    (s) => s.gradeId === row.gradeId,
                  ).length;
                  return (
                    <div className="flex items-center gap-2">
                      <button
                        onClick={() => setViewingGradeStudents(row)}
                        className="font-semibold text-indigo-600 hover:text-indigo-900 hover:underline text-left focus:outline-none"
                      >
                        {row.name}
                      </button>
                      <span className="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded-full">
                        {count} {count === 1 ? "Student" : "Students"}
                      </span>
                    </div>
                  );
                },
                className: "font-medium text-gray-900",
              },
              {
                header: "Actions",
                accessor: (row) => {
                  const hasStudents = students.some(
                    (s) => s.gradeId === row.gradeId,
                  );
                  return (
                    <div className="flex gap-2 justify-end">
                      <Button
                        variant="secondary"
                        size="sm"
                        onClick={() => handleOpenEdit(row)}
                      >
                        Edit
                      </Button>
                      <div
                        title={
                          hasStudents
                            ? "Cannot delete grade with assigned students"
                            : ""
                        }
                      >
                        <Button
                          variant="danger"
                          size="sm"
                          disabled={hasStudents}
                          onClick={() => setDeletingGradeId(row.gradeId)}
                        >
                          Delete
                        </Button>
                      </div>
                    </div>
                  );
                },
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
                {editingGrade ? "Edit Grade" : "Add New Grade"}
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
                label="Grade Name"
                type="text"
                value={name}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setName(e.target.value)
                }
                placeholder="e.g. Grade 7"
                required
                autoFocus
              />
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

      {viewingGradeStudents && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-xl shadow-lg max-w-md w-full overflow-hidden">
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">
                  Assigned Students
                </h3>
                <p className="text-xs text-gray-500 mt-0.5">
                  List of students enrolled in {viewingGradeStudents.name}
                </p>
              </div>
              <button
                onClick={() => setViewingGradeStudents(null)}
                className="text-gray-400 hover:text-gray-600 text-xl"
              >
                &times;
              </button>
            </div>
            <div className="p-6 max-h-96 overflow-y-auto">
              {activeGradeStudents.length === 0 ? (
                <div className="text-center py-6 text-gray-500 text-sm">
                  No students are currently assigned to this grade level.
                </div>
              ) : (
                <ul className="divide-y divide-gray-100 border border-gray-100 rounded-lg overflow-hidden">
                  {activeGradeStudents.map((student) => (
                    <li
                      key={student.studentId}
                      className="px-4 py-3 text-sm text-gray-800 hover:bg-gray-50 transition-colors"
                    >
                      {student.name}
                    </li>
                  ))}
                </ul>
              )}
              <div className="flex justify-end mt-6">
                <Button
                  type="button"
                  variant="secondary"
                  onClick={() => setViewingGradeStudents(null)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}

      {deletingGradeId && (
        <ConfirmationDialog
          isOpen={!!deletingGradeId}
          title="Delete Grade"
          message="Are you sure you want to delete this grade? This action cannot be undone."
          onConfirm={handleDeleteConfirm}
          onCancel={() => setDeletingGradeId(null)}
        />
      )}
    </div>
  );
};
