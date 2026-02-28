import { Heading, Content } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import ProfileLayout from '../../layouts/ProfileLayout';

const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  gap: 16
});

/**
 * ProfileOverviewPage - User profile overview
 * Displays basic user information, contact details, and preferences.
 */
export default function ProfileOverviewPage() {
  const { t } = useTranslation();

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <Heading level={2}>{t('profile.content.profile.title')}</Heading>
        <Content>{t('profile.content.profile.description')}</Content>
      </div>
    </ProfileLayout>
  );
}
