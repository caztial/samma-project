import { BrowserRouter, Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { Provider } from '@react-spectrum/s2';
import { LocaleProvider, useLocale } from './i18n/LocaleContext';
import { ThemeProvider, useTheme } from './contexts/ThemeContext';
import LoginPage from './pages/auth/LoginPage';
import SignupPage from './pages/auth/SignupPage';

/**
 * Inner routes - placed inside BrowserRouter so useNavigate works,
 * and inside LocaleProvider + ThemeProvider so we can read their state
 * to pass into the S2 Provider.
 */
function AppRoutes() {
  const navigate = useNavigate();
  const { locale } = useLocale();
  const { colorScheme } = useTheme();

  return (
    <Provider locale={locale} colorScheme={colorScheme} router={{ navigate }}>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignupPage />} />
        <Route path="/" element={<Navigate to="/login" replace />} />
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Provider>
  );
}

function App() {
  return (
    <ThemeProvider>
      <LocaleProvider>
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
      </LocaleProvider>
    </ThemeProvider>
  );
}

export default App;
