import { useEffect, useState } from 'react';
import { getSpaces } from './services/spaceService';
import { getStoredAuth, loginUser, setStoredAuth } from './services/api';

import Dashboard from './components/Dashboard/Dashboard';
import GISMap from './components/GIS/GISMap';

import Spaces from './pages/Spaces';
import LearningRequests from './pages/LearningRequests';
import Communities from './pages/Communities';
import StudentDemand from './pages/StudentDemand';

import Allocation from './pages/Allocation';
import Allocations from './pages/Allocations';
import Analytics from './pages/Analytics';

import Partners from './pages/Partners';
import Courses from './pages/Courses';
import Scheduling from './pages/Scheduling';
import AuditLogs from './pages/AuditLogs';
import SocialImpact from './pages/SocialImpact';
import SDGDashboard from './pages/SDGDashboard';

const PRESET_ACCOUNTS = [
  { role: 'Admin', email: 'admin@eduspace.local', pass: 'Admin@12345', label: 'Administrator', cls: 'role-admin' },
  { role: 'NGO', email: 'ngo@eduspace.local', pass: 'NGO@12345', label: 'NGO Partner', cls: 'role-ngo' },
  { role: 'EducationCoordinator', email: 'coordinator@eduspace.local', pass: 'Coordinator@12345', label: 'Coordinator', cls: 'role-coordinator' },
  { role: 'Viewer', email: 'viewer@eduspace.local', pass: 'Viewer@12345', label: 'Public Viewer', cls: 'role-viewer' },
];

function App() {
  const [activePage, setActivePage] = useState('dashboard');
  const [spaces, setSpaces] = useState([]);
  const [currentAuth, setCurrentAuth] = useState(() => getStoredAuth());
  const [showRoleModal, setShowRoleModal] = useState(false);
  const [authLoading, setAuthLoading] = useState(false);

  useEffect(() => {
    getSpaces().then(setSpaces).catch(console.error);

    // Auto-login as Admin on first launch if not logged in
    const stored = getStoredAuth();
    if (!stored.token) {
      handleSwitchRole(PRESET_ACCOUNTS[0]);
    }
  }, []);

  const handleSwitchRole = async (account) => {
    setAuthLoading(true);
    try {
      const res = await loginUser(account.email, account.pass);
      setCurrentAuth({
        token: res.token,
        user: { email: res.email, roles: res.roles },
      });
      setShowRoleModal(false);
    } catch (err) {
      console.error('Failed to authenticate:', err);
    } finally {
      setAuthLoading(false);
    }
  };

  const activeRole = currentAuth?.user?.roles?.[0] || 'Admin';
  const roleClass =
    activeRole === 'Admin'
      ? 'role-admin'
      : activeRole === 'NGO'
      ? 'role-ngo'
      : activeRole === 'EducationCoordinator'
      ? 'role-coordinator'
      : 'role-viewer';

  const menu = [
    ['dashboard', 'Dashboard'],
    ['spaces', 'Space Registry'],
    ['requests', 'Learning Requests'],
    ['communities', 'Communities'],
    ['demand', 'Student Demand'],
    ['gis', 'GIS Mapping'],
    ['allocation', 'Smart Allocation'],
    ['allocations', 'Allocation Results'],
    ['analytics', 'Analytics'],
    ['partners', 'Partners'],
    ['courses', 'Courses'],
    ['scheduling', 'Scheduling'],
    ['audit', 'Audit Logs'],
    ['impact', 'Social Impact'],
    ['sdg', 'SDG Dashboard'],
  ];

  const renderPage = () => {
    switch (activePage) {
      case 'spaces':
        return <Spaces />;

      case 'requests':
        return <LearningRequests />;

      case 'communities':
        return <Communities />;

      case 'demand':
        return <StudentDemand />;

      case 'gis':
        return (
          <>
            <div className="page-heading">
              <div>
                <h2>GIS Mapping & Spatial Coverage</h2>
                <p className="page-subtitle">
                  Interactive PostGIS layers: Learning Spaces, Community Demand, and Smart Allocations.
                </p>
              </div>
            </div>
            <GISMap spaces={spaces} />
          </>
        );

      case 'allocation':
        return <Allocation />;

      case 'allocations':
        return <Allocations />;

      case 'analytics':
        return <Analytics />;

      case 'partners':
        return <Partners />;

      case 'courses':
        return <Courses />;

      case 'scheduling':
        return <Scheduling />;

      case 'audit':
        return <AuditLogs />;

      case 'impact':
        return <SocialImpact />;

      case 'sdg':
        return <SDGDashboard />;

      default:
        return <Dashboard />;
    }
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-title">EduSpace</div>
          <div className="brand-subtitle">Allocator • Smart City</div>
        </div>

        <nav className="sidebar-nav">
          {menu.map(([id, label]) => (
            <button
              key={id}
              className={`nav-button ${activePage === id ? 'active' : ''}`}
              onClick={() => setActivePage(id)}
            >
              {label}
            </button>
          ))}
        </nav>
      </aside>

      <main className="main-content">
        <header className="topbar">
          <div>
            <h1>EduSpace Allocator</h1>
            <span>Smart Education Infrastructure & Underutilized Space Optimization</span>
          </div>

          <div className="topbar-right">
            <div className="user-badge" onClick={() => setShowRoleModal(true)} style={{ cursor: 'pointer' }}>
              <span>👤 {currentAuth?.user?.email || 'admin@eduspace.local'}</span>
              <span className={`role-pill ${roleClass}`}>{activeRole}</span>
              <span style={{ fontSize: '11px', color: '#94a3b8' }}>▼ Switch</span>
            </div>

            <div className="system-status">
              <span className="status-dot" />
              API Online
            </div>
          </div>
        </header>

        <section className="page-content">
          {renderPage()}
        </section>
      </main>

      {showRoleModal && (
        <div className="modal-overlay" onClick={() => setShowRoleModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Switch User Role (Phase 22 RBAC)</h3>
              <button className="close-btn" onClick={() => setShowRoleModal(false)}>✕</button>
            </div>
            <p style={{ fontSize: '13px', color: '#94a3b8', margin: '0 0 16px' }}>
              Select a persona to test role-based authentication and JWT permission levels:
            </p>

            <div className="role-switch-grid">
              {PRESET_ACCOUNTS.map((acc) => (
                <div
                  key={acc.role}
                  className={`role-card ${activeRole === acc.role ? 'active' : ''}`}
                  onClick={() => handleSwitchRole(acc)}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '6px' }}>
                    <strong style={{ fontSize: '14px' }}>{acc.label}</strong>
                    <span className={`role-pill ${acc.cls}`}>{acc.role}</span>
                  </div>
                  <div style={{ fontSize: '12px', color: '#94a3b8' }}>{acc.email}</div>
                </div>
              ))}
            </div>

            {authLoading && <div style={{ textAlign: 'center', color: '#3b82f6', fontSize: '13px' }}>Authenticating via JWT...</div>}
          </div>
        </div>
      )}
    </div>
  );
}

export default App;