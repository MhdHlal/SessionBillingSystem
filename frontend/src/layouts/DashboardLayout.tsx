import { useState } from "react";
import {
  GraduationCap,
  Users,
  BookOpen,
  CalendarDays,
  Receipt,
  CreditCard,
  BarChart3,
  Menu,
  X,
  User,
  LogOut,
  Coins,
} from "lucide-react";

export interface DashboardLayoutProps {
  children: React.ReactNode;
  activeTab: string;
  onTabChange: (tabId: string) => void;
}

export function DashboardLayout({
  children,
  activeTab,
  onTabChange,
}: DashboardLayoutProps) {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const navigationItems = [
    { id: "grades", name: "Academic Grades", icon: GraduationCap },
    { id: "teachers", name: "Teachers Directory", icon: Users },
    { id: "students", name: "Students Directory", icon: BookOpen },
    { id: "pricing", name: "Teacher Pricing", icon: Coins },
    { id: "sessions", name: "Session Registry", icon: CalendarDays },
    { id: "invoices", name: "Invoices & Payments", icon: Receipt },
    { id: "teacher-payments", name: "Teacher Payroll", icon: CreditCard },
    { id: "reports", name: "Financial Reports", icon: BarChart3 },
  ];

  const activeItem =
    navigationItems.find((item) => item.id === activeTab) || navigationItems[0];
  const ActiveIcon = activeItem.icon;

  const handleNavigation = (tabId: string) => {
    onTabChange(tabId);
    setIsMobileMenuOpen(false);
  };

  return (
    <div className="min-h-screen bg-slate-50 flex text-left" dir="ltr">
      <aside className="hidden lg:flex flex-col w-72 bg-slate-900 text-slate-300 border-r border-slate-800 fixed inset-y-0 left-0 z-20">
        <div className="h-20 flex items-center gap-3 px-6 border-b border-slate-800 bg-slate-950">
          <div className="flex items-center justify-center w-10 h-10 rounded-xl bg-indigo-600 text-white shadow-md shadow-indigo-600/10">
            <GraduationCap className="w-5 h-5" />
          </div>
          <div>
            <h1 className="text-sm font-bold text-white leading-tight">
              Educational Hub
            </h1>
            <span className="text-[10px] text-slate-500 font-medium uppercase tracking-wider">
              Accounting Center
            </span>
          </div>
        </div>

        <nav className="flex-1 px-4 py-6 space-y-1 overflow-y-auto">
          {navigationItems.map((item) => {
            const Icon = item.icon;
            const isActive = item.id === activeTab;
            return (
              <button
                key={item.id}
                onClick={() => handleNavigation(item.id)}
                className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all duration-150 ${
                  isActive
                    ? "bg-indigo-600 text-white shadow-md shadow-indigo-600/10"
                    : "text-slate-400 hover:bg-slate-800 hover:text-white"
                }`}
              >
                <Icon
                  className={`w-4.5 h-4.5 ${isActive ? "text-white" : "text-slate-400"}`}
                />
                <span>{item.name}</span>
              </button>
            );
          })}
        </nav>

        <div className="p-4 border-t border-slate-800 bg-slate-950">
          <div className="flex items-center justify-between p-3 rounded-xl bg-slate-900 border border-slate-800">
            <div className="flex items-center gap-3">
              <div className="w-9 h-9 rounded-lg bg-slate-800 flex items-center justify-center text-indigo-400">
                <User className="w-4 h-4" />
              </div>
              <div className="text-left">
                <h4 className="text-xs font-bold text-white">Administrator</h4>
                <p className="text-[10px] text-slate-500 font-medium">
                  Finance Dept
                </p>
              </div>
            </div>
            <button className="p-1.5 rounded-lg text-slate-500 hover:bg-slate-800 hover:text-rose-400 transition-colors">
              <LogOut className="w-4 h-4" />
            </button>
          </div>
        </div>
      </aside>

      {isMobileMenuOpen && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div
            className="fixed inset-0 bg-slate-950/60 backdrop-blur-sm"
            onClick={() => setIsMobileMenuOpen(false)}
          />
          <aside className="fixed inset-y-0 left-0 w-72 bg-slate-900 text-slate-300 flex flex-col z-50 animate-in slide-in-from-left duration-200">
            <div className="h-20 flex items-center justify-between px-6 border-b border-slate-800 bg-slate-950">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-indigo-600 text-white flex items-center justify-center shadow-md">
                  <GraduationCap className="w-5 h-5" />
                </div>
                <h1 className="text-sm font-bold text-white">
                  Educational Hub
                </h1>
              </div>
              <button
                onClick={() => setIsMobileMenuOpen(false)}
                className="p-1.5 rounded-lg text-slate-400 hover:bg-slate-800"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            <nav className="flex-1 px-4 py-6 space-y-1 overflow-y-auto">
              {navigationItems.map((item) => {
                const Icon = item.icon;
                const isActive = item.id === activeTab;
                return (
                  <button
                    key={item.id}
                    onClick={() => handleNavigation(item.id)}
                    className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all ${
                      isActive
                        ? "bg-indigo-600 text-white"
                        : "text-slate-400 hover:bg-slate-800"
                    }`}
                  >
                    <Icon
                      className={`w-4.5 h-4.5 ${isActive ? "text-white" : "text-slate-400"}`}
                    />
                    <span>{item.name}</span>
                  </button>
                );
              })}
            </nav>
          </aside>
        </div>
      )}

      <div className="flex-1 flex flex-col lg:pl-72 min-w-0">
        <header className="h-20 bg-white border-b border-slate-200/80 shadow-sm flex items-center justify-between px-4 sm:px-8 sticky top-0 z-10">
          <div className="flex items-center gap-4">
            <button
              onClick={() => setIsMobileMenuOpen(true)}
              className="p-2 rounded-xl text-slate-600 hover:bg-slate-50 border border-slate-200 lg:hidden"
            >
              <Menu className="w-5 h-5" />
            </button>
            <div className="flex items-center gap-3">
              <div className="p-2 bg-indigo-50 rounded-xl text-indigo-600 hidden sm:block">
                <ActiveIcon className="w-5 h-5" />
              </div>
              <div className="text-left">
                <h2 className="text-lg font-bold text-slate-900 leading-tight">
                  {activeItem.name}
                </h2>
                <p className="text-xs text-slate-400 font-medium hidden sm:block">
                  Educational Center Accounting Portal
                </p>
              </div>
            </div>
          </div>

          <div className="flex items-center gap-3">
            <span className="flex h-2.5 w-2.5 relative">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-emerald-400 opacity-75"></span>
              <span className="relative inline-flex rounded-full h-2.5 w-2.5 bg-emerald-500"></span>
            </span>
            <span className="text-xs font-semibold text-slate-500 bg-slate-100 px-3 py-1.5 rounded-full">
              Secure Connection
            </span>
          </div>
        </header>

        <main className="flex-1 p-4 sm:p-8 overflow-y-auto max-w-[1600px] mx-auto w-full text-left">
          <div className="animate-in fade-in slide-in-from-bottom-2 duration-200">
            {children}
          </div>
        </main>
      </div>
    </div>
  );
}
