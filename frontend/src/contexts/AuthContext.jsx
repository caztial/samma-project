import { createContext, useContext, useState, useEffect, useMemo, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext(undefined);

// Local storage keys
const TOKEN_KEY = 'dhamma-token';
const USER_KEY = 'dhamma-user';

export function AuthProvider({ children }) {
  const navigate = useNavigate();
  
  const [user, setUser] = useState(() => {
    const saved = localStorage.getItem(USER_KEY);
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch {
        return null;
      }
    }
    return null;
  });

  const [token, setToken] = useState(() => {
    return localStorage.getItem(TOKEN_KEY) || null;
  });

  // Persist token and user to localStorage
  useEffect(() => {
    if (token) {
      localStorage.setItem(TOKEN_KEY, token);
    } else {
      localStorage.removeItem(TOKEN_KEY);
    }
  }, [token]);

  useEffect(() => {
    if (user) {
      localStorage.setItem(USER_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(USER_KEY);
    }
  }, [user]);

  // Check if user has a specific role
  const hasRole = useCallback((role) => {
    return user?.roles?.includes(role) ?? false;
  }, [user]);

  // Check if user has any of the specified roles
  const hasAnyRole = useCallback((roles) => {
    if (!user?.roles) return false;
    return roles.some(role => user.roles.includes(role));
  }, [user]);

  // Login - store token and user data
  const login = useCallback((loginResponse) => {
    const { token: newToken, ...userData } = loginResponse;
    setToken(newToken);
    setUser({
      profileId: userData.profileId,
      userId: userData.userId,
      email: userData.email,
      firstName: userData.firstName,
      lastName: userData.lastName,
      roles: userData.roles || [],
    });
  }, []);

  // Logout - clear all auth data
  const logout = useCallback(() => {
    setToken(null);
    setUser(null);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }, []);

  // Handle 401 Unauthorized responses - logout and redirect to login
  const onUnauthorized = useCallback(() => {
    console.warn('[AuthContext] Unauthorized - logging out and redirecting to login');
    logout();
    navigate('/login', { replace: true });
  }, [logout, navigate]);

  // Get the current token (for API client)
  const getToken = useCallback(() => {
    return token;
  }, [token]);

  // Derived state
  const isAuthenticated = useMemo(() => !!token && !!user, [token, user]);
  const isAdmin = useMemo(() => hasRole('Admin'), [hasRole]);
  const isModerator = useMemo(() => hasRole('Moderator'), [hasRole]);
  const isAdminOrModerator = useMemo(() => hasAnyRole(['Admin', 'Moderator']), [hasAnyRole]);

  const value = useMemo(() => ({
    user,
    token,
    isAuthenticated,
    isAdmin,
    isModerator,
    isAdminOrModerator,
    hasRole,
    hasAnyRole,
    login,
    logout,
    // For API client
    getToken,
    onUnauthorized,
  }), [user, token, isAuthenticated, isAdmin, isModerator, isAdminOrModerator, hasRole, hasAnyRole, login, logout, getToken, onUnauthorized]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}