import type { InputHTMLAttributes, SelectHTMLAttributes } from "react";

interface BaseProps {
  label?: string;
  error?: string;
}

export interface InputProps
  extends InputHTMLAttributes<HTMLInputElement>, BaseProps {}

export function Input({
  label,
  error,
  className = "",
  id,
  ...props
}: InputProps) {
  const inputId = id || props.name;
  return (
    <div className="w-full space-y-1.5">
      {label && (
        <label
          htmlFor={inputId}
          className="block text-xs font-semibold text-slate-500 uppercase tracking-wider"
        >
          {label}
        </label>
      )}
      <input
        id={inputId}
        className={`w-full px-4 py-2.5 rounded-xl border bg-white text-sm text-slate-800 placeholder-slate-400 transition-all focus:outline-none focus:ring-2 focus:border-indigo-600 disabled:bg-slate-50 disabled:text-slate-400 ${
          error
            ? "border-rose-300 focus:ring-rose-500/20"
            : "border-slate-200 focus:ring-indigo-500/20"
        } ${className}`}
        {...props}
      />
      {error && (
        <p className="text-xs font-medium text-rose-600 animate-in fade-in slide-in-from-top-1 duration-150">
          {error}
        </p>
      )}
    </div>
  );
}

export interface SelectProps
  extends SelectHTMLAttributes<HTMLSelectElement>, BaseProps {
  options: { value: string; label: string }[];
}

export function Select({
  label,
  error,
  options,
  className = "",
  id,
  ...props
}: SelectProps) {
  const selectId = id || props.name;
  return (
    <div className="w-full space-y-1.5">
      {label && (
        <label
          htmlFor={selectId}
          className="block text-xs font-semibold text-slate-500 uppercase tracking-wider"
        >
          {label}
        </label>
      )}
      <div className="relative">
        <select
          id={selectId}
          className={`w-full px-4 py-2.5 rounded-xl border bg-white text-sm text-slate-800 transition-all focus:outline-none focus:ring-2 focus:border-indigo-600 disabled:bg-slate-50 disabled:text-slate-400 appearance-none pr-10 ${
            error
              ? "border-rose-300 focus:ring-rose-500/20"
              : "border-slate-200 focus:ring-indigo-500/20"
          } ${className}`}
          {...props}
        >
          <option value="">Select an option</option>
          {options.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
        <div className="absolute inset-y-0 right-0 flex items-center pr-3 pointer-events-none text-slate-400">
          <svg
            className="w-4 h-4"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M19 9l-7 7-7-7"
            />
          </svg>
        </div>
      </div>
      {error && (
        <p className="text-xs font-medium text-rose-600 animate-in fade-in slide-in-from-top-1 duration-150">
          {error}
        </p>
      )}
    </div>
  );
}
