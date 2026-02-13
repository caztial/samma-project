import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AppProvider } from '@shopify/polaris';
import enTranslations from '@shopify/polaris/locales/en.json';

// Import auth components
import AuthPage from './features/auth/components/AuthPage';
import { authService } from './features/auth/services/AuthService';

// Import layout components
import Layout from './components/Layout';

// Import page components (to be created)
import ParticipantDashboard from './pages/ParticipantDashboard';
import AdminDashboard from './pages/AdminDashboard';
import PresenterDashboard from './pages/PresenterDashboard';

// Import CSS
import './App.css';

const App = () => {
  // Check authentication status
  const isAuthenticated = authService.isAuthenticated();

  return (
    <AppProvider i18n={enTranslations}>
      <Router>
        <div className="app">
          <Routes>
            {/* Public routes */}
            <Route path="/auth" element={<AuthPage />} />
            <Route path="/login" element={<Navigate to="/auth" replace />} />
            <Route path="/register" element={<Navigate to="/auth" replace />} />
            <Route path="/forgot-password" element={<Navigate to="/auth" replace />} />

            {/* Protected routes */}
            <Route 
              path="/" 
              element={
                isAuthenticated ? (
                  <Navigate to="/participant/dashboard" replace />
                ) : (
                  <Navigate to="/auth" replace />
                )
              } 
            />

            {/* Participant routes */}
            <Route 
              path="/participant/*" 
              element={
                isAuthenticated ? (
                  <Layout>
                    <Routes>
                      <Route path="dashboard" element={<ParticipantDashboard />} />
                      {/* Add more participant routes here */}
                    </Routes>
                  </Layout>
                ) : (
                  <Navigate to="/auth" replace />
                )
              } 
            />

            {/* Admin routes */}
            <Route 
              path="/admin/*" 
              element={
                isAuthenticated ? (
                  <Layout>
                    <Routes>
                      <Route path="dashboard" element={<AdminDashboard />} />
                      {/* Add more admin routes here */}
                    </Routes>
                  </Layout>
                ) : (
                  <Navigate to="/auth" replace />
                )
              } 
            />

            {/* Presenter routes */}
            <Route 
              path="/presenter/*" 
              element={
                isAuthenticated ? (
                  <Layout>
                    <Routes>
                      <Route path="dashboard" element={<PresenterDashboard />} />
                      {/* Add more presenter routes here */}
                    </Routes>
                  </Layout>
                ) : (
                  <Navigate to="/auth" replace />
                )
              } 
            />

            {/* Catch all route */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </Router>
    </AppProvider>
  );
};

export default App;