import { BrowserRouter, Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { Provider } from '@react-spectrum/s2';
import { LocaleProvider, useLocale } from './i18n/LocaleContext';
import { ThemeProvider, useTheme } from './contexts/ThemeContext';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import LoginPage from './pages/auth/LoginPage';
import SignupPage from './pages/auth/SignupPage';
import MyProfilePage from './pages/profile/MyProfilePage';
import ProfileOverviewPage from './pages/profile/ProfileOverviewPage';
import EducationPage from './pages/profile/EducationPage';
import BankAccountsPage from './pages/profile/BankAccountsPage';
import SessionsPage from './pages/profile/SessionsPage';
import AdminPortalPage from './pages/admin/AdminPortalPage';
import ProtectedRoute from './components/ProtectedRoute';

/**
 * Inner routes - placed inside BrowserRouter so useNavigate works,
 * and inside LocaleProvider + ThemeProvider so we can read their state
 * to pass into the S2 Provider.
 */
function AppRoutes() {
  const navigate = useNavigate();
  const { locale } = useLocale();
  const { colorScheme } = useTheme();
  const { isAuthenticated, isAdminOrModerator } = useAuth();

  return (
    <Provider locale={locale} colorScheme={colorScheme} router={{ navigate }}>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        
        {/* Protected routes - all authenticated users */}
        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <MyProfilePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile/overview"
          element={
            <ProtectedRoute>
              <ProfileOverviewPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile/education"
          element={
            <ProtectedRoute>
              <EducationPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile/bank-accounts"
          element={
            <ProtectedRoute>
              <BankAccountsPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/profile/sessions"
          element={
            <ProtectedRoute>
              <SessionsPage />
            </ProtectedRoute>
          }
        />
        
        {/* Protected routes - Admin/Moderator only */}
        <Route
          path="/admin"
          element={
            <ProtectedRoute requiredRoles={['Admin', 'Moderator']}>
              <AdminPortalPage />
            </ProtectedRoute>
          }
        />
        
        {/* Root redirect based on auth status and role */}
        <Route
          path="/"
          element={
            isAuthenticated ? (
              isAdminOrModerator ? (
                <Navigate to="/admin" replace />
              ) : (
                <Navigate to="/profile" replace />
              )
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />
        
        {/* Catch-all redirect */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Provider>
  );
}

function App() {
  return (
    <ThemeProvider>
      <LocaleProvider>
        <AuthProvider>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </AuthProvider>
      </LocaleProvider>
    </ThemeProvider>
  );
}

export default App;