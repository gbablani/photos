import React, { useState } from 'react';
import { Layout } from '../components/Layout';
import { useAuthStore } from '../store';

export const IntegrationsPage: React.FC = () => {
  const { user, updateUser } = useAuthStore();
  const [connecting, setConnecting] = useState<string | null>(null);

  const handleConnectGooglePhotos = async () => {
    setConnecting('google');
    // In production, redirect to Google OAuth
    // Simulate successful connection
    setTimeout(() => {
      updateUser({ googlePhotosConnected: true });
      setConnecting(null);
    }, 1500);
  };

  const handleDisconnectGooglePhotos = async () => {
    setConnecting('google');
    setTimeout(() => {
      updateUser({ googlePhotosConnected: false });
      setConnecting(null);
    }, 1000);
  };

  const handleConnectOneDrive = async () => {
    setConnecting('onedrive');
    setTimeout(() => {
      updateUser({ oneDriveConnected: true });
      setConnecting(null);
    }, 1500);
  };

  const handleDisconnectOneDrive = async () => {
    setConnecting('onedrive');
    setTimeout(() => {
      updateUser({ oneDriveConnected: false });
      setConnecting(null);
    }, 1000);
  };

  return (
    <Layout>
      <div className="integrations-page">
        <div className="page-header">
          <h2>🔗 Connected Services</h2>
          <p>Connect your cloud photo libraries to easily import and sync your photos</p>
        </div>

        <div className="integrations-list">
          {/* Google Photos */}
          <div className="integration-card">
            <div className="integration-icon google">📷</div>
            <div className="integration-info">
              <h3>Google Photos</h3>
              <p>Access and import photos from your Google Photos library</p>
              {user?.googlePhotosConnected && (
                <span className="status connected">✓ Connected</span>
              )}
            </div>
            <div className="integration-actions">
              {user?.googlePhotosConnected ? (
                <>
                  <a href="/import/google-photos" className="btn secondary">
                    Browse Photos
                  </a>
                  <button
                    className="btn danger"
                    onClick={handleDisconnectGooglePhotos}
                    disabled={connecting === 'google'}
                  >
                    {connecting === 'google' ? 'Disconnecting...' : 'Disconnect'}
                  </button>
                </>
              ) : (
                <button
                  className="btn primary"
                  onClick={handleConnectGooglePhotos}
                  disabled={connecting === 'google'}
                >
                  {connecting === 'google' ? 'Connecting...' : 'Connect'}
                </button>
              )}
            </div>
          </div>

          {/* OneDrive */}
          <div className="integration-card">
            <div className="integration-icon onedrive">☁️</div>
            <div className="integration-info">
              <h3>Microsoft OneDrive</h3>
              <p>Access photos from your OneDrive storage</p>
              {user?.oneDriveConnected && <span className="status connected">✓ Connected</span>}
            </div>
            <div className="integration-actions">
              {user?.oneDriveConnected ? (
                <>
                  <a href="/import/onedrive" className="btn secondary">
                    Browse Photos
                  </a>
                  <button
                    className="btn danger"
                    onClick={handleDisconnectOneDrive}
                    disabled={connecting === 'onedrive'}
                  >
                    {connecting === 'onedrive' ? 'Disconnecting...' : 'Disconnect'}
                  </button>
                </>
              ) : (
                <button
                  className="btn primary"
                  onClick={handleConnectOneDrive}
                  disabled={connecting === 'onedrive'}
                >
                  {connecting === 'onedrive' ? 'Connecting...' : 'Connect'}
                </button>
              )}
            </div>
          </div>

          {/* Coming Soon */}
          <div className="integration-card coming-soon">
            <div className="integration-icon">📦</div>
            <div className="integration-info">
              <h3>Dropbox</h3>
              <p>Coming soon - Import photos from Dropbox</p>
            </div>
            <div className="integration-actions">
              <button className="btn disabled" disabled>
                Coming Soon
              </button>
            </div>
          </div>

          <div className="integration-card coming-soon">
            <div className="integration-icon">🍎</div>
            <div className="integration-info">
              <h3>iCloud Photos</h3>
              <p>Coming soon - Access your Apple iCloud Photo Library</p>
            </div>
            <div className="integration-actions">
              <button className="btn disabled" disabled>
                Coming Soon
              </button>
            </div>
          </div>
        </div>

        {/* Auto-sync settings */}
        <div className="sync-settings">
          <h3>Sync Settings</h3>
          <label className="toggle-setting">
            <input
              type="checkbox"
              checked={user?.autoSyncEnabled || false}
              onChange={(e) => updateUser({ autoSyncEnabled: e.target.checked })}
            />
            <span className="toggle-label">
              <strong>Auto-sync enabled</strong>
              <p>Automatically sync new photos from connected services</p>
            </span>
          </label>
        </div>
      </div>
    </Layout>
  );
};
