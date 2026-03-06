import { useState } from 'react';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import MainLayout from './MainLayout';
import AdminNavigation from '../components/admin/AdminNavigation';

// Static styles at module level for S2 style macro compatibility
const containerStyle = style({
  display: 'flex',
  flexGrow: 1,
  minHeight: 0,
  backgroundColor: 'base',
});

const mainContentExpandedStyle = style({
  flexGrow: 1,
  display: 'flex',
  flexDirection: 'column',
  minWidth: 0,
  padding: 24,
  overflowY: 'auto',
  marginLeft: 240,
  transition: 'default',
});

const mainContentCollapsedStyle = style({
  flexGrow: 1,
  display: 'flex',
  flexDirection: 'column',
  minWidth: 0,
  padding: 24,
  overflowY: 'auto',
  marginLeft: 64,
  transition: 'default',
});

/**
 * AdminLayout - Layout wrapper for admin pages
 *
 * Provides a sticky, collapsible sidebar navigation
 * for all admin-related pages.
 */
import { Picker, PickerItem, Switch, Button } from '@react-spectrum/s2';
import { useTranslation } from '../i18n/useTranslation';
import { useLocale } from '../i18n/LocaleContext';
import { useTheme } from '../contexts/ThemeContext';
import { useAuth } from '../contexts/AuthContext';

export default function AdminLayout({ children }) {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const { t } = useTranslation();
  const { locale, setLocale } = useLocale();
  const { colorScheme, toggleColorScheme } = useTheme();
  const { logout } = useAuth();

  return (
    <div className={containerStyle}>
      {/* Sidebar navigation */}
      <AdminNavigation
        isCollapsed={isCollapsed}
        onToggleCollapse={() => setIsCollapsed(!isCollapsed)}
      />

      {/* Main content area */}
      <div className={isCollapsed ? mainContentCollapsedStyle : mainContentExpandedStyle}>
        {/* Top bar with controls */}
        <header className={style({ display: 'flex', justifyContent: 'end', alignItems: 'center', gap: 16, padding: 16, borderBottomWidth: 1, borderBottomStyle: 'solid', borderColor: 'gray-200' })}>
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
          <Button variant="secondary" size="S" onPress={logout}>
            {t('navigation.logout')}
          </Button>
        </header>
        
        <main className={style({ flexGrow: 1, padding: 24, overflowY: 'auto' })}>
          {children}
        </main>
      </div>
    </div>
  );
}
