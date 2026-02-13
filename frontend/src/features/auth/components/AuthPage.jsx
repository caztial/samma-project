import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Box, BlockStack, Spinner, Text } from '@shopify/polaris';
import Login from './Login';
import Register from './Register';
import ForgotPassword from './ForgotPassword';
import { authService } from '../services/AuthService';

const AuthPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [currentView, setCurrentView] = useState('login');

  // Check if user is already authenticated
  useEffect(() => {
    if (authService.isAuthenticated()) {
      // Try to get user info to determine role
      authService.getCurrentUser()
        .then(user => {
          // Redirect based on user role
          if (user.role === 'Admin') {
            navigate('/admin/dashboard');
          } else if (user.role === 'Presenter') {
            navigate('/presenter/dashboard');
          } else {
            navigate('/participant/dashboard');
          }
        })
        .catch(() => {
          // If we can't get user info, just redirect to participant dashboard
          navigate('/participant/dashboard');
        });
    }
  }, [navigate]);

  const handleLoginSuccess = (authResult) => {
    // Redirect based on user role
    if (authResult.user?.role === 'Admin') {
      navigate('/admin/dashboard');
    } else if (authResult.user?.role === 'Presenter') {
      navigate('/presenter/dashboard');
    } else {
      navigate('/participant/dashboard');
    }
  };

  const handleRegisterSuccess = () => {
    // After successful registration, show login form
    setCurrentView('login');
  };

  const switchView = (view) => {
    setCurrentView(view);
  };

  // If user is already authenticated, show loading
  if (authService.isAuthenticated()) {
    return (
      <Box padding="400" background="bg" borderRadius="200" maxWidth="400px" margin="0 auto">
        <BlockStack gap="300" align="center">
          <Spinner size="large" />
          <Text>Redirecting to dashboard...</Text>
        </BlockStack>
      </Box>
    );
  }

  return (
    <Box padding="400" background="bg" borderRadius="200" maxWidth="400px" margin="0 auto">
      <BlockStack gap="300">
        {currentView === 'login' && (
          <Login
            onSwitchToRegister={() => switchView('register')}
            onSwitchToForgotPassword={() => switchView('forgot-password')}
            onLoginSuccess={handleLoginSuccess}
          />
        )}

        {currentView === 'register' && (
          <Register
            onSwitchToLogin={() => switchView('login')}
            onRegisterSuccess={handleRegisterSuccess}
          />
        )}

        {currentView === 'forgot-password' && (
          <ForgotPassword
            onSwitchToLogin={() => switchView('login')}
          />
        )}
      </BlockStack>
    </Box>
  );
};

export default AuthPage;