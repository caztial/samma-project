import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { authService } from '../features/auth/services/AuthService';

const ParticipantDashboard = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const userData = await authService.getCurrentUser();
        setUser(userData);
      } catch (error) {
        console.error('Failed to fetch user data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchUserData();
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
                Welcome back, {user?.firstName || 'Participant'}!
              </s-heading>
              <s-text color="subdued">
                Here's what's happening with your meditation sessions.
              </s-text>
            </s-stack>
            <s-stack direction="inline" gap="small-200">
              <s-button variant="primary" onClick={() => navigate('/participant/sessions')}>
                View Sessions
              </s-button>
              <s-button onClick={() => navigate('/participant/progress')}>
                View Progress
              </s-button>
            </s-stack>
          </s-stack>
        </s-stack>
      </s-card>

      {/* Quick Stats */}
      <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Upcoming Sessions</s-text>
            <s-heading size="large">3</s-heading>
            <s-text size="small" color="subdued">Next session: Tomorrow at 6:00 PM</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Completed Sessions</s-text>
            <s-heading size="large">12</s-heading>
            <s-text size="small" color="subdued">This month</s-text>
          </s-stack>
        </s-card>

        <s-card>
          <s-stack gap="small-200">
            <s-text size="small" color="subdued">Total Hours</s-text>
            <s-heading size="large">24.5</s-heading>
            <s-text size="small" color="subdued">Meditation time</s-text>
          </s-stack>
        </s-card>
      </s-grid>

      {/* Recent Activity */}
      <s-card>
        <s-stack gap="base">
          <s-stack direction="inline" gap="large-400" alignItems="center" justifyContent="space-between">
            <s-heading size="medium">Recent Activity</s-heading>
            <s-link href="#" onClick={(e) => { e.preventDefault(); navigate('/participant/sessions'); }}>
              View all
            </s-link>
          </s-stack>

          <s-stack gap="small-200">
            <s-stack direction="inline" gap="small-200" alignItems="center">
              <s-badge tone="success">Completed</s-badge>
              <s-text>Evening Meditation - 1 hour</s-text>
              <s-text color="subdued">2 days ago</s-text>
            </s-stack>

            <s-stack direction="inline" gap="small-200" alignItems="center">
              <s-badge tone="warning">Upcoming</s-badge>
              <s-text>Morning Chanting - 30 min</s-text>
              <s-text color="subdued">Tomorrow at 6:00 AM</s-text>
            </s-stack>

            <s-stack direction="inline" gap="small-200" alignItems="center">
              <s-badge tone="info">In Progress</s-badge>
              <s-text>Vipassana Retreat - Day 3</s-text>
              <s-text color="subdued">Started yesterday</s-text>
            </s-stack>
          </s-stack>
        </s-stack>
      </s-card>

      {/* Quick Actions */}
      <s-card>
        <s-stack gap="base">
          <s-heading size="medium">Quick Actions</s-heading>
          <s-grid gridTemplateColumns="repeat(3, 1fr)" gap="base">
            <s-button variant="secondary" onClick={() => navigate('/participant/sessions')}>
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-icon source="calendar" />
                <s-text>Book Session</s-text>
              </s-stack>
            </s-button>

            <s-button variant="secondary" onClick={() => navigate('/participant/progress')}>
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-icon source="chart" />
                <s-text>View Progress</s-text>
              </s-stack>
            </s-button>

            <s-button variant="secondary" onClick={() => navigate('/participant/materials')}>
              <s-stack direction="inline" gap="small-200" alignItems="center">
                <s-icon source="document" />
                <s-text>Download Materials</s-text>
              </s-stack>
            </s-button>
          </s-grid>
        </s-stack>
      </s-card>
    </s-stack>
  );
};

export default ParticipantDashboard;