import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authService } from '../features/auth/services/AuthService';
import {
  AppProvider,
  Frame,
  Navigation,
  TopBar,
  Page,
  Box,
  BlockStack,
  Card,
  Text,
  Button,
  Badge,
  Divider,
  Link,
  Icon
} from '@shopify/polaris';

const Layout = ({ children }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const [isMobileNavOpen, setIsMobileNavOpen] = useState(false);

  const handleLogout = () => {
    authService.logout();
    navigate('/auth');
  };

  const getCurrentUserRole = () => {
    // This would ideally come from the auth service or context
    // For now, we'll determine it from the current path
    if (location.pathname.startsWith('/admin')) return 'Admin';
    if (location.pathname.startsWith('/presenter')) return 'Presenter';
    return 'Participant';
  };

  const userRole = getCurrentUserRole();

  const navigationItems = [
    {
      label: 'Dashboard',
      url: userRole === 'Admin' ? '/admin/dashboard' : 
           userRole === 'Presenter' ? '/presenter/dashboard' : '/participant/dashboard',
      icon: 'dashboard'
    }
  ];

  // Add role-specific navigation items
  if (userRole === 'Admin') {
    navigationItems.push(
      { label: 'Sessions', url: '/admin/sessions', icon: 'calendar' },
      { label: 'Users', url: '/admin/users', icon: 'users' },
      { label: 'Reports', url: '/admin/reports', icon: 'chart' }
    );
  } else if (userRole === 'Presenter') {
    navigationItems.push(
      { label: 'My Sessions', url: '/presenter/sessions', icon: 'calendar' },
      { label: 'Materials', url: '/presenter/materials', icon: 'document' }
    );
  } else {
    navigationItems.push(
      { label: 'My Sessions', url: '/participant/sessions', icon: 'calendar' },
      { label: 'Progress', url: '/participant/progress', icon: 'chart' }
    );
  }

  const navigationMarkup = (
    <Navigation location={location.pathname}>
      {navigationItems.map((item, index) => (
        <Navigation.Item
          key={index}
          label={item.label}
          url={item.url}
          selected={location.pathname === item.url}
        />
      ))}
    </Navigation>
  );

  const userMenuMarkup = (
    <TopBar.UserMenu
      actions={[
        {
          items: [
            {
              content: 'Logout',
              onAction: handleLogout,
            },
          ],
        },
      ]}
      name={userRole}
      detail={userRole}
      initials={userRole.charAt(0)}
      open={false}
    />
  );

  const topBarMarkup = (
    <TopBar
      showNavigationToggle
      userMenu={userMenuMarkup}
      secondaryMenu={null}
      searchResultsVisible={false}
      searchField={null}
      onNavigationToggle={() => setIsMobileNavOpen(!isMobileNavOpen)}
    />
  );

  return (
    <Frame
      topBar={topBarMarkup}
      navigation={navigationMarkup}
      showMobileNavigation={isMobileNavOpen}
      onNavigationDismiss={() => setIsMobileNavOpen(false)}
    >
      <Page fullWidth>
        <BlockStack gap="400">
          {children}
        </BlockStack>
      </Page>
    </Frame>
  );
};

export default Layout;