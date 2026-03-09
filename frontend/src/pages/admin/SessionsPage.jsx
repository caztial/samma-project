import { Heading, Content, IllustratedMessage } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import AdminLayout from '../../layouts/AdminLayout';

const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  flexGrow: 1,
});

const headerStyle = style({
  marginBottom: 16,
});

const titleStyle = style({
  font: 'heading',
  color: 'heading',
  marginBottom: 4,
});

const subtitleStyle = style({
  font: 'body-sm',
  color: 'neutral-subdued',
});

/**
 * SessionsPage - Admin session management
 */
export default function SessionsPage() {
  const { t } = useTranslation();

  return (
    <AdminLayout>
      <div className={containerStyle}>
        <div className={headerStyle}>
          <h1 className={titleStyle}>{t('admin.sessions.title')}</h1>
          <p className={subtitleStyle}>{t('admin.sessions.subtitle')}</p>
        </div>

        <IllustratedMessage>
          <Heading>{t('admin.comingSoon')}</Heading>
          <Content>{t('admin.sessions.description')}</Content>
        </IllustratedMessage>
      </div>
    </AdminLayout>
  );
}