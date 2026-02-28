import { Navigate } from 'react-router-dom';

/**
 * MyProfilePage - Redirects to profile overview by default
 * This serves as the entry point for /profile route
 */
export default function MyProfilePage() {
  return <Navigate to="/profile/overview" replace />;
}