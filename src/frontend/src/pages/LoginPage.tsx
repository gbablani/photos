import React from 'react';
import { useAuthStore } from '../store';

export const LoginPage: React.FC = () => {
  const { setAuth } = useAuthStore();

  const handleGoogleLogin = () => {
    // In production, this would redirect to Google OAuth
    // For demo, we'll simulate a successful login
    const mockUser = {
      id: 'demo-user-id',
      email: 'demo@example.com',
      displayName: 'Demo User',
      profilePictureUrl: undefined,
      subscriptionTier: 'Free' as const,
      freeEnhancementsRemaining: 2,
      enhancementCredits: 0,
      subscriptionExpiresAt: undefined,
      googlePhotosConnected: false,
      oneDriveConnected: false,
      autoSyncEnabled: false,
    };
    setAuth(mockUser, 'demo-token', 'demo-refresh-token');
    window.location.href = '/photos';
  };

  const handleMicrosoftLogin = () => {
    // Similar to Google login
    handleGoogleLogin();
  };

  const handleFacebookLogin = () => {
    // Similar to Google login
    handleGoogleLogin();
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <div className="login-header">
          <h1>📸 Photo Memories</h1>
          <p>Rediscover and enhance your precious memories</p>
        </div>

        <div className="login-features">
          <div className="feature">
            <span className="feature-icon">🎨</span>
            <h3>Colorize B&W Photos</h3>
            <p>Bring old black & white photos to life with AI-powered colorization</p>
          </div>
          <div className="feature">
            <span className="feature-icon">🎬</span>
            <h3>Animate Still Photos</h3>
            <p>Transform static images into dynamic video clips</p>
          </div>
          <div className="feature">
            <span className="feature-icon">🔧</span>
            <h3>Restore Quality</h3>
            <p>Enhance faded or low-resolution images instantly</p>
          </div>
        </div>

        <div className="login-buttons">
          <h2>Get Started</h2>
          <p>Sign in with your favorite provider</p>

          <button className="login-btn google" onClick={handleGoogleLogin}>
            <span className="btn-icon">🔵</span>
            Continue with Google
          </button>

          <button className="login-btn microsoft" onClick={handleMicrosoftLogin}>
            <span className="btn-icon">🔷</span>
            Continue with Microsoft
          </button>

          <button className="login-btn facebook" onClick={handleFacebookLogin}>
            <span className="btn-icon">🔹</span>
            Continue with Facebook
          </button>
        </div>

        <div className="login-footer">
          <p>
            By signing in, you agree to our <a href="/terms">Terms of Service</a> and{' '}
            <a href="/privacy">Privacy Policy</a>
          </p>
        </div>
      </div>
    </div>
  );
};
