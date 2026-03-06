import { NavLink } from 'react-router-dom';
import { ActionButton, Divider, Text } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import { useAuth } from '../../contexts/AuthContext';
import Home from '@react-spectrum/s2/icons/Home';
import Calendar from '@react-spectrum/s2/icons/Calendar';
import Question from '@react-spectrum/s2/icons/HelpCircle';
import User from '@react-spectrum/s2/icons/User';
import ChevronLeft from '@react-spectrum/s2/icons/ChevronLeft';
import ChevronRight from '@react-spectrum/s2/icons/ChevronRight';

// Static styles at module level for S2 style macro compatibility
const navContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  height: '100vh',
  backgroundColor: 'layer-1',
  borderRightWidth: 1,
  borderRightStyle: 'solid',
  borderColor: 'gray-200',
  transition: 'default',
  zIndex: 30,
});

const navContainerCollapsedStyle = style({
  width: 64,
});

const navContainerExpandedStyle = style({
  width: 240,
});

const greetingSectionStyle = style({
  display: 'flex',
  flexDirection: 'column',
  paddingX: 12,
  paddingY: 16,
  borderBottomWidth: 1,
  borderBottomStyle: 'solid',
  borderColor: 'gray-200',
  font: 'body',
});

const greetingTextStyle = style({
  color: 'neutral-subdued',
});

const userNameStyle = style({
  whiteSpace: 'nowrap',
  overflow: 'hidden',
  textOverflow: 'ellipsis',
});

const navItemsStyle = style({
  display: 'flex',
  flexDirection: 'column',
  flexGrow: 1,
  paddingX: 8,
  paddingTop: 8,
  gap: 4,
  overflowY: 'auto',
});

const navItemStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 12,
  paddingX: 12,
  paddingY: 8,
  borderRadius: 'default',
  cursor: 'pointer',
  transition: 'default',
  textDecoration: 'none',
  color: 'neutral',
  font: 'body',
  whiteSpace: 'nowrap',
  overflow: 'hidden',
});

const navItemActiveStyle = style({
  backgroundColor: 'accent-subtle',
  color: 'accent',
  font: 'body',
  fontWeight: 'medium',
});

const navItemLabelStyle = style({
  overflow: 'hidden',
  textOverflow: 'ellipsis',
});

const collapseButtonContainerStyle = style({
  paddingX: 8,
  paddingY: 12,
  borderTopWidth: 1,
  borderTopStyle: 'solid',
  borderColor: 'gray-200',
  display: 'flex',
  flexDirection: 'column',
  gap: 8,
});

const collapseButtonStyle = style({
  width: 'full',
  justifyContent: 'center',
});

/**
 * Navigation item configuration
 */
const navItems = [
  { key: 'dashboard', path: '/admin', icon: Home, exactMatch: true },
  { key: 'sessions', path: '/admin/sessions', icon: Calendar },
  { key: 'questions', path: '/admin/questions', icon: Question },
  { key: 'users', path: '/admin/users', icon: User },
];

/**
 * AdminNavigation - Collapsible sidebar navigation for admin panel
 *
 * @param {Object} props
 * @param {boolean} props.isCollapsed - Whether the sidebar is collapsed
 * @param {function} props.onToggleCollapse - Callback to toggle collapsed state
 */
export default function AdminNavigation({ isCollapsed, onToggleCollapse }) {
  const { t } = useTranslation();
  const { user } = useAuth();

  const userDisplayName = user?.firstName
    ? `${user.firstName}${user.lastName ? ` ${user.lastName}` : ''}`
    : user?.email || 'User';

  return (
    <nav
      className={`${navContainerStyle} ${isCollapsed ? navContainerCollapsedStyle : navContainerExpandedStyle}`}
      aria-label={t('admin.nav.ariaLabel')}
    >
      {/* Greeting section */}
      <div className={greetingSectionStyle}>
        <Text className={greetingTextStyle}>{t('navigation.greeting', { name: '' })}</Text>
        {!isCollapsed && (
          <Text className={userNameStyle}>{userDisplayName}</Text>
        )}
      </div>

      {/* Navigation items */}
      <div className={navItemsStyle}>
        {navItems.map(({ key, path, icon: Icon, exactMatch }) => (
          <NavLink
            key={key}
            to={path}
            end={exactMatch}
            className={({ isActive }) => {
              const activeStyles = isActive ? navItemActiveStyle : '';
              return `${navItemStyle} ${activeStyles}`;
            }}
          >
            <Icon />
            {!isCollapsed && (
              <span className={navItemLabelStyle}>
                {t(`admin.nav.${key}`)}
              </span>
            )}
          </NavLink>
        ))}
      </div>

      <Divider size="S" />

      {/* Collapse button */}
      <div className={collapseButtonContainerStyle}>
        <ActionButton
          isQuiet
          onPress={onToggleCollapse}
          aria-label={
            isCollapsed
              ? t('admin.nav.expandSidebar')
              : t('admin.nav.collapseSidebar')
          }
          styles={collapseButtonStyle}
        >
          {isCollapsed ? <ChevronRight /> : <ChevronLeft />}
          {!isCollapsed && <span>{t('admin.nav.collapse')}</span>}
        </ActionButton>
      </div>
    </nav>
  );
}
