import React from 'react';
import { useAuthStore } from '../store';
import { useUIStore } from '../store';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuthStore();
  const { sidebarOpen, toggleSidebar, searchQuery, setSearchQuery } = useUIStore();

  return (
    <div className="app-layout">
      {/* Header */}
      <header className="app-header">
        <div className="header-left">
          <button className="menu-toggle" onClick={toggleSidebar} aria-label="Toggle menu">
            ☰
          </button>
          <h1 className="app-title">📸 Photo Memories</h1>
        </div>

        <div className="header-center">
          <div className="search-bar">
            <input
              type="text"
              placeholder="Search photos by text, date, or person..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
            <button className="search-btn">🔍</button>
          </div>
        </div>

        <div className="header-right">
          {user && (
            <div className="user-menu">
              <img
                src={user.profilePictureUrl || '/default-avatar.png'}
                alt={user.displayName}
                className="user-avatar"
              />
              <span className="user-name">{user.displayName}</span>
              <button onClick={logout} className="logout-btn">
                Logout
              </button>
            </div>
          )}
        </div>
      </header>

      <div className="app-body">
        {/* Sidebar */}
        <aside className={`app-sidebar ${sidebarOpen ? 'open' : 'closed'}`}>
          <nav className="sidebar-nav">
            <a href="/photos" className="nav-item">
              🖼️ My Photos
            </a>
            <a href="/albums" className="nav-item">
              📁 Albums
            </a>
            <a href="/timeline" className="nav-item">
              📅 Timeline
            </a>
            <a href="/people" className="nav-item">
              👥 People
            </a>
            <a href="/enhancements" className="nav-item">
              ✨ Enhancements
            </a>

            <hr className="nav-divider" />

            <a href="/integrations" className="nav-item">
              🔗 Connected Services
            </a>
            <a href="/settings" className="nav-item">
              ⚙️ Settings
            </a>
          </nav>

          {user && (
            <div className="subscription-info">
              <h4>Subscription</h4>
              <p className="tier">{user.subscriptionTier}</p>
              {user.subscriptionTier === 'Free' && (
                <p className="credits">{user.freeEnhancementsRemaining} free enhancements left</p>
              )}
              {user.enhancementCredits > 0 && <p className="credits">{user.enhancementCredits} credits</p>}
              <a href="/upgrade" className="upgrade-link">
                Upgrade Plan
              </a>
            </div>
          )}
        </aside>

        {/* Main Content */}
        <main className="app-main">{children}</main>
      </div>
    </div>
  );
};
