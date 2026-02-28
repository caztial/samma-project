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
 * EducationPage - Education details management
 * Allows users to add and manage their educational background and qualifications.
 */
export default function EducationPage() {
  const { t } = useTranslation();

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <Heading level={2}>{t('profile.content.education.title')}</Heading>
        <Content>{t('profile.content.education.description')}</Content>
      </div>
    </ProfileLayout>
  );
}