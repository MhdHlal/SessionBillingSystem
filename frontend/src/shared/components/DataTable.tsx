import type { ReactNode } from "react";

export interface Column<T> {
  header: string;
  accessor: (item: T) => ReactNode;
  className?: string;
}

export interface DataTableProps<T> {
  data: T[];
  columns: Column<T>[];
  isLoading?: boolean;
  emptyMessage?: string;
}

export function DataTable<T>({
  data,
  columns,
  isLoading = false,
  emptyMessage = "No records found",
}: DataTableProps<T>) {
  if (isLoading) {
    return (
      <div className="w-full bg-white rounded-2xl border border-slate-100 overflow-hidden shadow-sm">
        <div className="p-6 space-y-4">
          <div className="h-6 bg-slate-100 rounded-lg w-1/4 animate-pulse" />
          <div className="space-y-3">
            <div className="h-10 bg-slate-50 rounded-xl w-full animate-pulse" />
            <div className="h-10 bg-slate-50 rounded-xl w-full animate-pulse" />
            <div className="h-10 bg-slate-50 rounded-xl w-full animate-pulse" />
          </div>
        </div>
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <div className="w-full bg-white rounded-2xl border border-slate-100 p-12 text-center shadow-sm">
        <div className="inline-flex items-center justify-center w-12 h-12 rounded-xl bg-slate-50 text-slate-400 mb-4">
          <svg
            className="w-6 h-6"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0a2 2 0 01-2 2H6a2 2 0 01-2-2m16 0V9a2 2 0 00-2-2H6a2 2 0 00-2 2v4.5m16 0h-3.5"
            />
          </svg>
        </div>
        <p className="text-sm font-semibold text-slate-900">{emptyMessage}</p>
        <p className="text-xs text-slate-400 mt-1">
          Please try adding a new record to get started.
        </p>
      </div>
    );
  }

  return (
    <div className="w-full bg-white rounded-2xl border border-slate-100 overflow-hidden shadow-sm">
      <div className="overflow-x-auto">
        <table className="w-full border-collapse text-left text-sm text-slate-600">
          <thead>
            <tr className="border-b border-slate-100 bg-slate-50/75 text-xs font-bold text-slate-400 uppercase tracking-wider">
              {columns.map((col, idx) => (
                <th key={idx} className={`px-6 py-4 ${col.className || ""}`}>
                  {col.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100">
            {data.map((item, rowIdx) => (
              <tr
                key={rowIdx}
                className="hover:bg-slate-50/50 transition-colors"
              >
                {columns.map((col, colIdx) => (
                  <td
                    key={colIdx}
                    className={`px-6 py-4 whitespace-nowrap font-medium text-slate-700 ${col.className || ""}`}
                  >
                    {col.accessor(item)}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
