import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useAuthStore } from './store';
import { LoginPage } from './pages/LoginPage';
import { PhotosPage } from './pages/PhotosPage';
import { EnhancementsPage } from './pages/EnhancementsPage';
import { IntegrationsPage } from './pages/IntegrationsPage';
import { UpgradePage } from './pages/UpgradePage';
import './App.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            path="/photos"
            element={
              <ProtectedRoute>
                <PhotosPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/enhancements"
            element={
              <ProtectedRoute>
                <EnhancementsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/integrations"
            element={
              <ProtectedRoute>
                <IntegrationsPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/upgrade"
            element={
              <ProtectedRoute>
                <UpgradePage />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<Navigate to="/photos" replace />} />
          <Route path="*" element={<Navigate to="/photos" replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
