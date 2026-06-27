import React, { useState, useEffect } from "react";
import type { Teacher } from "../teachers/types";
import type { Grade } from "../grades/types";
import type { TeacherPricing } from "./types";
import { teacherService } from "../teachers/teacherService";
import { gradeService } from "../grades/gradeService";
import { teacherPricingService } from "./teacherPricingService";
import { Button } from "../../shared/components/Button";
import { DataTable } from "../../shared/components/DataTable";
import { Input } from "../../shared/components/FormField";
import { ConfirmationDialog } from "../../shared/components/ConfirmationDialog";

// تعريف هيكل الخطأ الخاص باستجابة Axios
interface AxiosErrorResponse {
  response?: {
    status?: number;
    data?: {
      errors?: Array<{ message: string }>;
      message?: string;
    };
  };
  message?: string;
}

export const TeacherPricingScreen: React.FC = () => {
  const [pricings, setPricings] = useState<TeacherPricing[]>([]);
  const [teachers, setTeachers] = useState<Teacher[]>([]);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);
  const [editingPricing, setEditingPricing] = useState<TeacherPricing | null>(
    null,
  );
  const [deletingPricingId, setDeletingPricingId] = useState<string | null>(
    null,
  );
  const [submitting, setSubmitting] = useState<boolean>(false);
  const [searchTerm, setSearchTerm] = useState<string>("");

  const [teacherId, setTeacherId] = useState<string>("");
  const [gradeId, setGradeId] = useState<string>("");
  const [sessionPrice, setSessionPrice] = useState<string>("");

  // دالة لاستخراج رسالة الخطأ الفعلية من السيرفر بشكل آمن ومتوافق مع ESLint
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

        // جلب البيانات الأساسية بالتوازي
        const [pricingsResult, teachersResult, gradesResult] =
          await Promise.allSettled([
            teacherPricingService.getAll(),
            teacherService.getAll(),
            gradeService.getAll(),
          ]);

        if (isMounted) {
          if (pricingsResult.status === "fulfilled") {
            setPricings(pricingsResult.value || []);
          } else {
            const reasonStr = String(
              pricingsResult.reason?.message || pricingsResult.reason,
            );
            // إذا كانت البيانات فارغة على السيرفر، نتعامل معها كمصفوفة فارغة بشكل طبيعي
            if (reasonStr.includes("404") || reasonStr.includes("not found")) {
              setPricings([]);
            } else {
              setError(extractErrorMessage(pricingsResult.reason));
            }
          }

          if (teachersResult.status === "fulfilled") {
            setTeachers(teachersResult.value || []);
          } else {
            setError(extractErrorMessage(teachersResult.reason));
          }

          if (gradesResult.status === "fulfilled") {
            setGrades(gradesResult.value || []);
          } else {
            setError(extractErrorMessage(gradesResult.reason));
          }
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

  // دالة تحديث قائمة الأسعار بعد العمليات الناجحة
  const refreshPricings = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await teacherPricingService.getAll();
      setPricings(data || []);
    } catch (err: unknown) {
      const reasonStr = err instanceof Error ? err.message : String(err);
      if (reasonStr.includes("404") || reasonStr.includes("not found")) {
        setPricings([]);
      } else {
        setError(extractErrorMessage(err));
      }
    } finally {
      setLoading(false);
    }
  };

  const handleOpenCreate = () => {
    setEditingPricing(null);
    setTeacherId(teachers.length > 0 ? teachers[0].teacherId : "");
    setGradeId(grades.length > 0 ? grades[0].gradeId : "");
    setSessionPrice("");
    setIsFormOpen(true);
  };

  const handleOpenEdit = (pricing: TeacherPricing) => {
    setEditingPricing(pricing);
    setTeacherId(pricing.teacherId);
    setGradeId(pricing.gradeId);
    setSessionPrice(pricing.sessionPrice.toString());
    setIsFormOpen(true);
  };

  const handleCloseForm = () => {
    setIsFormOpen(false);
    setTeacherId("");
    setGradeId("");
    setSessionPrice("");
    setEditingPricing(null);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const priceNum = parseFloat(sessionPrice);

    // التحقق الوقائي من صحة السعر وإيجابيته
    if (isNaN(priceNum) || priceNum <= 0) {
      setError("Session price must be a positive number greater than zero.");
      return;
    }

    try {
      setSubmitting(true);
      setError(null);

      if (editingPricing) {
        // تحديث السعر الحالي للمدرس
        await teacherPricingService.update(editingPricing.id, {
          id: editingPricing.id,
          newPrice: priceNum,
        });
      } else {
        if (!teacherId || !gradeId) {
          setError("Please select both a teacher and a grade level.");
          setSubmitting(false);
          return;
        }

        // منع تكرار نفس ثنائية المدرس والمرحلة في الواجهة
        const isDuplicate = pricings.some(
          (p) => p.teacherId === teacherId && p.gradeId === gradeId,
        );
        if (isDuplicate) {
          setError("Pricing for this teacher and grade already exists.");
          setSubmitting(false);
          return;
        }

        // إنشاء سجل تسعيرة جديدة
        await teacherPricingService.create({
          teacherId,
          gradeId,
          sessionPrice: priceNum,
        });
      }

      await refreshPricings();
      handleCloseForm();
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!deletingPricingId) return;

    try {
      setLoading(true);
      setError(null);
      await teacherPricingService.delete(deletingPricingId);
      await refreshPricings();
      setDeletingPricingId(null);
    } catch (err: unknown) {
      setError(extractErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  // فلترة قائمة الأسعار بناءً على اسم المدرس أو المرحلة الدراسية
  const filteredPricings = pricings.filter(
    (p) =>
      p.teacherName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      p.gradeName.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
    <div className="p-6 max-w-6xl mx-auto space-y-6">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            Teacher Pricing Management
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Configure and manage session rates for teachers per grade level
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
            Add New Pricing
          </Button>
        </div>
      </div>

      {(teachers.length === 0 || grades.length === 0) && !loading && (
        <div className="bg-yellow-50 border border-yellow-200 text-yellow-800 px-4 py-3 rounded-lg text-sm">
          Warning: You must have at least one Teacher and one Grade created
          before configuring session prices.
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

      {loading && !pricings.length ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-gray-100 overflow-hidden">
          <DataTable
            data={filteredPricings}
            columns={[
              {
                header: "Teacher Name",
                accessor: (row) => row.teacherName,
                className: "font-medium text-gray-900",
              },
              {
                header: "Grade Level",
                accessor: (row) => row.gradeName,
                className: "text-gray-600",
              },
              {
                header: "Session Price",
                accessor: (row) => `$${row.sessionPrice.toFixed(2)}`,
                className: "font-semibold text-gray-900",
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
                      onClick={() => setDeletingPricingId(row.id)}
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
                {editingPricing
                  ? "Edit Session Price"
                  : "Add New Session Price"}
              </h3>
              <button
                onClick={handleCloseForm}
                className="text-gray-400 hover:text-gray-600 text-xl"
              >
                &times;
              </button>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div className="space-y-1">
                <label className="block text-sm font-medium text-gray-700">
                  Select Teacher
                </label>
                <select
                  value={teacherId}
                  onChange={(e: React.ChangeEvent<HTMLSelectElement>) =>
                    setTeacherId(e.target.value)
                  }
                  disabled={!!editingPricing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 text-sm disabled:bg-gray-100 disabled:text-gray-500"
                  required
                >
                  {teachers.map((teacher) => (
                    <option key={teacher.teacherId} value={teacher.teacherId}>
                      {teacher.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="space-y-1">
                <label className="block text-sm font-medium text-gray-700">
                  Select Grade Level
                </label>
                <select
                  value={gradeId}
                  onChange={(e: React.ChangeEvent<HTMLSelectElement>) =>
                    setGradeId(e.target.value)
                  }
                  disabled={!!editingPricing}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500 text-sm disabled:bg-gray-100 disabled:text-gray-500"
                  required
                >
                  {grades.map((grade) => (
                    <option key={grade.gradeId} value={grade.gradeId}>
                      {grade.name}
                    </option>
                  ))}
                </select>
              </div>

              <Input
                label="Session Price"
                type="number"
                step="0.01"
                min="0.01"
                value={sessionPrice}
                onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                  setSessionPrice(e.target.value)
                }
                placeholder="e.g. 150.00"
                required
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

      {deletingPricingId && (
        <ConfirmationDialog
          isOpen={!!deletingPricingId}
          title="Delete Pricing Record"
          message="Are you sure you want to delete this session pricing setup? This action cannot be undone."
          onConfirm={handleDeleteConfirm}
          onCancel={() => setDeletingPricingId(null)}
        />
      )}
    </div>
  );
};
