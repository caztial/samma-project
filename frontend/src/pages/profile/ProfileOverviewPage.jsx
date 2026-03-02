import { useState, useEffect, useMemo, useCallback } from 'react';
import {
  Accordion,
  Disclosure,
  DisclosureTitle,
  DisclosurePanel,
  DisclosureHeader,
  ActionButton,
  Content,
  Heading,
  ProgressCircle,
  IllustratedMessage,
  Badge,
} from '@react-spectrum/s2';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import Edit from '@react-spectrum/s2/icons/Edit';
import { useTranslation } from '../../i18n/useTranslation';
import { useAuth } from '../../contexts/AuthContext';
import { createProfileService } from '../../services/profileService';
import ProfileLayout from '../../layouts/ProfileLayout';

// Static styles at module level for S2 style macro compatibility
const containerStyle = style({
  display: 'flex',
  flexDirection: 'column',
  gap: 16,
  width: '100%',
  maxWidth: 800,
});

const loadingContainerStyle = style({
  display: 'flex',
  justifyContent: 'center',
  alignItems: 'center',
  padding: 48,
});

const fieldGridStyle = style({
  display: 'grid',
  gap: 12,
  sm: {
    gridTemplateColumns: 'repeat(2, 1fr)',
  },
});

const fieldGroupStyle = style({
  display: 'flex',
  flexDirection: 'column',
  gap: 4,
});

const labelStyle = style({
  fontSize: 'detail-sm',
  color: 'neutral-subdued',
});

const valueStyle = style({
  fontSize: 'body-sm',
});

const listItemStyle = style({
  padding: 12,
  borderBottom: '1px solid',
  borderColor: 'gray-200',
  ':last-child': {
    borderBottom: 'none',
  },
});

const emptyMessageStyle = style({
  padding: 16,
  textAlign: 'center',
  color: 'neutral-subdued',
});

/**
 * ProfileOverviewPage - User profile overview
 * Displays user profile data fetched from API in expandable accordion sections.
 */
