/**
 * @file App.tsx
 * @description Root Application Entry Point and Route Orchestrator.
 * @architecture Strictly bound React Router mapping to domain views.
 */

import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Layout from "./components/Layout";
import StudentManagement from "./pages/MasterData/StudentManagement";

// Placeholder components for routing architecture completeness
// These will be replaced by actual implementations in subsequent phases
const PlaceholderView = ({ title }: { title: string }) => (
  <div style={{ padding: "24px", textAlign: "center", color: "#64748b" }}>
    <h2>{title}</h2>
    <p>هذه الشاشة قيد التطوير...</p>
  </div>
);

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* التوجيه الجذري يعتمد Layout كحاضنة لجميع الشاشات الداخلية */}
        <Route path="/" element={<Layout />}>
          {/* إعادة التوجيه التلقائي من الجذر إلى شاشة الطلاب كشاشة افتراضية */}
          <Route index element={<Navigate to="/students" replace />} />

          {/* الشاشات النشطة والمستقبلية */}
          <Route path="students" element={<StudentManagement />} />
          <Route
            path="teachers"
            element={<PlaceholderView title="إدارة المعلمين" />}
          />
          <Route
            path="grades"
            element={<PlaceholderView title="المراحل الدراسية" />}
          />
          <Route
            path="sessions"
            element={<PlaceholderView title="إدارة الحصص" />}
          />
          <Route
            path="billing"
            element={<PlaceholderView title="المالية والفواتير" />}
          />
        </Route>

        {/* معالجة الروابط غير الموجودة (404 Fallback) */}
        <Route
          path="*"
          element={
            <div
              style={{
                display: "flex",
                height: "100vh",
                alignItems: "center",
                justifyContent: "center",
                direction: "rtl",
              }}
            >
              <div style={{ textAlign: "center" }}>
                <h1 style={{ fontSize: "48px", color: "#ef4444", margin: 0 }}>
                  404
                </h1>
                <p style={{ fontSize: "18px", color: "#475569" }}>
                  الصفحة المطلوبة غير موجودة.
                </p>
                <a
                  href="/"
                  style={{
                    color: "#2563eb",
                    textDecoration: "none",
                    fontWeight: 600,
                  }}
                >
                  العودة للرئيسية
                </a>
              </div>
            </div>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}
