import { useState, useEffect, useMemo, useCallback } from 'react';
import {
  Accordion,
  AccordionItem,
  AccordionItemTitle,
  AccordionItemPanel,
  AccordionItemHeader,
  ActionButton,
  Content,
  Heading,
  ProgressCircle,
  IllustratedMessage,
  Badge,
  Divider,
  AlertDialog,
  DialogContainer,
  Dialog,
  ButtonGroup,
  Button,
  Form,
  TextField,
  Picker,
  PickerItem,
  DatePicker,
  ToastQueue
} from '@react-spectrum/s2';
import { parseDate, CalendarDate } from '@internationalized/date';
import { style } from '@react-spectrum/s2/style' with { type: 'macro' };
import Edit from '@react-spectrum/s2/icons/Edit';
import Add from '@react-spectrum/s2/icons/Add';
import Delete from '@react-spectrum/s2/icons/Delete';
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
  marginX: 'auto',
});

const loadingContainerStyle = style({
  display: 'flex',
  justifyContent: 'center',
  alignItems: 'center',
  padding: 48,
});

const fieldGridStyle = style({
  display: 'grid',
  gap: 20,
  gridTemplateColumns: {
    sm: 'repeat(1, minmax(0, 1fr))',
    md: 'repeat(2, minmax(0, 1fr))',
    lg: 'repeat(2, minmax(0, 1fr))',
  },
});

const inlineFieldGroupStyle = style({
  display: 'flex',
  flexDirection: 'row',
  gap: 16,
  alignItems: 'baseline',
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
});