export default function ProfileOverviewPage() {
  const { t } = useTranslation();
  const { token } = useAuth();

  // Create profile service with token getter
  const profileService = useMemo(() => {
    return createProfileService(() => token);
  }, [token]);

  const [profile, setProfile] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch profile data on mount
  useEffect(() => {
    const fetchProfile = async () => {
      if (!token) {
        setError('No authentication token');
        setIsLoading(false);
        return;
      }

      try {
        setIsLoading(true);
        setError(null);
        const response = await profileService.getProfile();
        setProfile(response.data);
      } catch (err) {
        console.error('Failed to fetch profile:', err);
        setError(err.response?.data?.error || err.message || 'Failed to load profile');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, [profileService, token]);

  // Handle edit button click (placeholder for future implementation)
  const handleEditClick = useCallback((section) => {
    console.log(`Edit clicked for section: ${section}`);
    // TODO: Implement edit functionality
  }, []);

  // Format date for display
  const formatDate = useCallback((dateString) => {
    if (!dateString) return '-';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString();
    } catch {
      return dateString;
    }
  }, []);

  // Render loading state
  if (isLoading) {
    return (
      <ProfileLayout>
        <div className={containerStyle}>
          <Heading level={2}>{t('profile.overview.title')}</Heading>
          <div className={loadingContainerStyle}>
            <ProgressCircle size="L" isIndeterminate aria-label="Loading profile" />
          </div>
        </div>
      </ProfileLayout>
    );
  }

  // Render error state
  if (error) {
    return (
      <ProfileLayout>
        <div className={containerStyle}>
          <Heading level={2}>{t('profile.overview.title')}</Heading>
          <IllustratedMessage>
            <Content>{t('common.error')}: {error}</Content>
          </IllustratedMessage>
        </div>
      </ProfileLayout>
    );
  }

  // Render no profile state
  if (!profile) {
    return (
      <ProfileLayout>
        <div className={containerStyle}>
          <Heading level={2}>{t('profile.overview.title')}</Heading>
          <Content>{t('profile.overview.noProfile')}</Content>
        </div>
      </ProfileLayout>
    );
  }

  return (
    <ProfileLayout>
      <div className={containerStyle}>
        <Heading level={2}>{t('profile.overview.title')}</Heading>

        <Accordion allowsMultipleExpanded>
          {/* Personal Information Section */}
          <Disclosure id="personal">
            <DisclosureHeader>
              <DisclosureTitle>{t('profile.overview.sections.personal')}</DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('personal')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.personal') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              <div className={fieldGridStyle}>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.firstName')}</span>
                  <span className={valueStyle}>{profile.firstName || '-'}</span>
                </div>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.lastName')}</span>
                  <span className={valueStyle}>{profile.lastName || '-'}</span>
                </div>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.gender')}</span>
                  <span className={valueStyle}>{profile.gender || '-'}</span>
                </div>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.dateOfBirth')}</span>
                  <span className={valueStyle}>{formatDate(profile.dateOfBirth)}</span>
                </div>
              </div>
            </DisclosurePanel>
          </Disclosure>

          {/* Contact Information Section */}
          <Disclosure id="contact">
            <DisclosureHeader>
              <DisclosureTitle>{t('profile.overview.sections.contact')}</DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('contact')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.contact') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              <div className={fieldGridStyle}>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.email')}</span>
                  <span className={valueStyle}>{profile.contact?.email || '-'}</span>
                </div>
                <div className={fieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.phone')}</span>
                  <span className={valueStyle}>{profile.contact?.contactNumber || '-'}</span>
                </div>
              </div>
            </DisclosurePanel>
          </Disclosure>

          {/* Addresses Section */}
          <Disclosure id="addresses">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.addresses')}
                {profile.addresses?.length > 0 && (
                  <Badge variant="informative">{profile.addresses.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('addresses')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.addresses') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.addresses?.length > 0 ? (
                profile.addresses.map((address, index) => (
                  <div key={address.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.addressType')}</span>
                        <span className={valueStyle}>{address.type || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.addressLine1')}</span>
                        <span className={valueStyle}>{address.line1 || '-'}</span>
                      </div>
                      {address.line2 && (
                        <div className={fieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.addressLine2')}</span>
                          <span className={valueStyle}>{address.line2}</span>
                        </div>
                      )}
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.suburb')}</span>
                        <span className={valueStyle}>{address.suburb || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.stateProvince')}</span>
                        <span className={valueStyle}>{address.stateProvince || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.country')}</span>
                        <span className={valueStyle}>{address.country || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.postcode')}</span>
                        <span className={valueStyle}>{address.postcode || '-'}</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noAddresses')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>

          {/* Emergency Contacts Section */}
          <Disclosure id="emergencyContacts">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.emergencyContacts')}
                {profile.emergencyContacts?.length > 0 && (
                  <Badge variant="informative">{profile.emergencyContacts.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('emergencyContacts')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.emergencyContacts') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.emergencyContacts?.length > 0 ? (
                profile.emergencyContacts.map((contact, index) => (
                  <div key={contact.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.contactName')}</span>
                        <span className={valueStyle}>{contact.name || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.relationship')}</span>
                        <span className={valueStyle}>{contact.relationship || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.phone')}</span>
                        <span className={valueStyle}>{contact.contactNumber || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.email')}</span>
                        <span className={valueStyle}>{contact.email || '-'}</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noEmergencyContacts')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>

          {/* Education Section */}
          <Disclosure id="education">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.education')}
                {profile.educations?.length > 0 && (
                  <Badge variant="informative">{profile.educations.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('education')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.education') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.educations?.length > 0 ? (
                profile.educations.map((edu, index) => (
                  <div key={edu.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.institution')}</span>
                        <span className={valueStyle}>{edu.institution || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.degree')}</span>
                        <span className={valueStyle}>{edu.degree || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.fieldOfStudy')}</span>
                        <span className={valueStyle}>{edu.fieldOfStudy || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.startDate')}</span>
                        <span className={valueStyle}>{formatDate(edu.startDate)}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.endDate')}</span>
                        <span className={valueStyle}>{formatDate(edu.endDate)}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.grade')}</span>
                        <span className={valueStyle}>{edu.grade || '-'}</span>
                      </div>
                      {edu.isVerified && (
                        <div className={fieldGroupStyle}>
                          <Badge variant="positive">{t('profile.overview.fields.verified')}</Badge>
                        </div>
                      )}
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noEducation')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>

          {/* Bank Accounts Section */}
          <Disclosure id="bankAccounts">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.bankAccounts')}
                {profile.bankAccounts?.length > 0 && (
                  <Badge variant="informative">{profile.bankAccounts.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('bankAccounts')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.bankAccounts') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.bankAccounts?.length > 0 ? (
                profile.bankAccounts.map((account, index) => (
                  <div key={account.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.bankName')}</span>
                        <span className={valueStyle}>{account.bankName || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.accountType')}</span>
                        <span className={valueStyle}>{account.accountType || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.accountHolderName')}</span>
                        <span className={valueStyle}>{account.accountHolderName || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.accountNumber')}</span>
                        <span className={valueStyle}>{account.accountNumber || '-'}</span>
                      </div>
                      {account.branchCode && (
                        <div className={fieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.branchCode')}</span>
                          <span className={valueStyle}>{account.branchCode}</span>
                        </div>
                      )}
                      {account.isVerified && (
                        <div className={fieldGroupStyle}>
                          <Badge variant="positive">{t('profile.overview.fields.verified')}</Badge>
                        </div>
                      )}
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noBankAccounts')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>

          {/* Identifications Section */}
          <Disclosure id="identifications">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.identifications')}
                {profile.identifications?.length > 0 && (
                  <Badge variant="informative">{profile.identifications.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('identifications')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.identifications') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.identifications?.length > 0 ? (
                profile.identifications.map((id, index) => (
                  <div key={id.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.idType')}</span>
                        <span className={valueStyle}>{id.type || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.idNumber')}</span>
                        <span className={valueStyle}>{id.value || '-'}</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noIdentifications')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>

          {/* Consents Section */}
          <Disclosure id="consents">
            <DisclosureHeader>
              <DisclosureTitle>
                {t('profile.overview.sections.consents')}
                {profile.consents?.length > 0 && (
                  <Badge variant="informative">{profile.consents.length}</Badge>
                )}
              </DisclosureTitle>
              <ActionButton
                onPress={() => handleEditClick('consents')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.consents') })}
              >
                <Edit />
              </ActionButton>
            </DisclosureHeader>
            <DisclosurePanel>
              {profile.consents?.length > 0 ? (
                profile.consents.map((consent, index) => (
                  <div key={consent.id || index} className={listItemStyle}>
                    <div className={fieldGridStyle}>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.termsVersion')}</span>
                        <span className={valueStyle}>{consent.termsVersion || '-'}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.acceptedAt')}</span>
                        <span className={valueStyle}>{formatDate(consent.acceptedAt)}</span>
                      </div>
                      <div className={fieldGroupStyle}>
                        <span className={labelStyle}>{t('profile.overview.fields.ipAddress')}</span>
                        <span className={valueStyle}>{consent.ipAddress || '-'}</span>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noConsents')}</div>
              )}
            </DisclosurePanel>
          </Disclosure>
        </Accordion>
      </div>
    </ProfileLayout>
  );
}