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
 * BankAccountsPage - Bank account management
 * Allows users to manage their bank account information for payments and withdrawals.
 */
export default function BankAccountsPage() {
  const { t } = useTranslation();

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <Heading level={2}>{t('profile.content.bankAccounts.title')}</Heading>
        <Content>{t('profile.content.bankAccounts.description')}</Content>
      </div>
    </ProfileLayout>
  );
}