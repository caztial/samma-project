import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Disclosure, DisclosureTitle, DisclosurePanel, Text } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import User from '@react-spectrum/s2/icons/User';
import Calendar from '@react-spectrum/s2/icons/Calendar';
import ChevronRight from '@react-spectrum/s2/icons/ChevronRight';
import People from '@react-spectrum/s2/icons/People';

// Static styles for all combinations - must be static for style macro
const baseNavItemStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  width: 'full',
  padding: 8,
  paddingLeft: 12,
  paddingRight: 12,
  borderRadius: 'default',
  borderStyle: 'none',
  cursor: 'pointer',
  transition: 'default',
  textAlign: 'start',
  hover: {
    backgroundColor: 'neutral-subtle',
  },
});

const indentedNavItemStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  width: 'full',
  padding: 8,
  paddingLeft: 40,
  paddingRight: 12,
  borderRadius: 'default',
  borderStyle: 'none',
  cursor: 'pointer',
  transition: 'default',
  textAlign: 'start',
  hover: {
    backgroundColor: 'neutral-subtle',
  },
});

const selectedStyle = style({
  backgroundColor: 'neutral-subtle',
});

const unselectedStyle = style({
  backgroundColor: 'transparent',
});

const iconStyle = style({ width: 16, height: 16 });

/**
 * Navigation item component for the profile sidebar
 */
function NavItem({ icon, label, isSelected, onPress, indent = false }) {
  // Combine static styles using template literals
  const className = `${indent ? indentedNavItemStyle : baseNavItemStyle} ${isSelected ? selectedStyle : unselectedStyle}`;

  return (
    <button
      onClick={onPress}
      className={className}
    >
      {icon}
      <Text>{label}</Text>
    </button>
  );
}

/**
 * Profile Navigation Component
 * 
 * A left sidebar navigation with collapsible sections for profile settings.
 * Uses React Router for navigation between pages.
 */
export default function ProfileNavigation({ onClose }) {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const location = useLocation();
  const [isProfileExpanded, setIsProfileExpanded] = useState(true);
  const [isSessionsExpanded, setIsSessionsExpanded] = useState(true);

  // Determine which item is selected based on current route
  const currentPath = location.pathname;
  const isProfileSelected = currentPath === '/profile' || currentPath === '/profile/overview';
  const isJoinSessionSelected = currentPath === '/profile/sessions/join';
  const isSessionsSelected = currentPath === '/profile/sessions' || isJoinSessionSelected;

  // Navigation handlers
  const handleNavigate = (path) => {
    navigate(path);
    onClose?.();
  };

  return (
    <nav
      className={style({
        display: 'flex',
        flexDirection: 'column',
        gap: 4,
        width: 'full',
        minWidth: 220,
      })}
    >
      {/* Profile Section (expandable) */}
      <Disclosure
        isExpanded={isProfileExpanded}
        onExpandedChange={setIsProfileExpanded}
        isQuiet
      >
        <DisclosureTitle>
          <div
            className={style({
              display: 'flex',
              alignItems: 'center',
              gap: 12,
            })}
          >
            <User />
            <Text>{t('profile.nav.profile')}</Text>
          </div>
        </DisclosureTitle>
        <DisclosurePanel>
          <div className={style({ display: 'flex', flexDirection: 'column', gap: 2, marginTop: 4 })}>
            {/* Profile Overview */}
            <NavItem
              icon={<ChevronRight className={iconStyle} />}
              label={t('profile.nav.profileOverview')}
              isSelected={isProfileSelected}
              onPress={() => handleNavigate('/profile/overview')}
              indent
            />
          </div>
        </DisclosurePanel>
      </Disclosure>

      {/* Sessions Section (expandable) */}
      <Disclosure
        isExpanded={isSessionsExpanded}
        onExpandedChange={setIsSessionsExpanded}
        isQuiet
      >
        <DisclosureTitle>
          <div
            className={style({
              display: 'flex',
              alignItems: 'center',
              gap: 12,
            })}
          >
            <Calendar />
            <Text>{t('profile.nav.sessions')}</Text>
          </div>
        </DisclosureTitle>
        <DisclosurePanel>
          <div className={style({ display: 'flex', flexDirection: 'column', gap: 2, marginTop: 4 })}>
            {/* Join Session */}
            <NavItem
              icon={<People className={iconStyle} />}
              label={t('profile.nav.joinSession')}
              isSelected={isJoinSessionSelected}
              onPress={() => handleNavigate('/profile/sessions/join')}
              indent
            />
          </div>
        </DisclosurePanel>
      </Disclosure>
    </nav>
  );
}