import React, { useState, useEffect } from "react";
import type { Teacher } from "./types";
import { teacherService } from "./teacherService";
import { Button } from "../../shared/components/Button";
import { DataTable } from "../../shared/components/DataTable";
import { Input } from "../../shared/components/FormField";
import { ConfirmationDialog } from "../../shared/components/ConfirmationDialog";

export const TeachersScreen: React.FC = () => {
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [editingTeacher, setEditingTeacher] = useState<Teacher | null>(null);
  const [deletingTeacherId, setDeletingTeacherId] = useState<string | null>(
    null,
  );
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");

  useEffect(() => {
    let isMounted = true;

    const fetchTeachers = async () => {
      try {
        setLoading(true);
        setError(null);
        const data = await teacherService.getAll();
        if (isMounted) {
          setTeachers(data);
        }
      } catch (err: unknown) {
        if (isMounted) {
          setError(
            err instanceof Error ? err.message : "Failed to fetch teachers.",
          );
        }
      } finally {
        if (isMounted) {
          setLoading(false);
        }
      }
    };

    fetchTeachers();

    return () => {
      isMounted = false;
    };
  }, []);

  const refreshTeachers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await teacherService.getAll();
      setTeachers(data);
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Failed to fetch teachers.",
      );
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingTeacher(null);
    setName("");
    setIsFormOpen(true);
  };

  const handleOpenEdit = (teacher: Teacher) => {
    setEditingTeacher(teacher);
    setName(teacher.name);
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setName("");
    setEditingTeacher(null);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!name.trim()) return;

    try {
      setSubmitting(true);
      setError(null);
      if (editingTeacher) {
        await teacherService.update(editingTeacher.teacherId, {
          teacherId: editingTeacher.teacherId,
          name: name.trim(),
        });
      } else {
        await teacherService.create({ name: name.trim() });
      }
      await refreshTeachers();
      handleCloseForm();
    } catch (err: unknown) {
      setError(
        err instanceof Error
          ? err.message
          : "An error occurred while saving the teacher.",
      );
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deletingTeacherId) return;

    try {
      setLoading(true);
      setError(null);
      await teacherService.delete(deletingTeacherId);
      await refreshTeachers();
      setDeletingTeacherId(null);
    } catch (err: unknown) {
      setError(
        err instanceof Error ? err.message : "Failed to delete teacher.",
      );
    } finally {
      setLoading(false);
    }
  };

  const filteredTeachers = teachers.filter((teacher) =>
    teacher.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Teachers Management
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Configure and manage the teachers of the educational center
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
              placeholder="Search by teacher name..."
            />
          </div>
          <Button
            onClick={handleOpenCreate}
            variant="primary"
            className="whitespace-nowrap"
          >
            Add Teacher
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

      {loading && !teachers.length ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-gray-100 overflow-hidden">
          <DataTable
            data={filteredTeachers}
            columns={[
              {
                header: "Teacher Name",
                accessor: (row) => row.name,
                className: "font-medium text-gray-900",
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
                      onClick={() => setDeletingTeacherId(row.teacherId)}
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
                {editingTeacher ? "Edit Teacher" : "Add New Teacher"}
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
                label="Teacher Name"
                type="text"
                value={name}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setName(e.target.value)
                }
                placeholder="e.g. Ahmad Mahmoud"
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

      {deletingTeacherId && (
        <ConfirmationDialog
          isOpen={!!deletingTeacherId}
          title="Delete Teacher"
          message="Are you sure you want to delete this teacher? This action cannot be undone."
          onConfirm={handleDeleteConfirm}
          onCancel={() => setDeletingTeacherId(null)}
        />
      )}
    </div>
  );
};
