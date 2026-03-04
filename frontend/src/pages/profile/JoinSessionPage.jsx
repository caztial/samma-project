import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Heading,
  Text,
  TextField,
  Button,
  Divider,
  ToastQueue,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';
import Pattern from '@react-spectrum/s2/icons/Pattern';
import { useAuth } from '../../contexts/AuthContext';
import { createSessionService } from '../../services/sessionService';
import {
  saveCurrentSession,
  getCurrentSession,
  isSessionValid,
} from '../../services/sessionStorage';

const containerStyle = style({
  padding: 16,
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  minHeight: '80vh',
});

// Plain div styled to look like a card — Card component is for selectable/navigable objects with slots
const cardStyle = style({
  width: 'full',
  maxWidth: 400,
  backgroundColor: 'layer-1',
  borderRadius: 'lg',
  boxShadow: 'elevated',
  display: 'flex',
  flexDirection: 'column',
  gap: 16,
  alignItems: 'center',
  paddingX: 24,
  paddingY: 20,
});

const headingStyle = style({
  textAlign: 'center',
  font: 'heading',
});

const subtitleStyle = style({
  font: 'body-sm',
  textAlign: 'center',
  color: 'neutral-subdued',
});

const dividerContainerStyle = style({
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  gap: 16,
  width: 'full',
  paddingY: 16,
  font: 'body-sm',
});

const orTextStyle = style({
  paddingX: 12,
  color: 'neutral-subdued',
});

const inputContainerStyle = style({
  width: 'full',
  paddingTop: 8,
});

const joinButtonStyle = style({
  width: 'full',
});

const scanQrButtonStyle = style({
  width: 'full',
  padding: 20,
});

const textFieldStyle = style({
  width: 'full',
});

const rejoinButtonStyle = style({
  width: 'full',
  marginTop: 8,
});

/**
 * JoinSessionPage - Mobile-first page for joining a session
 *
 * Allows participants to join a session by entering a session code
 * or by scanning a QR code (future implementation).
 */
