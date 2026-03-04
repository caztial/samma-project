import { useState } from 'react';
import { Heading, ActionButton, Divider } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../i18n/useTranslation';
import MainLayout from './MainLayout';
import ProfileNavigation from '../components/profile/ProfileNavigation';
import MenuHamburger from '@react-spectrum/s2/icons/MenuHamburger';
import Close from '@react-spectrum/s2/icons/Close';

// Static styles at module level for S2 style macro compatibility
const containerStyle = style({
  display: 'flex',
  // flex shorthand is not supported by the S2 style macro — use flexGrow instead
  flexGrow: 1,
  flexDirection: 'column',
  minHeight: 0,
  backgroundColor: 'base',
});

const mobileNavButtonStyle = style({
  position: 'fixed',
  top: 64,
  left: 16,
  zIndex: 40,
});

const mobileNavOverlayStyle = style({
  position: 'fixed',
  inset: 0,
  // Use UNSAFE_style for rgba overlay color — transparent-overlay-500 is not
  // a guaranteed S2 style macro token and grey-900 is not a semantic S2 color
  zIndex: 50,
});

const mobileNavDrawerStyle = style({
  backgroundColor: 'layer-1',
  width: 280,
  height: 'full',
  padding: 16,
  overflowY: 'auto',
  boxShadow: 'elevated',
});

const mobileNavHeaderStyle = style({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: 16,
});

const mainContentStyle = style({
  flexGrow: 1,
  display: 'flex',
  flexDirection: 'column',
  minWidth: 0,
  paddingTop: 8,
  backgroundColor: 'base',
  // font and color should not be set globally on layout containers —
  // S2 components manage their own typography per the style macro guidelines
});

const navWrapperStyle = style({ marginTop: 16 });

/**
 * ProfileLayout - Layout wrapper for profile pages
 *
 * Provides mobile navigation drawer with ProfileNavigation
 * for all profile-related pages.
 */
export default function ProfileLayout({ children }) {
  const { t } = useTranslation();
  const [isMobileNavOpen, setIsMobileNavOpen] = useState(false);

  return (
    <MainLayout>
      <div className={containerStyle}>
        {/* Mobile Navigation Toggle Button */}
        <div className={mobileNavButtonStyle}>
          <ActionButton
            onPress={() => setIsMobileNavOpen(true)}
            aria-label={t('profile.nav.openMenu')}
          >
            <MenuHamburger />
          </ActionButton>
        </div>

        {/* Mobile Navigation Overlay — UNSAFE_style used for semi-transparent overlay
            because rgba background colors are not expressible as S2 style macro tokens */}
        {isMobileNavOpen && (
          <div
            className={mobileNavOverlayStyle}
            // eslint-disable-next-line react/forbid-dom-props
            style={{ backgroundColor: 'rgba(0, 0, 0, 0.5)' }}
            onClick={() => setIsMobileNavOpen(false)}
          >
            <div
              className={mobileNavDrawerStyle}
              onClick={(e) => e.stopPropagation()}
            >
              <div className={mobileNavHeaderStyle}>
                <Heading level={3}>{t('profile.title')}</Heading>
                <ActionButton
                  onPress={() => setIsMobileNavOpen(false)}
                  aria-label={t('profile.nav.closeMenu')}
                  isQuiet
                >
                  <Close />
                </ActionButton>
              </div>
              <Divider size="S" />
              <div className={navWrapperStyle}>
                <ProfileNavigation onClose={() => setIsMobileNavOpen(false)} />
              </div>
            </div>
          </div>
        )}

        {/* Main Content Area */}
        <div className={mainContentStyle}>
          {children}
        </div>
      </div>
    </MainLayout>
  );
}
