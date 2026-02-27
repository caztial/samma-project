import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import {
  Form,
  TextField,
  Button,
  InlineAlert,
  Heading,
  Content,
  ActionButton,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import Visibility from '@react-spectrum/s2/icons/Visibility';
import VisibilityOff from '@react-spectrum/s2/icons/VisibilityOff';
import { useTranslation } from '../../i18n/useTranslation';
import authService from '../../services/authService';
import AuthLayout from './AuthLayout';

export default function SignupPage() {
  const navigate = useNavigate();
  const { t } = useTranslation();

  const [serverError, setServerError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [hasValidationError, setHasValidationError] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [passwordMismatch, setPasswordMismatch] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    setHasValidationError(false);
    setFieldErrors({});

    // Check password match before submitting
    if (password !== confirmPassword) {
      setPasswordMismatch(true);
      return;
    }
    setPasswordMismatch(false);

    const formData = new FormData(e.currentTarget);
    const firstName = formData.get('firstName');
    const lastName = formData.get('lastName');
    const email = formData.get('email');

    setIsLoading(true);
    try {
      const response = await authService.register(firstName, lastName, email, password);
      if (response.data?.token) {
        localStorage.setItem('token', response.data.token);
      }
      navigate('/');
    } catch (err) {
      if (err.response) {
        const status = err.response.status;
        const errorData = err.response.data;

        if (status === 409 || status === 400) {
          // Check for field-specific errors
          if (errorData?.errors && typeof errorData.errors === 'object') {
            setFieldErrors(errorData.errors);
            setServerError(errorData.message || t('signup.errors.accountExists'));
          } else {
            const detail =
              errorData?.message ||
              errorData?.detail ||
              t('signup.errors.accountExists');
            setServerError(detail);
          }
        } else {
          setServerError(t('signup.errors.serverError'));
        }
      } else {
        setServerError(t('signup.errors.connectionError'));
      }
    } finally {
      setIsLoading(false);
    }
  }

  function handleInvalid(e) {
    e.preventDefault();
    setHasValidationError(true);
    // Clear server errors when frontend validation fails
    setServerError('');
    setFieldErrors({});
    setPasswordMismatch(false);
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
            {t('signup.title')}
          </h1>
          <p className={style({ font: 'body-lg', color: 'gray-700' })}>
            {t('signup.subtitle')}
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
        {(serverError || passwordMismatch) && (
          <InlineAlert variant="negative" autoFocus>
            <Heading>{t('auth.signupFailedTitle')}</Heading>
            <Content>
              {passwordMismatch && t('signup.passwordMismatch')}
              {serverError && !passwordMismatch && serverError}
              {Object.keys(fieldErrors).length > 0 && (
                <ul className={style({ margin: 0, paddingLeft: 20 })}>
                  {Object.entries(fieldErrors).map(([field, messages]) => (
                    <li key={field}>
                      <strong>{field}:</strong> {Array.isArray(messages) ? messages.join(', ') : messages}
                    </li>
                  ))}
                </ul>
              )}
            </Content>
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
          {/* Name row — wraps on narrow screens */}
          <div
            className={style({
              display: 'flex',
              gap: 16,
              flexWrap: 'wrap',
            })}
          >
            <TextField
              label={t('signup.firstName')}
              name="firstName"
              type="text"
              autoComplete="given-name"
              placeholder={t('signup.firstNamePlaceholder')}
              isRequired
              isDisabled={isLoading}
              validationBehavior="native"
              styles={style({ flex: 1, minWidth: 140 })}
            />
            <TextField
              label={t('signup.lastName')}
              name="lastName"
              type="text"
              autoComplete="family-name"
              placeholder={t('signup.lastNamePlaceholder')}
              isRequired
              isDisabled={isLoading}
              validationBehavior="native"
              styles={style({ flex: 1, minWidth: 140 })}
            />
          </div>

          <TextField
            label={t('signup.email')}
            name="email"
            type="email"
            inputMode="email"
            autoComplete="email"
            placeholder={t('signup.emailPlaceholder')}
            isRequired
            isDisabled={isLoading}
            validationBehavior="native"
            {...(fieldErrors.email ? { isInvalid: true, errorMessage: fieldErrors.email[0] } : {})}
            onChange={() => {
              // Clear email server error when user types
              if (fieldErrors.email) {
                setFieldErrors((prev) => {
                  const { email: _, ...rest } = prev;
                  return rest;
                });
                setServerError('');
              }
            }}
            styles={style({ width: 'full' })}
          />

          {/* Password field with show/hide toggle */}
          <div style={{ display: 'flex', alignItems: 'flex-end', gap: 8 }}>
            <div style={{ flex: 1 }}>
              <TextField
                label={t('signup.password')}
                name="password"
                type={showPassword ? 'text' : 'password'}
                autoComplete="new-password"
                placeholder={t('signup.passwordPlaceholder')}
                isRequired
                minLength={8}
                description={t('signup.passwordDescription')}
                isDisabled={isLoading}
                validationBehavior="native"
                value={password}
                onChange={(value) => {
                  setPassword(value);
                  // Clear password mismatch if passwords now match
                  if (passwordMismatch && value === confirmPassword) {
                    setPasswordMismatch(false);
                  }
                }}
                styles={style({ width: 'full' })}
              />
            </div>
            <ActionButton
              type="button"
              isQuiet
              aria-label={showPassword ? t('signup.hidePassword') : t('signup.showPassword')}
              onPress={() => setShowPassword(!showPassword)}
              isDisabled={isLoading}
            >
              {showPassword ? <VisibilityOff /> : <Visibility />}
            </ActionButton>
          </div>

          {/* Confirm password field with show/hide toggle */}
          <div style={{ display: 'flex', alignItems: 'flex-end', gap: 8 }}>
            <div style={{ flex: 1 }}>
              <TextField
                label={t('signup.confirmPassword')}
                name="confirmPassword"
                type={showConfirmPassword ? 'text' : 'password'}
                autoComplete="new-password"
                placeholder={t('signup.confirmPasswordPlaceholder')}
                isRequired
                isDisabled={isLoading}
                validationBehavior="native"
                {...(passwordMismatch ? { isInvalid: true, errorMessage: t('signup.passwordMismatch') } : {})}
                value={confirmPassword}
                onChange={(value) => {
                  setConfirmPassword(value);
                  // Clear password mismatch if passwords now match
                  if (passwordMismatch && value === password) {
                    setPasswordMismatch(false);
                  }
                }}
                styles={style({ width: 'full' })}
              />
            </div>
            <ActionButton
              type="button"
              isQuiet
              aria-label={showConfirmPassword ? t('signup.hidePassword') : t('signup.showPassword')}
              onPress={() => setShowConfirmPassword(!showConfirmPassword)}
              isDisabled={isLoading}
            >
              {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
            </ActionButton>
          </div>

          <div className={style({ paddingTop: 8 })}>
            <Button
              type="submit"
              variant="accent"
              isPending={isLoading}
              styles={style({ width: 'full' })}
            >
              {isLoading ? t('signup.creatingAccount') : t('signup.createAccount')}
            </Button>
          </div>
        </Form>

        {/* ── Sign in link ── */}
        <p
          className={style({
            textAlign: 'center',
            font: 'body',
            color: 'gray-700',
          })}
        >
          {t('signup.haveAccount')}{' '}
          <Link
            to="/login"
            className={style({ color: 'accent-900', fontWeight: 'bold' })}
          >
            {t('signup.signIn')}
          </Link>
        </p>
      </div>
    </AuthLayout>
  );
}
