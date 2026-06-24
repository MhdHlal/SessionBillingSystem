import { useState } from "react";
import { DashboardLayout } from "./layouts/DashboardLayout";
import { GradesScreen } from "./features/grades/GradesScreen";

export default function App() {
  const [activeTab, setActiveTab] = useState("grades");

  const renderContent = () => {
    switch (activeTab) {
      case "grades":
        return <GradesScreen />;
      case "teachers":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Teachers Management
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module is part of Sprint 1 and will be built next.
            </p>
          </div>
        );
      case "students":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Students Directory
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module will be built in Sprint 2.
            </p>
          </div>
        );
      case "sessions":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Session Attendance
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module will be built in Sprint 2.
            </p>
          </div>
        );
      case "invoices":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Invoices & Payments
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module will be built in Sprint 3.
            </p>
          </div>
        );
      case "teacher-payments":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Teacher Payroll & Dues
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module will be built in Sprint 3.
            </p>
          </div>
        );
      case "reports":
        return (
          <div className="bg-white p-8 rounded-2xl border border-slate-100 text-center shadow-sm">
            <h3 className="text-lg font-bold text-slate-800">
              Financial Reports
            </h3>
            <p className="text-sm text-slate-400 mt-1">
              This module will be built in Sprint 4.
            </p>
          </div>
        );
      default:
        return <GradesScreen />;
    }
  };

  return (
    <DashboardLayout activeTab={activeTab} onTabChange={setActiveTab}>
      {renderContent()}
    </DashboardLayout>
  );
}
