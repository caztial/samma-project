import { Heading } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import AdminLayout from '../../layouts/AdminLayout';
import QuestionsTable from './components/QuestionsTable';

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
 * QuestionsPage - Admin question bank management
 */
export default function QuestionsPage() {
  const { t } = useTranslation();

  return (
    <AdminLayout>
      <div className={containerStyle}>
        <div className={headerStyle}>
          <h1 className={titleStyle}>{t('admin.questions.title')}</h1>
          <p className={subtitleStyle}>{t('admin.questions.subtitle')}</p>
        </div>
        <QuestionsTable />
      </div>
    </AdminLayout>
  );
}