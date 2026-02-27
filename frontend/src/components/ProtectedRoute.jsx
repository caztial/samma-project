import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

/**
 * Protected route wrapper that checks authentication and optional role requirements.
 * 
 * @param {Object} props
 * @param {React.ReactNode} props.children - The protected content to render
 * @param {string[]} [props.requiredRoles] - Optional array of roles that can access this route
 * @param {string} [props.fallbackPath='/login'] - Where to redirect if not authenticated
 * @param {string} [props.unauthorizedPath='/profile'] - Where to redirect if authenticated but lacks role
 */
export default function ProtectedRoute({ 
  children, 
  requiredRoles = [], 
  fallbackPath = '/login',
  unauthorizedPath = '/profile'
}) {
  const { isAuthenticated, hasAnyRole } = useAuth();

  // Not authenticated - redirect to login
  if (!isAuthenticated) {
    return <Navigate to={fallbackPath} replace />;
  }

  // Has role requirements but doesn't meet them - redirect to unauthorized path
  if (requiredRoles.length > 0 && !hasAnyRole(requiredRoles)) {
    return <Navigate to={unauthorizedPath} replace />;
  }

  // Authenticated (and has required role if specified)
  return children;
}