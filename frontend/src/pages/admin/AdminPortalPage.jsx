import { Heading, Content, IllustratedMessage } from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import { useTranslation } from '../../i18n/useTranslation';
import MainLayout from '../../layouts/MainLayout';

export default function AdminPortalPage() {
  const { t } = useTranslation();

  return (
    <MainLayout>
      <div
        className={style({
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          flex: 1,
          gap: 24,
        })}
      >
        <IllustratedMessage>
          <Heading>{t('admin.title')}</Heading>
          <Content>{t('admin.comingSoon')}</Content>
        </IllustratedMessage>
      </div>
    </MainLayout>
  );
}