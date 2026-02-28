import { useNavigate } from 'react-router-dom';
import { Picker, PickerItem, Switch, Button, MenuTrigger, Menu, MenuItem, MenuSection, Header, Heading, Text, ActionButton } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../i18n/useTranslation';
import { useLocale } from '../i18n/LocaleContext';
import { useTheme } from '../contexts/ThemeContext';
import { useAuth } from '../contexts/AuthContext';
import MenuHamburger from '@react-spectrum/s2/icons/MenuHamburger';
import Translate from '@react-spectrum/s2/icons/Translate';
import Lightbulb from '@react-spectrum/s2/icons/Lightbulb';
import Leave from '@react-spectrum/s2/icons/Leave';

/**
 * Shared layout for authenticated pages.
 * Renders a top bar with user greeting, language picker, dark mode toggle, and logout button.
 * On mobile screens (< 640px), these controls are collapsed into a hamburger menu.
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
          paddingLeft: 16,
          paddingRight: 16,
          sm: {
            paddingLeft: 24,
            paddingRight: 24,
          },
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
        
        {/* Mobile Menu (visible < 640px) */}
        <div
          className={style({
            display: 'flex',
            sm: { display: 'none' },
          })}
        >
          <MenuTrigger>
            <ActionButton
              aria-label={t('navigation.menu')}
              isQuiet
            >
              <MenuHamburger />
            </ActionButton>
            <Menu>
              <MenuSection>
                <Header>
                  <Heading>{t('navigation.settings')}</Heading>
                </Header>
                {/* Language selection as submenu */}
                <MenuItem
                  textValue={t('navigation.language')}
                  onAction={() => setLocale(locale === 'en-US' ? 'si-LK' : 'en-US')}
                >
                  <Translate />
                  <Text slot="label">{t('navigation.language')}</Text>
                  <Text slot="value">{locale === 'en-US' ? t('language.english') : t('language.sinhala')}</Text>
                </MenuItem>
                {/* Dark mode toggle */}
                <MenuItem
                  textValue={t('theme.darkMode')}
                  onAction={toggleColorScheme}
                >
                  <Lightbulb />
                  <Text slot="label">{t('theme.darkMode')}</Text>
                  <Text slot="value">{colorScheme === 'dark' ? 'On' : 'Off'}</Text>
                </MenuItem>
              </MenuSection>
              <MenuSection>
                <MenuItem
                  textValue={t('navigation.logout')}
                  onAction={handleLogout}
                >
                  <Leave />
                  <Text slot="label">{t('navigation.logout')}</Text>
                </MenuItem>
              </MenuSection>
            </Menu>
          </MenuTrigger>
        </div>

        {/* Desktop Controls (hidden < 640px) */}
        <div
          className={style({
            display: 'none',
            sm: { display: 'flex' },
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
          paddingLeft: 16,
          paddingRight: 16,
          sm: {
            paddingLeft: 24,
            paddingRight: 24,
          },
        })}
      >
        {children}
      </div>
    </div>
  );
}