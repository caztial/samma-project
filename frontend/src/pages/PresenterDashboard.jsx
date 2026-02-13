import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../features/auth/services/AuthService';

const PresenterDashboard = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState({
    totalSessions: 0,
    upcomingSessions: 0,
    totalParticipants: 0
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        const userData = await authService.getCurrentUser();
        setUser(userData);
        
        // Mock stats - would come from API
        setStats({
          totalSessions: 12,
          upcomingSessions: 3,
          totalParticipants: 89
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
                Welcome, {user?.firstName || 'Presenter'}!
              </s-heading>
              <s-text color="subdued">
                Manage your meditation sessions and materials.
              </s-text>
            </s-stack>
            <s-stack direction="inline" gap="small-200">
              <s-button variant="primary" onClick={() => navigate('/presenter/sessions/new')}>
                Create Session
              </s-button>
              <s-button onClick={() => navigate('/presenter/materials')}>
                Manage Materials
              </s-button>
            </s-stack>
          </s-stack>
        </s-stack>
      </s-card>

      {/* Key Metrics */}
      <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Total Sessions</s-text>
            <s-heading size="large">{stats.totalSessions}</s-heading>
            <s-text size="small" color="subdued">Created and managed</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Upcoming Sessions</s-text>
            <s-heading size="large">{stats.upcomingSessions}</s-heading>
            <s-text size="small" color="subdued">Next 30 days</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Total Participants</s-text>
            <s-heading size="large">{stats.totalParticipants}</s-heading>
            <s-text size="small" color="subdued">Across all sessions</s-text>
          </s-stack>
        </s-card>
      </s-grid>

      {/* Presenter Actions */}
      <s-grid gridTemplateColumns="2fr 1fr" gap="base">
        <s-card>
          <s-stack gap="base">
            <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
              <s-heading size="medium">Quick Actions</s-heading>
              <s-link href="#" onClick={(e) => { e.preventDefault(); navigate('/presenter/sessions'); }}>
                View all sessions
              </s-link>
            </s-stack>

            <s-grid gridTemplateColumns="repeat(2, 1fr)" gap="base">
              <s-button variant="secondary" onClick={() => navigate('/presenter/sessions/new')}>
                <s-stack gap="small-200">
                  <s-icon source="calendar" size="large" />
                  <s-text>Create Session</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/presenter/materials')}>
                <s-stack gap="small-200">
                  <s-icon source="document" size="large" />
                  <s-text>Upload Materials</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/presenter/analytics')}>
                <s-stack gap="small-200">
                  <s-icon source="chart" size="large" />
                  <s-text>View Analytics</s-text>
                </s-stack>
              </s-button>

              <s-button variant="secondary" onClick={() => navigate('/presenter/profile')}>
                <s-stack gap="small-200">
                  <s-icon source="user" size="large" />
                  <s-text>Profile Settings</s-text>
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
                <s-badge tone="success">Session Completed</s-badge>
                <s-text>Morning Meditation - 25 attendees</s-text>
                <s-text color="subdued">Yesterday</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="info">New Material</s-badge>
                <s-text>Vipassana guide uploaded</s-text>
                <s-text color="subdued">2 days ago</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="warning">Reminder</s-badge>
                <s-text>Evening session tomorrow at 6 PM</s-text>
                <s-text color="subdued">3 days ago</s-text>
              </s-stack>

              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="info">Feedback</s-badge>
                <s-text>Received 5 new session reviews</s-text>
                <s-text color="subdued">1 week ago</s-text>
              </s-stack>
            </s-stack>
          </s-stack>
        </s-card>
      </s-grid>

      {/* Upcoming Sessions */}
      <s-card>
        <s-stack gap="base">
          <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
            <s-heading size="medium">Upcoming Sessions</s-heading>
            <s-link href="#" onClick={(e) => { e.preventDefault(); navigate('/presenter/sessions'); }}>
              View all
            </s-link>
          </s-stack>

          <s-stack gap="small-200">
            <s-stack direction="inline" gap="small-200" alignItems="center" justifyContent="space-between">
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="info">Morning</s-badge>
                <s-text>Guided Meditation</s-text>
              </s-stack>
              <s-text color="subdued">Tomorrow • 6:00 AM</s-text>
            </s-stack>

            <s-stack direction="inline" gap="small-200" alignItems="center" justifyContent="space-between">
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="success">Evening</s-badge>
                <s-text>Vipassana Practice</s-text>
              </s-stack>
              <s-text color="subdued">Feb 15 • 6:00 PM</s-text>
            </s-stack>

            <s-stack direction="inline" gap="small-200" alignItems="center" justifyContent="space-between">
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-badge tone="warning">Weekend</s-badge>
                <s-text>Retreat Session</s-text>
              </s-stack>
              <s-text color="subdued">Feb 16 • 9:00 AM</s-text>
            </s-stack>
          </s-stack>
        </s-stack>
      </s-card>

      {/* Performance Summary */}
      <s-card>
        <s-stack gap="base">
          <s-heading size="medium">Performance Summary</s-heading>
          <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
            <s-stack gap="small-200">
              <s-text size="small" color="subdued">Average Rating</s-text>
              <s-heading size="large">4.8</s-heading>
              <s-text size="small" color="subdued">Based on 156 reviews</s-text>
            </s-stack>

            <s-stack gap="small-200">
              <s-text size="small" color="subdued">Attendance Rate</s-text>
              <s-heading size="large">87%</s-heading>
              <s-text size="small" color="subdued">Of registered participants</s-text>
            </s-stack>

            <s-stack gap="small-200">
              <s-text size="small" color="subdued">Materials Downloaded</s-text>
              <s-heading size="large">234</s-heading>
              <s-text size="small" color="subdued">This month</s-text>
            </s-stack>
          </s-grid>
        </s-stack>
      </s-card>
    </s-stack>
  );
};

export default PresenterDashboard;