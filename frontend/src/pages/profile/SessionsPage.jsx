import { Heading, Content } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';

const containerStyle = style({
  padding: 16,
  display: 'flex',
  flexDirection: 'column',
  gap: 16,
});

/**
 * SessionsPage - Session history and progress
 * Allows users to view their session history and track their learning progress.
 */
export default function SessionsPage() {
  const { t } = useTranslation();

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <Heading level={2}>{t('profile.content.sessions.title')}</Heading>
        <Content>{t('profile.content.sessions.description')}</Content>
      </div>
    </ProfileLayout>
  );
}