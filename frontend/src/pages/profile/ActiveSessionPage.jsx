import { useState, useEffect, useRef } from 'react';
import { useParams } from 'react-router-dom';
import {
  Heading,
  Text,
  Badge,
  IllustratedMessage,
  ProgressCircle,
  ToastQueue,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';
import { useAuth } from '../../contexts/AuthContext';
import { createSessionService } from '../../services/sessionService';
import { getCurrentSession } from '../../services/sessionStorage';
import Location from '@react-spectrum/s2/icons/Location';
import Clock from '@react-spectrum/s2/icons/Clock';

// Static styles at module level for S2 style macro compatibility
const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  flexGrow: 1,
  padding: 16,
  minWidth: 0,
});

const headerRowStyle = style({
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center',
  marginBottom: 8,
  gap: 12,
});

const sessionNameStyle = style({
  // Heading component handles font styling
});

const infoRowStyle = style({
  display: 'flex',
  flexWrap: 'wrap',
  gap: 16,
  marginBottom: 24,
  color: 'neutral-subdued',
  font: 'body-sm',
});

const infoItemStyle = style({
  display: 'flex',
  alignItems: 'center',
  gap: 4,
});

const questionsAreaStyle = style({
  display: 'flex',
  flexGrow: 1,
  justifyContent: 'center',
  alignItems: 'center',
  minHeight: 300,
});

const loadingContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  flexGrow: 1,
  gap: 16,
});

const errorContainerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  flexGrow: 1,
  gap: 16,
  padding: 32,
});

/**
 * Format a date to a readable time string
 * @param {string} dateString - ISO date string
 * @returns {string} Formatted time string
 */
function formatStartedTime(dateString) {
  if (!dateString) return '';

  const date = new Date(dateString);
  const now = new Date();
  const isToday = date.toDateString() === now.toDateString();

  if (isToday) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  return date.toLocaleDateString([], {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
}

/**
 * ActiveSessionPage - Page for active session participation
 *
 * Displays session info, connection status, and active questions.
 */
export default function ActiveSessionPage() {
  const { sessionId } = useParams();
  const { t } = useTranslation();
  const { getToken, onUnauthorized } = useAuth();

  // Session state
  const [session, setSession] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  // Connection status (static for now - SignalR integration later)
  const [connectionStatus] = useState('connected'); // 'connected' | 'disconnected' | 'connecting'

  // Use ref to store session service to avoid re-creation
  const sessionServiceRef = useRef(null);

  // Initialize session service only once
  if (!sessionServiceRef.current) {
    sessionServiceRef.current = createSessionService({
      getToken: () => getToken(),
      onUnauthorized
    });
  }

  // Fetch session data on mount
  useEffect(() => {
    let isMounted = true;

    const fetchSession = async () => {
      setIsLoading(true);
      setError(null);

      try {
        // First try to get from localStorage (faster)
        const cachedSession = getCurrentSession();

        if (cachedSession && cachedSession.sessionId === sessionId) {
          // Use cached data but still fetch fresh data from API
          if (isMounted) {
            setSession({
              id: cachedSession.sessionId,
              name: cachedSession.sessionName,
              code: cachedSession.sessionCode,
              state: cachedSession.sessionState,
              startedAt: cachedSession.joinedAt,
            });
          }
        }

        // Fetch fresh data from API
        const response = await sessionServiceRef.current.getSession(sessionId);

        if (isMounted) {
          setSession(response);
        }
      } catch (err) {
        console.error('Failed to fetch session:', err);

        if (isMounted) {
          if (err.response?.status === 404) {
            setError(t('profile.content.sessions.detail.notFound'));
          } else {
            setError(t('profile.content.sessions.detail.loadError'));
          }

          ToastQueue.negative(t('profile.content.sessions.detail.loadError'), {
            timeout: 5000
          });
        }
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    if (sessionId) {
      fetchSession();
    }

    return () => {
      isMounted = false;
    };
  }, [sessionId]);

  // Loading state
  if (isLoading) {
    return (
      <ProfileLayout>
        <div className={loadingContainerStyle}>
          <ProgressCircle size="L" isIndeterminate />
          <Text>{t('common.loading')}</Text>
        </div>
      </ProfileLayout>
    );
  }

  // Error state
  if (error || !session) {
    return (
      <ProfileLayout>
        <div className={errorContainerStyle}>
          <IllustratedMessage>
            <Text>{error || t('profile.content.sessions.detail.notFound')}</Text>
          </IllustratedMessage>
        </div>
      </ProfileLayout>
    );
  }

  // Get badge variant based on connection status
  const getBadgeVariant = () => {
    switch (connectionStatus) {
      case 'connected':
        return 'positive';
      case 'connecting':
        return 'informative';
      case 'disconnected':
      default:
        return 'negative';
    }
  };

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        {/* Header Row: Session Name + Connection Status */}
        <div className={headerRowStyle}>
          <Heading level={3} styles={sessionNameStyle}>
            {session.name}
          </Heading>
          <Badge size='S' variant={getBadgeVariant()}>
            {t(`profile.content.sessions.active.connectionStatus.${connectionStatus}`)}
          </Badge>
        </div>

        {/* Info Row: Location + Started Time */}
        <div className={infoRowStyle}>
          {session.location && (
            <div className={infoItemStyle}>
              <Location />
              <Text>{session.location}</Text>
            </div>
          )}
          {session.startedAt && (
            <div className={infoItemStyle}>
              <Clock />
              <Text>
                {t('profile.content.sessions.active.startedAt', {
                  time: formatStartedTime(session.startedAt)
                })}
              </Text>
            </div>
          )}
        </div>

        {/* Questions Area - Placeholder for future implementation */}
        <div className={questionsAreaStyle}>
          <IllustratedMessage>
            <Text>{t('profile.content.sessions.active.waitingForQuestion')}</Text>
          </IllustratedMessage>
        </div>
      </div>
    </ProfileLayout>
  );
}