const listItemActionsStyle = style({
  display: 'flex',
  justifyContent: 'end',
  gap: 8,
  marginTop: 8,
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
  const { getToken, onUnauthorized, token } = useAuth();

  // Create profile service with token getter and unauthorized handler
  const profileService = useMemo(() => {
    return createProfileService({ getToken, onUnauthorized });
  }, [getToken, onUnauthorized]);

  const [profile, setProfile] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [pendingDelete, setPendingDelete] = useState(null);
  const [isDeleting, setIsDeleting] = useState(false);

  // Personal & Contact Information edit dialog state
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editData, setEditData] = useState({
    firstName: '',
    lastName: '',
    gender: null,
    dateOfBirth: null,
    email: '',
    contactNumber: ''
  });
  const [isUpdating, setIsUpdating] = useState(false);
  const [validationErrors, setValidationErrors] = useState({});

  // Gender options matching backend enum (Gender.cs) - use string keys that backend expects
  const genderOptions = useMemo(() => [
    { id: 'Male', name: t('profile.overview.genderOptions.male') },
    { id: 'Female', name: t('profile.overview.genderOptions.female') },
    { id: 'Other', name: t('profile.overview.genderOptions.other') },
    { id: 'PreferNotToSay', name: t('profile.overview.genderOptions.preferNotToSay') }
  ], [t]);

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

  // Handle edit button click for personal & contact information
  const handleEditClick = useCallback((section, itemId) => {
    if (section === 'personal' && profile) {
      // Parse date from ISO string to CalendarDate for DatePicker
      let dob = null;
      if (profile.dateOfBirth) {
        try {
          // Backend sends date as ISO string, we need YYYY-MM-DD format for parseDate
          const dateStr = profile.dateOfBirth.split('T')[0];
          dob = parseDate(dateStr);
        } catch (e) {
          console.error('Failed to parse date:', e);
        }
      }

      setEditData({
        firstName: profile.firstName || '',
        lastName: profile.lastName || '',
        gender: profile.gender || null,  // Backend returns gender as string enum
        dateOfBirth: dob,
        email: profile.contact?.email || '',
        contactNumber: profile.contact?.contactNumber || ''
      });
      setValidationErrors({});
      setEditDialogOpen(true);
    } else {
      console.log(`Edit clicked for section: ${section}, item: ${itemId}`);
      // TODO: Implement edit functionality for other sections
    }
  }, [profile]);

  // Handle field changes in edit dialog
  const handleFieldChange = useCallback((field, value) => {
    setEditData(prev => ({ ...prev, [field]: value }));
    // Clear validation error for this field when user makes changes
    if (validationErrors[field]) {
      setValidationErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  }, [validationErrors]);

  // Email validation regex
  const emailRegex = useMemo(() => /^[^\s@]+@[^\s@]+\.[^\s@]+$/, []);

  // Validate edit form
  const validateForm = useCallback(() => {
    const errors = {};

    if (!editData.firstName || editData.firstName.trim() === '') {
      errors.firstName = t('profile.overview.editPersonalDialog.validation.firstNameRequired');
    } else if (editData.firstName.trim().length < 3) {
      errors.firstName = t('profile.overview.editPersonalDialog.validation.firstNameMin');
    }

    if (!editData.gender) {
      errors.gender = t('profile.overview.editPersonalDialog.validation.genderRequired');
    }

    if (!editData.dateOfBirth) {
      errors.dob = t('profile.overview.editPersonalDialog.validation.dobRequired');
    }

    // Email validation - only validate if email is provided
    if (editData.email && editData.email.trim() !== '') {
      if (!emailRegex.test(editData.email.trim())) {
        errors.email = t('profile.overview.editPersonalDialog.validation.emailInvalid');
      }
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  }, [editData, t, emailRegex]);

  // Check if form has any errors for disabling submit button
  const hasValidationErrors = useMemo(() => {
    const errors = {};

    if (!editData.firstName || editData.firstName.trim() === '') {
      errors.firstName = true;
    } else if (editData.firstName.trim().length < 3) {
      errors.firstName = true;
    }

    if (!editData.gender) {
      errors.gender = true;
    }

    if (!editData.dateOfBirth) {
      errors.dob = true;
    }

    // Email validation - only validate if email is provided
    if (editData.email && editData.email.trim() !== '') {
      if (!emailRegex.test(editData.email.trim())) {
        errors.email = true;
      }
    }

    return Object.keys(errors).length > 0;
  }, [editData, emailRegex]);

  // Handle update submission
  const handleUpdatePersonalInfo = useCallback(async () => {
    if (!validateForm() || !profile) return;

    try {
      setIsUpdating(true);

      // Format date as ISO string for backend
      const dobString = editData.dateOfBirth
        ? editData.dateOfBirth.toString() // CalendarDate.toString() returns YYYY-MM-DD
        : null;

      const updateData = {
        firstName: editData.firstName.trim(),
        lastName: editData.lastName?.trim() || null,
        gender: editData.gender,  // Send string enum value (e.g., "Male", "Female")
        dateOfBirth: dobString,
        email: editData.email?.trim() || null,
        contactNumber: editData.contactNumber?.trim() || null
      };

      // Pass profileId to update the user's profile
      await profileService.updateProfile(profile.id, updateData);

      // Update local state
      setProfile(prev => ({
        ...prev,
        firstName: editData.firstName.trim(),
        lastName: editData.lastName?.trim() || null,
        gender: editData.gender,
        dateOfBirth: dobString,
        contact: {
          ...prev.contact,
          email: editData.email?.trim() || null,
          contactNumber: editData.contactNumber?.trim() || null
        }
      }));

      ToastQueue.positive(t('profile.overview.editPersonalDialog.success'), { timeout: 3000 });
      setEditDialogOpen(false);
    } catch (err) {
      console.error('Failed to update profile:', err);
      ToastQueue.negative(t('profile.overview.editPersonalDialog.error'), { timeout: 3000 });
    } finally {
      setIsUpdating(false);
    }
  }, [editData, profile, profileService, validateForm, t]);

  // Handle delete button click - shows confirmation dialog
  const handleDeleteClick = useCallback((section, itemId) => {
    setPendingDelete({ section, itemId });
  }, []);

  // Handle add button click (placeholder for future implementation)
  const handleAddClick = useCallback((section) => {
    console.log(`Add clicked for section: ${section}`);
    // TODO: Implement add functionality
  }, []);

  // Confirm and execute delete
  const confirmDelete = useCallback(async () => {
    if (!pendingDelete || !profile) return;

    const { section, itemId } = pendingDelete;

    try {
      setIsDeleting(true);

      if (section === 'addresses') {
        await profileService.removeAddress(profile.id, itemId);
        setProfile(prev => ({
          ...prev,
          addresses: prev.addresses.filter(a => a.id !== itemId)
        }));
        ToastQueue.positive(t('profile.overview.deleteConfirm.success', { section: t('profile.overview.sections.addresses') }), { timeout: 3000 });
      } else if (section === 'emergencyContacts') {
        await profileService.removeEmergencyContact(profile.id, itemId);
        setProfile(prev => ({
          ...prev,
          emergencyContacts: prev.emergencyContacts.filter(c => c.id !== itemId)
        }));
        ToastQueue.positive(t('profile.overview.deleteConfirm.success', { section: t('profile.overview.sections.emergencyContacts') }), { timeout: 3000 });
      } else if (section === 'education') {
        await profileService.removeEducation(profile.id, itemId);
        setProfile(prev => ({
          ...prev,
          educations: prev.educations.filter(e => e.id !== itemId)
        }));
        ToastQueue.positive(t('profile.overview.deleteConfirm.success', { section: t('profile.overview.sections.education') }), { timeout: 3000 });
      } else if (section === 'bankAccounts') {
        await profileService.removeBankAccount(profile.id, itemId);
        setProfile(prev => ({
          ...prev,
          bankAccounts: prev.bankAccounts.filter(b => b.id !== itemId)
        }));
        ToastQueue.positive(t('profile.overview.deleteConfirm.success', { section: t('profile.overview.sections.bankAccounts') }), { timeout: 3000 });
      } else if (section === 'identifications') {
        await profileService.removeIdentification(profile.id, itemId);
        setProfile(prev => ({
          ...prev,
          identifications: prev.identifications.filter(i => i.id !== itemId)
        }));
        ToastQueue.positive(t('profile.overview.deleteConfirm.success', { section: t('profile.overview.sections.identifications') }), { timeout: 3000 });
      }
    } catch (err) {
      console.error('Failed to delete:', err);
      ToastQueue.negative(t('profile.overview.deleteConfirm.error', { section: t(`profile.overview.sections.${pendingDelete.section}`) }), { timeout: 3000 });
    } finally {
      setIsDeleting(false);
      setPendingDelete(null);
    }
  }, [pendingDelete, profile, profileService, t]);

  // Cancel delete
  const cancelDelete = useCallback(() => {
    setPendingDelete(null);
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
          {/* Personal & Contact Information Section - Single Item */}
          <AccordionItem id="personal">
            <AccordionItemHeader>
              <AccordionItemTitle>{t('profile.overview.sections.personalAndContact')}</AccordionItemTitle>
              <ActionButton
                onPress={() => handleEditClick('personal')}
                aria-label={t('profile.overview.editSection', { section: t('profile.overview.sections.personalAndContact') })}
              >
                <Edit />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              <div className={fieldGridStyle}>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.firstName')}</span>
                  <span className={valueStyle}>{profile.firstName || '-'}</span>
                </div>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.lastName')}</span>
                  <span className={valueStyle}>{profile.lastName || '-'}</span>
                </div>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.gender')}</span>
                  <span className={valueStyle}>{profile.gender || '-'}</span>
                </div>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.dateOfBirth')}</span>
                  <span className={valueStyle}>{formatDate(profile.dateOfBirth)}</span>
                </div>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.email')}</span>
                  <span className={valueStyle}>{profile.contact?.email || '-'}</span>
                </div>
                <div className={inlineFieldGroupStyle}>
                  <span className={labelStyle}>{t('profile.overview.fields.phone')}</span>
                  <span className={valueStyle}>{profile.contact?.contactNumber || '-'}</span>
                </div>
              </div>
            </AccordionItemPanel>
          </AccordionItem>

          {/* Addresses Section - Multi Item */}
          <AccordionItem id="addresses">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.addresses')}
                {profile.addresses?.length > 0 && (
                  <Badge variant="informative">{profile.addresses.length}</Badge>
                )}
              </AccordionItemTitle>
              <ActionButton
                onPress={() => handleAddClick('addresses')}
                aria-label={t('profile.overview.addSection', { section: t('profile.overview.sections.addresses') })}
              >
                <Add />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.addresses?.length > 0 ? (
                profile.addresses.map((address, index) => (
                  <div key={address.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.addressType')}</span>
                          <span className={valueStyle}>{address.type || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.addressLine1')}</span>
                          <span className={valueStyle}>{address.line1 || '-'}</span>
                        </div>
                        {address.line2 && (
                          <div className={inlineFieldGroupStyle}>
                            <span className={labelStyle}>{t('profile.overview.fields.addressLine2')}</span>
                            <span className={valueStyle}>{address.line2}</span>
                          </div>
                        )}
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.suburb')}</span>
                          <span className={valueStyle}>{address.suburb || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.stateProvince')}</span>
                          <span className={valueStyle}>{address.stateProvince || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.country')}</span>
                          <span className={valueStyle}>{address.country || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.postcode')}</span>
                          <span className={valueStyle}>{address.postcode || '-'}</span>
                        </div>
                      </div>
                      <div className={listItemActionsStyle}>
                        <ActionButton
                          onPress={() => handleEditClick('addresses', address.id || index)}
                          aria-label={t('profile.overview.editItem')}
                        >
                          <Edit />
                        </ActionButton>
                        <ActionButton
                          onPress={() => handleDeleteClick('addresses', address.id || index)}
                          aria-label={t('profile.overview.deleteItem')}
                        >
                          <Delete />
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noAddresses')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>

          {/* Emergency Contacts Section - Multi Item */}
          <AccordionItem id="emergencyContacts">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.emergencyContacts')}
                {profile.emergencyContacts?.length > 0 && (
                  <Badge variant="informative">{profile.emergencyContacts.length}</Badge>
                )}
              </AccordionItemTitle>
              <ActionButton
                onPress={() => handleAddClick('emergencyContacts')}
                aria-label={t('profile.overview.addSection', { section: t('profile.overview.sections.emergencyContacts') })}
              >
                <Add />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.emergencyContacts?.length > 0 ? (
                profile.emergencyContacts.map((contact, index) => (
                  <div key={contact.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.contactName')}</span>
                          <span className={valueStyle}>{contact.name || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.relationship')}</span>
                          <span className={valueStyle}>{contact.relationship || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.phone')}</span>
                          <span className={valueStyle}>{contact.contactNumber || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.email')}</span>
                          <span className={valueStyle}>{contact.email || '-'}</span>
                        </div>
                      </div>
                      <div className={listItemActionsStyle}>
                        <ActionButton
                          onPress={() => handleEditClick('emergencyContacts', contact.id || index)}
                          aria-label={t('profile.overview.editItem')}
                        >
                          <Edit />
                        </ActionButton>
                        <ActionButton
                          onPress={() => handleDeleteClick('emergencyContacts', contact.id || index)}
                          aria-label={t('profile.overview.deleteItem')}
                        >
                          <Delete />
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noEmergencyContacts')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>

          {/* Education Section - Multi Item */}
          <AccordionItem id="education">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.education')}
                {profile.educations?.length > 0 && (
                  <Badge variant="informative">{profile.educations.length}</Badge>
                )}
              </AccordionItemTitle>
              <ActionButton
                onPress={() => handleAddClick('education')}
                aria-label={t('profile.overview.addSection', { section: t('profile.overview.sections.education') })}
              >
                <Add />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.educations?.length > 0 ? (
                profile.educations.map((edu, index) => (
                  <div key={edu.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.institution')}</span>
                          <span className={valueStyle}>{edu.institution || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.degree')}</span>
                          <span className={valueStyle}>{edu.degree || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.fieldOfStudy')}</span>
                          <span className={valueStyle}>{edu.fieldOfStudy || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.startDate')}</span>
                          <span className={valueStyle}>{formatDate(edu.startDate)}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.endDate')}</span>
                          <span className={valueStyle}>{formatDate(edu.endDate)}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.grade')}</span>
                          <span className={valueStyle}>{edu.grade || '-'}</span>
                        </div>
                        {edu.isVerified && (
                          <div className={inlineFieldGroupStyle}>
                            <Badge variant="positive">{t('profile.overview.fields.verified')}</Badge>
                          </div>
                        )}
                      </div>
                      <div className={listItemActionsStyle}>
                        <ActionButton
                          onPress={() => handleEditClick('education', edu.id || index)}
                          aria-label={t('profile.overview.editItem')}
                        >
                          <Edit />
                        </ActionButton>
                        <ActionButton
                          onPress={() => handleDeleteClick('education', edu.id || index)}
                          aria-label={t('profile.overview.deleteItem')}
                        >
                          <Delete />
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noEducation')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>

          {/* Bank Accounts Section - Multi Item */}
          <AccordionItem id="bankAccounts">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.bankAccounts')}
                {profile.bankAccounts?.length > 0 && (
                  <Badge variant="informative">{profile.bankAccounts.length}</Badge>
                )}
              </AccordionItemTitle>
              <ActionButton
                onPress={() => handleAddClick('bankAccounts')}
                aria-label={t('profile.overview.addSection', { section: t('profile.overview.sections.bankAccounts') })}
              >
                <Add />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.bankAccounts?.length > 0 ? (
                profile.bankAccounts.map((account, index) => (
                  <div key={account.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.bankName')}</span>
                          <span className={valueStyle}>{account.bankName || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.accountType')}</span>
                          <span className={valueStyle}>{account.accountType || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.accountHolderName')}</span>
                          <span className={valueStyle}>{account.accountHolderName || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.accountNumber')}</span>
                          <span className={valueStyle}>{account.accountNumber || '-'}</span>
                        </div>
                        {account.branchCode && (
                          <div className={inlineFieldGroupStyle}>
                            <span className={labelStyle}>{t('profile.overview.fields.branchCode')}</span>
                            <span className={valueStyle}>{account.branchCode}</span>
                          </div>
                        )}
                        {account.isVerified && (
                          <div className={inlineFieldGroupStyle}>
                            <Badge variant="positive">{t('profile.overview.fields.verified')}</Badge>
                          </div>
                        )}
                      </div>
                      <div className={listItemActionsStyle}>
                        <ActionButton
                          onPress={() => handleEditClick('bankAccounts', account.id || index)}
                          aria-label={t('profile.overview.editItem')}
                        >
                          <Edit />
                        </ActionButton>
                        <ActionButton
                          onPress={() => handleDeleteClick('bankAccounts', account.id || index)}
                          aria-label={t('profile.overview.deleteItem')}
                        >
                          <Delete />
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noBankAccounts')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>

          {/* Identifications Section - Multi Item */}
          <AccordionItem id="identifications">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.identifications')}
                {profile.identifications?.length > 0 && (
                  <Badge variant="informative">{profile.identifications.length}</Badge>
                )}
              </AccordionItemTitle>
              <ActionButton
                onPress={() => handleAddClick('identifications')}
                aria-label={t('profile.overview.addSection', { section: t('profile.overview.sections.identifications') })}
              >
                <Add />
              </ActionButton>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.identifications?.length > 0 ? (
                profile.identifications.map((id, index) => (
                  <div key={id.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.idType')}</span>
                          <span className={valueStyle}>{id.type || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.idNumber')}</span>
                          <span className={valueStyle}>{id.value || '-'}</span>
                        </div>
                      </div>
                      <div className={listItemActionsStyle}>
                        <ActionButton
                          onPress={() => handleEditClick('identifications', id.id || index)}
                          aria-label={t('profile.overview.editItem')}
                        >
                          <Edit />
                        </ActionButton>
                        <ActionButton
                          onPress={() => handleDeleteClick('identifications', id.id || index)}
                          aria-label={t('profile.overview.deleteItem')}
                        >
                          <Delete />
                        </ActionButton>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noIdentifications')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>

          {/* Consents Section - Multi Item (Read-only) */}
          <AccordionItem id="consents">
            <AccordionItemHeader>
              <AccordionItemTitle>
                {t('profile.overview.sections.consents')}
                {profile.consents?.length > 0 && (
                  <Badge variant="informative">{profile.consents.length}</Badge>
                )}
              </AccordionItemTitle>
            </AccordionItemHeader>
            <AccordionItemPanel>
              {profile.consents?.length > 0 ? (
                profile.consents.map((consent, index) => (
                  <div key={consent.id || index}>
                    {index > 0 && <Divider />}
                    <div className={listItemStyle}>
                      <div className={fieldGridStyle}>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.termsVersion')}</span>
                          <span className={valueStyle}>{consent.termsVersion || '-'}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.acceptedAt')}</span>
                          <span className={valueStyle}>{formatDate(consent.acceptedAt)}</span>
                        </div>
                        <div className={inlineFieldGroupStyle}>
                          <span className={labelStyle}>{t('profile.overview.fields.ipAddress')}</span>
                          <span className={valueStyle}>{consent.ipAddress || '-'}</span>
                        </div>
                      </div>
                    </div>
                  </div>
                ))
              ) : (
                <div className={emptyMessageStyle}>{t('profile.overview.noConsents')}</div>
              )}
            </AccordionItemPanel>
          </AccordionItem>
        </Accordion>

        {/* Delete Confirmation Dialog */}
        <DialogContainer onDismiss={cancelDelete}>
          {pendingDelete && (
            <AlertDialog
              title={t('profile.overview.deleteConfirm.title')}
              variant="destructive"
              primaryActionLabel={t('profile.overview.deleteConfirm.delete')}
              secondaryActionLabel={t('profile.overview.deleteConfirm.cancel')}
              onPrimaryAction={confirmDelete}
              onSecondaryAction={cancelDelete}
              isPending={isDeleting}
            >
              {t('profile.overview.deleteConfirm.message', { section: t(`profile.overview.sections.${pendingDelete.section}`) })}
            </AlertDialog>
          )}
        </DialogContainer>

        {/* Edit Personal Information Dialog */}
        <DialogContainer onDismiss={() => setEditDialogOpen(false)}>
          {editDialogOpen && (
            <Dialog>
              <Heading slot="title">{t('profile.overview.editPersonalDialog.title')}</Heading>
              <Content>
                <Form>
                  <TextField
                    label={t('profile.overview.fields.firstName')}
                    isRequired
                    necessityIndicator="icon"
                    value={editData.firstName}
                    onChange={(value) => handleFieldChange('firstName', value)}
                    errorMessage={validationErrors.firstName}
                    validationState={validationErrors.firstName ? 'invalid' : 'valid'}
                  />
                  <TextField
                    label={t('profile.overview.fields.lastName')}
                    value={editData.lastName || ''}
                    onChange={(value) => handleFieldChange('lastName', value)}
                  />
                  <Picker
                    label={t('profile.overview.fields.gender')}
                    isRequired
                    necessityIndicator="icon"
                    selectedKey={editData.gender}
                    onSelectionChange={(key) => handleFieldChange('gender', key)}
                    errorMessage={validationErrors.gender}
                    validationState={validationErrors.gender ? 'invalid' : 'valid'}
                    placeholder={t('profile.overview.editPersonalDialog.selectGender')}
                  >
                    {genderOptions.map((option) => (
                      <PickerItem key={option.id} id={option.id}>
                        {option.name}
                      </PickerItem>
                    ))}
                  </Picker>
                  <DatePicker
                    label={t('profile.overview.fields.dateOfBirth')}
                    isRequired
                    necessityIndicator="icon"
                    value={editData.dateOfBirth}
                    onChange={(value) => handleFieldChange('dateOfBirth', value)}
                    errorMessage={validationErrors.dob}
                    validationState={validationErrors.dob ? 'invalid' : 'valid'}
                    description={t('profile.overview.editPersonalDialog.dobFormat')}
                  />
                  <TextField
                    label={t('profile.overview.fields.email')}
                    type="email"
                    value={editData.email || ''}
                    onChange={(value) => handleFieldChange('email', value)}
                    errorMessage={validationErrors.email}
                    validationState={validationErrors.email ? 'invalid' : 'valid'}
                  />
                  <TextField
                    label={t('profile.overview.fields.phone')}
                    type="tel"
                    value={editData.contactNumber || ''}
                    onChange={(value) => handleFieldChange('contactNumber', value)}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setEditDialogOpen(false)}>
                  {t('profile.overview.editPersonalDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleUpdatePersonalInfo}
                  isPending={isUpdating}
                  isDisabled={hasValidationErrors}
                >
                  {t('profile.overview.editPersonalDialog.save')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>
      </div>
    </ProfileLayout>
  );
}
