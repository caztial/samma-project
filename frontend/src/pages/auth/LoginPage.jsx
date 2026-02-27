import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import {
  Form,
  TextField,
  Button,
  InlineAlert,
  Heading,
  Content,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import { useAuth } from '../../contexts/AuthContext';
import authService from '../../services/authService';
import AuthLayout from './AuthLayout';

export default function LoginPage() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const { login } = useAuth();

  const [serverError, setServerError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [hasValidationError, setHasValidationError] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    setHasValidationError(false);

    const formData = new FormData(e.currentTarget);
    const email = formData.get('email');
    const password = formData.get('password');

    setIsLoading(true);
    try {
      const response = await authService.login(email, password);
      if (response.data?.token) {
        // Store all auth data using AuthContext
        login(response.data);
        
        // Role-based redirect
        const roles = response.data.roles || [];
        const hasAdminAccess = roles.some(role => role === 'Admin' || role === 'Moderator');
        
        if (hasAdminAccess) {
          navigate('/admin');
        } else {
          navigate('/profile');
        }
      }
    } catch (err) {
      if (err.response) {
        const status = err.response.status;
        if (status === 401 || status === 400) {
          setServerError(t('login.errors.invalidCredentials'));
        } else {
          setServerError(t('login.errors.serverError'));
        }
      } else {
        setServerError(t('login.errors.connectionError'));
      }
    } finally {
      setIsLoading(false);
    }
  }

  function handleInvalid(e) {
    e.preventDefault();
    setHasValidationError(true);
  }

  return (
    <AuthLayout>
      <div
        className={style({
          width: 'full',
          maxWidth: 460,
          display: 'flex',
          flexDirection: 'column',
          gap: 32,
        })}
      >
        {/* ── Header ── */}
        <div className={style({ textAlign: 'center' })}>
          <h1
            className={style({
              font: 'heading-xl',
              color: 'gray-900',
              marginBottom: 8,
            })}
          >
            {t('login.title')}
          </h1>
          <p className={style({ font: 'body-lg', color: 'gray-700' })}>
            {t('login.subtitle')}
          </p>
        </div>

        {/* ── Validation summary alert ── */}
        {hasValidationError && (
          <InlineAlert variant="negative" autoFocus>
            <Heading>{t('auth.validationTitle')}</Heading>
            <Content>{t('auth.validationMessage')}</Content>
          </InlineAlert>
        )}

        {/* ── Server error alert ── */}
        {serverError && (
          <InlineAlert variant="negative" autoFocus>
            <Heading>{t('auth.loginFailedTitle')}</Heading>
            <Content>{serverError}</Content>
          </InlineAlert>
        )}

        {/* ── Form ── */}
        <Form
          onSubmit={handleSubmit}
          onInvalid={handleInvalid}
          styles={style({
            display: 'flex',
            flexDirection: 'column',
            gap: 20,
          })}
        >
          <TextField
            label={t('login.email')}
            name="email"
            type="email"
            inputMode="email"
            autoComplete="email"
            placeholder={t('login.emailPlaceholder')}
            isRequired
            isDisabled={isLoading}
            validationBehavior="native"
            styles={style({ width: 'full' })}
          />

          <TextField
            label={t('login.password')}
            name="password"
            type="password"
            autoComplete="current-password"
            placeholder={t('login.passwordPlaceholder')}
            isRequired
            minLength={6}
            isDisabled={isLoading}
            validationBehavior="native"
            styles={style({ width: 'full' })}
          />

          <div className={style({ paddingTop: 8 })}>
            <Button
              type="submit"
              variant="accent"
              isPending={isLoading}
              styles={style({ width: 'full' })}
            >
              {isLoading ? t('login.signingIn') : t('login.signIn')}
            </Button>
          </div>
        </Form>

        {/* ── Sign up link ── */}
        <p
          className={style({
            textAlign: 'center',
            font: 'body',
            color: 'gray-700',
          })}
        >
          {t('login.noAccount')}{' '}
          <Link
            to="/signup"
            className={style({ color: 'accent-900', fontWeight: 'bold' })}
          >
            {t('login.signUp')}
          </Link>
        </p>
      </div>
    </AuthLayout>
  );
}