export default function JoinSessionPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { getToken, onUnauthorized } = useAuth();
  const [sessionCode, setSessionCode] = useState('');
  const [isJoining, setIsJoining] = useState(false);
  const [error, setError] = useState('');
  const [currentSession, setCurrentSession] = useState(null);

  // Initialize session service
  const sessionService = createSessionService({ getToken, onUnauthorized });

  // Check for existing valid session on component mount
  useEffect(() => {
    const validateExistingSession = async () => {
      const existingSession = getCurrentSession();
      if (!existingSession) return;

      try {
        // Use the enhanced isSessionValid that calls backend
        const isValid = await isSessionValid(existingSession, sessionService);

        if (isValid) {
          // Session is still active, show rejoin button
          setCurrentSession(existingSession);
        } else {
          // Session was cleared or not active, don't show rejoin button
          setCurrentSession(null);
        }
      } catch (error) {
        // Keep existing session for manual rejoin
        setCurrentSession(existingSession);
      }
    };

    validateExistingSession();
  }, []);

  // Handle join session by code
  const handleJoin = async () => {
    if (!sessionCode.trim()) {
      setError(t('profile.content.sessions.join.codePlaceholder'));
      return;
    }

    setIsJoining(true);
    setError('');

    try {
      // Convert session code to lowercase before sending to API
      const lowercaseCode = sessionCode.trim().toLowerCase();

      // Join the session using the code
      const participantResponse = await sessionService.joinSession(lowercaseCode);

      // Get session details using the participant's session ID
      const sessionResponse = await sessionService.getSession(participantResponse.sessionId);

      // Save complete session data to localStorage
      const sessionData = {
        sessionId: participantResponse.sessionId,
        participantId: participantResponse.id,
        sessionCode: sessionResponse.code,
        sessionName: sessionResponse.name,
        sessionState: sessionResponse.state,
        userName: participantResponse.userName,
        joinedAt: participantResponse.joinedAt,
        savedAt: new Date().toISOString()
      };

      saveCurrentSession(sessionData);

      // Show success toast with session details
      ToastQueue.positive(t('profile.content.sessions.join.success', {
        sessionName: sessionData.sessionName
      }), {
        timeout: 2000,
        actionLabel: t('profile.content.sessions.join.viewSession')
      });

      // Navigate to session detail page after a short delay
      setTimeout(() => {
        //navigate(`/profile/sessions/${participantResponse.sessionId}`);
      }, 2000);

    } catch (error) {
      const errorMessage = getErrorMessage(error);
      setError(errorMessage);
      ToastQueue.negative(errorMessage, {
        timeout: 8000
      });
    } finally {
      setIsJoining(false);
    }
  };

  // Handle scan QR code (placeholder)
  const handleScanQR = () => {
    // TODO: Implement QR code scanning
    console.log('Scan QR Code clicked');
  };

  // Handle rejoin existing session
  const handleRejoin = () => {
    if (currentSession) {
      navigate(`/profile/sessions/${currentSession.sessionId}`);
    }
  };

  // Transform input to lowercase
  const handleCodeChange = (value) => {
    setSessionCode(value.toLowerCase());
    setError('');
  };

  // Truncate session name to max 25 characters
  const truncateSessionName = (name, maxLength = 25) => {
    if (!name) return '';
    if (name.length <= maxLength) return name;
    return name.substring(0, maxLength) + '...';
  };

  // Get appropriate error message based on API response
  const getErrorMessage = (error) => {
    if (error.response?.status === 404) {
      return t('profile.content.sessions.join.errors.notFound');
    } else if (error.response?.status === 409) {
      return t('profile.content.sessions.join.errors.alreadyJoined');
    } else if (error.response?.status === 410) {
      return t('profile.content.sessions.join.errors.ended');
    } else if (error.response?.status === 429) {
      return t('profile.content.sessions.join.errors.full');
    } else if (error.response?.status === 400) {
      return error.response.data?.error || t('profile.content.sessions.join.errors.unableToJoin');
    } else if (error.response?.status === 401) {
      return t('profile.content.sessions.join.errors.unauthorized');
    } else {
      return t('profile.content.sessions.join.errors.unableToJoin');
    }
  };

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <div className={cardStyle}>
          {/* Heading */}
          <Heading level={2} styles={headingStyle}>
            {t('profile.content.sessions.join.title')}
          </Heading>

          {/* Subheading */}
          <Text styles={subtitleStyle}>
            {t('profile.content.sessions.join.subtitle')}
          </Text>

          {/* Rejoin Button - Shows when valid session exists */}
          {currentSession && (
            <Button
              variant="accent"
              onPress={handleRejoin}
              styles={rejoinButtonStyle}
            >
              <Text>{t('profile.content.sessions.join.rejoinSession', { sessionName: truncateSessionName(currentSession.sessionName) })}</Text>
            </Button>
          )}

          {/* Session Code Input */}
          <div className={inputContainerStyle}>
            <TextField
              value={sessionCode}
              onChange={handleCodeChange}
              placeholder={t('profile.content.sessions.join.codePlaceholder')}
              errorMessage={error}
              isInvalid={!!error}
              isDisabled={isJoining}
              autoFocus
              styles={textFieldStyle}
            />
          </div>

          {/* Join Button */}
          <Button
            variant="primary"
            onPress={handleJoin}
            isDisabled={isJoining || !sessionCode.trim()}
            styles={joinButtonStyle}
          >
            {isJoining
              ? t('profile.content.sessions.join.joining')
              : t('profile.content.sessions.join.joinButton')
            }
          </Button>

          {/* Divider with "or" */}
          <div className={dividerContainerStyle}>
            <Divider size="S" />
            <Text styles={orTextStyle}>{t('common.or')}</Text>
            <Divider size="S" />
          </div>

          {/* Scan QR Button */}
          <Button
            variant="secondary"
            onPress={handleScanQR}
            styles={scanQrButtonStyle}
          >
            <Pattern />
            <Text>{t('profile.content.sessions.join.scanQR')}</Text>
          </Button>
        </div>
      </div>
    </ProfileLayout>
  );
}
