import { useNavigate } from 'react-router-dom';
import { Picker, PickerItem, Switch, Button } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../i18n/useTranslation';
import { useLocale } from '../i18n/LocaleContext';
import { useTheme } from '../contexts/ThemeContext';
import { useAuth } from '../contexts/AuthContext';

/**
 * Shared layout for authenticated pages.
 * Renders a top bar with user greeting, language picker, dark mode toggle, and logout button.
 */
export default function MainLayout({ children }) {
  const { t } = useTranslation();
  const { locale, setLocale } = useLocale();
  const { colorScheme, toggleColorScheme } = useTheme();
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const userDisplayName = user?.firstName 
    ? `${user.firstName}${user.lastName ? ` ${user.lastName}` : ''}`
    : user?.email || 'User';

  return (
    <div
      className={style({
        display: 'flex',
        flexDirection: 'column',
        minHeight: '100vh',
        backgroundColor: 'base',
      })}
    >
      {/* ── Top bar ── */}
      <div
        className={style({
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          paddingTop: 12,
          paddingBottom: 12,
          paddingLeft: 24,
          paddingRight: 24,
          gap: 16,
          borderBottomWidth: 1,
          borderBottomStyle: 'solid',
          borderBottomColor: 'gray-200',
        })}
      >
        {/* ── Left: Greeting ── */}
        <div className={style({ display: 'flex', alignItems: 'center', gap: 8 })}>
          <span className={style({ font: 'body-lg', color: 'gray-900' })}>
            {t('navigation.greeting', { name: userDisplayName })}
          </span>
        </div>

        {/* ── Right: Controls ── */}
        <div
          className={style({
            display: 'flex',
            alignItems: 'center',
            gap: 16,
          })}
        >
          <Picker
            aria-label={t('language.switch')}
            selectedKey={locale}
            onSelectionChange={setLocale}
            size="S"
            isQuiet
          >
            <PickerItem id="en-US">{t('language.english')}</PickerItem>
            <PickerItem id="si-LK">{t('language.sinhala')}</PickerItem>
          </Picker>

          <Switch
            isSelected={colorScheme === 'dark'}
            onChange={toggleColorScheme}
            size="S"
            isEmphasized
          >
            {t('theme.darkMode')}
          </Switch>

          <Button
            variant="secondary"
            size="S"
            onPress={handleLogout}
          >
            {t('navigation.logout')}
          </Button>
        </div>
      </div>

      {/* ── Content ── */}
      <div
        className={style({
          flex: 1,
          display: 'flex',
          flexDirection: 'column',
          paddingTop: 24,
          paddingBottom: 24,
          paddingLeft: 24,
          paddingRight: 24,
        })}
      >
        {children}
      </div>
    </div>
  );
}