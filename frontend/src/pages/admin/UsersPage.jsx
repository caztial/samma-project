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
  marginBottom: 24,
});

const titleStyle = style({
  font: 'heading-xl',
  color: 'heading',
  marginBottom: 8,
});

const subtitleStyle = style({
  font: 'body',
  color: 'neutral-subdued',
});

/**
 * UsersPage - Admin user management
 */
export default function UsersPage() {
  const { t } = useTranslation();

  return (
    <AdminLayout>
      <div className={containerStyle}>
        <div className={headerStyle}>
          <h1 className={titleStyle}>{t('admin.users.title')}</h1>
          <p className={subtitleStyle}>{t('admin.users.subtitle')}</p>
        </div>

        <IllustratedMessage>
          <Heading>{t('admin.comingSoon')}</Heading>
          <Content>{t('admin.users.description')}</Content>
        </IllustratedMessage>
      </div>
    </AdminLayout>
  );
}