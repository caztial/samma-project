import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Heading,
  Text,
  TextField,
  Button,
  Divider,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';
import Pattern from '@react-spectrum/s2/icons/Pattern';

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

/**
 * JoinSessionPage - Mobile-first page for joining a session
 *
 * Allows participants to join a session by entering a session code
 * or by scanning a QR code (future implementation).
 */
export default function JoinSessionPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [sessionCode, setSessionCode] = useState('');
  const [isJoining, setIsJoining] = useState(false);
  const [error, setError] = useState('');

  // Handle join session by code
  const handleJoin = async () => {
    if (!sessionCode.trim()) {
      setError(t('profile.content.sessions.join.codePlaceholder'));
      return;
    }

    setIsJoining(true);
    setError('');

    try {
      // TODO: Implement actual API call to join session
      // For now, just simulate a delay
      await new Promise(resolve => setTimeout(resolve, 1000));

      // On success, navigate to session page
      // navigate(`/session/${sessionCode}`);

      // Placeholder: show success message
      console.log('Joining session:', sessionCode.toUpperCase());
    } catch (err) {
      setError(t('profile.content.sessions.join.invalidCode'));
    } finally {
      setIsJoining(false);
    }
  };

  // Handle scan QR code (placeholder)
  const handleScanQR = () => {
    // TODO: Implement QR code scanning
    console.log('Scan QR Code clicked');
  };

  // Transform input to uppercase
  const handleCodeChange = (value) => {
    setSessionCode(value.toUpperCase());
    setError('');
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
