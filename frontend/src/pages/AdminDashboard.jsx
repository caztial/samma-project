import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../features/auth/services/AuthService';

const AdminDashboard = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalUsers: 0,
    activeSessions: 0,
    pendingApprovals: 0
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const userData = await authService.getCurrentUser();
        setUser(userData);
        
        // Mock stats - would come from API
        setStats({
          totalUsers: 156,
          activeSessions: 8,
          pendingApprovals: 3
        });
      } catch (error) {
        console.error('Failed to fetch data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return (
      <s-stack gap="large-300" alignItems="center">
        <s-spinner size="large" />
        <s-text>Loading dashboard...</s-text>
      </s-stack>
    );
  }

  return (
    <s-stack gap="large-400">
      {/* Welcome Section */}
      <s-card>
        <s-stack gap="base">
          <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
            <s-stack gap="small-200">
              <s-heading size="large">
                Welcome, {user?.firstName || 'Admin'}!
              </s-heading>
              <s-text color="subdued">
                Manage your meditation center operations and users.
              </s-text>
            </s-stack>
            <s-stack direction="inline" gap="small-200">
              <s-button variant="primary" onClick={() => navigate('/admin/sessions/new')}>
                Create Session
              </s-button>
              <s-button onClick={() => navigate('/admin/users')}>
                Manage Users
              </s-button>
            </s-stack>
          </s-stack>
        </s-stack>
      </s-card>

      {/* Key Metrics */}
      <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Total Users</s-text>
            <s-heading size="large">{stats.totalUsers}</s-heading>
            <s-text size="small" color="subdued">Registered participants and presenters</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Active Sessions</s-text>
            <s-heading size="large">{stats.activeSessions}</s-heading>
            <s-text size="small" color="subdued">Currently running or upcoming</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Pending Approvals</s-text>
            <s-heading size="large">{stats.pendingApprovals}</s-heading>
            <s-text size="small" color="subdued">User registrations and content</s-text>
          </s-stack>
        </s-card>
      </s-grid>

      {/* Admin Actions */}
      <s-grid gridTemplateColumns="2fr 1fr" gap="base">
        <s-card>
          <s-stack gap="base">
            <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
              <s-heading size="medium">Quick Actions</s-heading>
              <s-link href="#" onClick={(e) => { e.preventDefault(); navigate('/admin/sessions'); }}>
                View all sessions
              </s-link>
            </s-stack>

            <s-grid gridTemplateColumns="repeat(2, 1fr)" gap="base">
              <s-button variant="secondary" onClick={() => navigate('/admin/sessions/new')}>
                <s-stack gap="small-200">
                  <s-icon source="calendar" size="large" />
                  <s-text>Create Session</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/admin/users')}>
                <s-stack gap="small-200">
                  <s-icon source="users" size="large" />
                  <s-text>Manage Users</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/admin/reports')}>
                <s-stack gap="small-200">
                  <s-icon source="chart" size="large" />
                  <s-text>View Reports</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/admin/settings')}>
                <s-stack gap="small-200">
                  <s-icon source="settings" size="large" />
                  <s-text>Settings</s-text>
                </s-stack>
              </s-button>
            </s-grid>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="base">
            <s-heading size="medium">Recent Activity</s-heading>
            <s-stack gap="small-200">
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="info">New User</s-badge>
                <s-text>John Smith registered</s-text>
                <s-text color="subdued">2 hours ago</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="success">Session Created</s-badge>
                <s-text>Vipassana Retreat scheduled</s-text>
                <s-text color="subdued">4 hours ago</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="warning">Pending</s-badge>
                <s-text>Material upload pending approval</s-text>
                <s-text color="subdued">1 day ago</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="critical">System Alert</s-badge>
                <s-text>Database backup completed</s-text>
                <s-text color="subdued">2 days ago</s-text>
              </s-stack>
            </s-stack>
          </s-stack>
        </s-card>
      </s-grid>

      {/* System Status */}
      <s-card>
        <s-stack gap="base">
          <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
            <s-heading size="medium">System Status</s-heading>
            <s-badge tone="success">All systems operational</s-badge>
          </s-stack>

          <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
            <s-stack gap="small-200">
              <s-text size="small" color="subdued">Uptime</s-text>
              <s-text>99.8%</s-text>
              <s-progress-bar value={99.8} size="small" />
            </s-stack>

            <s-stack gap="small-200">
              <s-text size="small" color="subdued">Database</s-text>
              <s-text>Healthy</s-text>
              <s-progress-bar value={100} size="small" tone="success" />
            </s-stack>

            <s-stack gap="small-200">
              <s-text size="small" color="subdued">API Response</s-text>
              <s-text>120ms avg</s-text>
              <s-progress-bar value={85} size="small" tone="success" />
            </s-stack>
          </s-grid>
        </s-stack>
      </s-card>
    </s-stack>
  );
};

export default AdminDashboard;