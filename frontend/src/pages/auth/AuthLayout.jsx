import { Picker, PickerItem, Switch } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import { useLocale } from '../../i18n/LocaleContext';
import { useTheme } from '../../contexts/ThemeContext';

/**
 * Shared layout for auth pages.
 * Renders a top bar with language + color scheme selectors,
 * and a vertically/horizontally centered content area.
 */
export default function AuthLayout({ children }) {
  const { t } = useTranslation();
  const { locale, setLocale } = useLocale();
  const { colorScheme, toggleColorScheme } = useTheme();

  return (
    <div
      className={style({
        display: 'flex',
        flexDirection: 'column',
        minHeight: '100vh',
      })}
    >
      {/* ── Top bar ── */}
      <div
        className={style({
          display: 'flex',
          justifyContent: 'end',
          alignItems: 'center',
          paddingTop: 12,
          paddingBottom: 12,
          paddingLeft: 24,
          paddingRight: 24,
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
      </div>

      {/* ── Centered content ── */}
      <div
        className={style({
          flex: 1,
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          paddingTop: 24,
          paddingBottom: 48,
          paddingLeft: 24,
          paddingRight: 24,
        })}
      >
        {children}
      </div>
    </div>
  );
}
