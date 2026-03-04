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
  Text,
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

const headingStyle = style({
  textAlign: 'center',
  font: 'heading',
});

const labelStyle = style({
  font: 'detail-sm',
  color: 'neutral-subdued',
});

const valueStyle = style({
  font: 'body-sm',
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
  display: 'block',
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

  // Address dialog state
  const [addressDialogOpen, setAddressDialogOpen] = useState(false);
  const [addressData, setAddressData] = useState({
    id: null,
    type: null,
    line1: '',
    line2: '',
    suburb: '',
    stateProvince: '',
    country: '',
    postcode: ''
  });
  const [addressDialogMode, setAddressDialogMode] = useState('add'); // 'add' or 'edit'
  const [isSavingAddress, setIsSavingAddress] = useState(false);

  // Emergency contact dialog state
  const [emergencyContactDialogOpen, setEmergencyContactDialogOpen] = useState(false);
  const [emergencyContactData, setEmergencyContactData] = useState({
    id: null,
    name: '',
    relationship: '',
    contactNumber: '',
    email: ''
  });
  const [emergencyContactDialogMode, setEmergencyContactDialogMode] = useState('add'); // 'add' or 'edit'
  const [isSavingEmergencyContact, setIsSavingEmergencyContact] = useState(false);

  // Education dialog state
  const [educationDialogOpen, setEducationDialogOpen] = useState(false);
  const [educationData, setEducationData] = useState({
    id: null,
    institution: '',
    degree: '',
    fieldOfStudy: '',
    startDate: null,
    endDate: null,
    grade: '',
    certificateNumber: ''
  });
  const [educationDialogMode, setEducationDialogMode] = useState('add'); // 'add' or 'edit'
  const [isSavingEducation, setIsSavingEducation] = useState(false);

  // Bank account dialog state
  const [bankAccountDialogOpen, setBankAccountDialogOpen] = useState(false);
  const [bankAccountData, setBankAccountData] = useState({
    id: null,
    bankName: '',
    accountType: '',
    accountHolderName: '',
    accountNumber: '',
    branchCode: ''
  });
  const [bankAccountDialogMode, setBankAccountDialogMode] = useState('add'); // 'add' or 'edit'
  const [isSavingBankAccount, setIsSavingBankAccount] = useState(false);

  // Identification dialog state
  const [identificationDialogOpen, setIdentificationDialogOpen] = useState(false);
  const [identificationData, setIdentificationData] = useState({
    id: null,
    type: '',
    value: ''
  });
  const [identificationDialogMode, setIdentificationDialogMode] = useState('add'); // 'add' or 'edit'
  const [isSavingIdentification, setIsSavingIdentification] = useState(false);

  // Gender options matching backend enum (Gender.cs) - use string keys that backend expects
  const genderOptions = useMemo(() => [
    { id: 'Male', name: t('profile.overview.genderOptions.male') },
    { id: 'Female', name: t('profile.overview.genderOptions.female') },
    { id: 'Other', name: t('profile.overview.genderOptions.other') },
    { id: 'PreferNotToSay', name: t('profile.overview.genderOptions.preferNotToSay') }
  ], [t]);

  // Address type options
  const addressTypeOptions = useMemo(() => [
    { id: 'Home', name: t('profile.overview.addressDialog.typeOptions.home') },
    { id: 'Work', name: t('profile.overview.addressDialog.typeOptions.work') },
    { id: 'Other', name: t('profile.overview.addressDialog.typeOptions.other') }
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
    } else if (section === 'addresses' && profile) {
      // Find the address to edit
      const address = profile.addresses?.find(a => a.id === itemId);
      if (address) {
        setAddressData({
          id: address.id,
          type: address.type || null,
          line1: address.line1 || '',
          line2: address.line2 || '',
          suburb: address.suburb || '',
          stateProvince: address.stateProvince || '',
          country: address.country || '',
          postcode: address.postcode || ''
        });
        setAddressDialogMode('edit');
        setAddressDialogOpen(true);
      }
    } else if (section === 'emergencyContacts' && profile) {
      // Find the emergency contact to edit
      const contact = profile.emergencyContacts?.find(c => c.id === itemId);
      if (contact) {
        setEmergencyContactData({
          id: contact.id,
          name: contact.name || '',
          relationship: contact.relationship || '',
          contactNumber: contact.contactNumber || '',
          email: contact.email || ''
        });
        setEmergencyContactDialogMode('edit');
        setEmergencyContactDialogOpen(true);
      }
    } else if (section === 'education' && profile) {
      // Find the education to edit
      const edu = profile.educations?.find(e => e.id === itemId);
      if (edu) {
        // Parse dates for DatePicker
        let startDate = null;
        let endDate = null;
        if (edu.startDate) {
          try {
            const dateStr = edu.startDate.split('T')[0];
            startDate = parseDate(dateStr);
          } catch (e) {
            console.error('Failed to parse start date:', e);
          }
        }
        if (edu.endDate) {
          try {
            const dateStr = edu.endDate.split('T')[0];
            endDate = parseDate(dateStr);
          } catch (e) {
            console.error('Failed to parse end date:', e);
          }
        }
        setEducationData({
          id: edu.id,
          institution: edu.institution || '',
          degree: edu.degree || '',
          fieldOfStudy: edu.fieldOfStudy || '',
          startDate: startDate,
          endDate: endDate,
          grade: edu.grade || '',
          certificateNumber: edu.certificateNumber || ''
        });
        setEducationDialogMode('edit');
        setEducationDialogOpen(true);
      }
    } else if (section === 'bankAccounts' && profile) {
      // Find the bank account to edit
      const account = profile.bankAccounts?.find(b => b.id === itemId);
      if (account) {
        setBankAccountData({
          id: account.id,
          bankName: account.bankName || '',
          accountType: account.accountType || '',
          accountHolderName: account.accountHolderName || '',
          accountNumber: account.accountNumber || '',
          branchCode: account.branchCode || ''
        });
        setBankAccountDialogMode('edit');
        setBankAccountDialogOpen(true);
      }
    } else if (section === 'identifications' && profile) {
      // Find the identification to edit
      const identification = profile.identifications?.find(i => i.id === itemId);
      if (identification) {
        setIdentificationData({
          id: identification.id,
          type: identification.type || '',
          value: identification.value || ''
        });
        setIdentificationDialogMode('edit');
        setIdentificationDialogOpen(true);
      }
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

  // Handle add button click
  const handleAddClick = useCallback((section) => {
    if (section === 'addresses') {
      // Reset form data for new address
      setAddressData({
        id: null,
        type: null,
        line1: '',
        line2: '',
        suburb: '',
        stateProvince: '',
        country: '',
        postcode: ''
      });
      setAddressDialogMode('add');
      setAddressDialogOpen(true);
    } else if (section === 'emergencyContacts') {
      // Reset form data for new emergency contact
      setEmergencyContactData({
        id: null,
        name: '',
        relationship: '',
        contactNumber: '',
        email: ''
      });
      setEmergencyContactDialogMode('add');
      setEmergencyContactDialogOpen(true);
    } else if (section === 'education') {
      // Reset form data for new education
      setEducationData({
        id: null,
        institution: '',
        degree: '',
        fieldOfStudy: '',
        startDate: null,
        endDate: null,
        grade: '',
        certificateNumber: ''
      });
      setEducationDialogMode('add');
      setEducationDialogOpen(true);
    } else if (section === 'bankAccounts') {
      // Reset form data for new bank account
      setBankAccountData({
        id: null,
        bankName: '',
        accountType: '',
        accountHolderName: '',
        accountNumber: '',
        branchCode: ''
      });
      setBankAccountDialogMode('add');
      setBankAccountDialogOpen(true);
    } else if (section === 'identifications') {
      // Reset form data for new identification
      setIdentificationData({
        id: null,
        type: '',
        value: ''
      });
      setIdentificationDialogMode('add');
      setIdentificationDialogOpen(true);
    } else {
      console.log(`Add clicked for section: ${section}`);
      // TODO: Implement add functionality for other sections
    }
  }, []);

  // Handle address field changes
  const handleAddressFieldChange = useCallback((field, value) => {
    setAddressData(prev => ({ ...prev, [field]: value }));
  }, []);

  // Check if address form has validation errors (required fields: type, line1, country)
  const hasAddressValidationErrors = useMemo(() => {
    return !addressData.type ||
      !addressData.line1?.trim() ||
      !addressData.country?.trim();
  }, [addressData]);

  // Handle emergency contact field changes
  const handleEmergencyContactFieldChange = useCallback((field, value) => {
    setEmergencyContactData(prev => ({ ...prev, [field]: value }));
  }, []);

  // Check if emergency contact form has validation errors (required fields: name, contactNumber)
  const hasEmergencyContactValidationErrors = useMemo(() => {
    const hasErrors = !emergencyContactData.name?.trim() ||
      !emergencyContactData.contactNumber?.trim();

    // Additional email validation if provided
    if (emergencyContactData.email?.trim() && !emailRegex.test(emergencyContactData.email.trim())) {
      return true;
    }

    return hasErrors;
  }, [emergencyContactData, emailRegex]);

  // Handle education field changes
  const handleEducationFieldChange = useCallback((field, value) => {
    setEducationData(prev => ({ ...prev, [field]: value }));
  }, []);

  // Check if education form has validation errors (required fields: institution, degree)
  const hasEducationValidationErrors = useMemo(() => {
    return !educationData.institution?.trim() ||
      !educationData.degree?.trim();
  }, [educationData]);

  // Handle bank account field changes
  const handleBankAccountFieldChange = useCallback((field, value) => {
    setBankAccountData(prev => ({ ...prev, [field]: value }));
  }, []);

  // Check if bank account form has validation errors (required fields: bankName, accountType, accountHolderName, accountNumber)
  const hasBankAccountValidationErrors = useMemo(() => {
    return !bankAccountData.bankName?.trim() ||
      !bankAccountData.accountType?.trim() ||
      !bankAccountData.accountHolderName?.trim() ||
      !bankAccountData.accountNumber?.trim();
  }, [bankAccountData]);

  // Handle identification field changes
  const handleIdentificationFieldChange = useCallback((field, value) => {
    setIdentificationData(prev => ({ ...prev, [field]: value }));
  }, []);

  // Check if identification form has validation errors (required fields: type, value)
  const hasIdentificationValidationErrors = useMemo(() => {
    return !identificationData.type?.trim() ||
      !identificationData.value?.trim();
  }, [identificationData]);

  // Handle save address (add or update)
  const handleSaveAddress = useCallback(async () => {
    if (hasAddressValidationErrors || !profile) return;

    try {
      setIsSavingAddress(true);

      // Backend expects: { type: string, address: { line1, line2, suburb, stateProvince, country, postcode } }
      const addressPayload = {
        type: addressData.type,
        address: {
          line1: addressData.line1.trim(),
          line2: addressData.line2?.trim() || '',
          suburb: addressData.suburb?.trim() || '',
          stateProvince: addressData.stateProvince?.trim() || '',
          country: addressData.country.trim(),
          postcode: addressData.postcode?.trim() || ''
        }
      };

      if (addressDialogMode === 'add') {
        const response = await profileService.addAddress(profile.id, addressPayload);
        setProfile(prev => ({
          ...prev,
          addresses: [...(prev.addresses || []), response.data]
        }));
        ToastQueue.positive(t('profile.overview.addressDialog.addSuccess'), { timeout: 3000 });
      } else {
        await profileService.updateAddress(profile.id, addressData.id, addressPayload);
        // Update local state - flatten the address object for display
        setProfile(prev => ({
          ...prev,
          addresses: prev.addresses.map(a =>
            a.id === addressData.id ? {
              ...a,
              type: addressPayload.type,
              line1: addressPayload.address.line1,
              line2: addressPayload.address.line2,
              suburb: addressPayload.address.suburb,
              stateProvince: addressPayload.address.stateProvince,
              country: addressPayload.address.country,
              postcode: addressPayload.address.postcode
            } : a
          )
        }));
        ToastQueue.positive(t('profile.overview.addressDialog.updateSuccess'), { timeout: 3000 });
      }

      setAddressDialogOpen(false);
    } catch (err) {
      console.error('Failed to save address:', err);
      ToastQueue.negative(t('profile.overview.addressDialog.error'), { timeout: 3000 });
    } finally {
      setIsSavingAddress(false);
    }
  }, [addressData, addressDialogMode, profile, profileService, hasAddressValidationErrors, t]);

  // Handle save emergency contact (add or update)
  const handleSaveEmergencyContact = useCallback(async () => {
    if (hasEmergencyContactValidationErrors || !profile) return;

    try {
      setIsSavingEmergencyContact(true);

      // Backend expects: { emergencyContact: { name, relationship, contactNumber, email } }
      const contactPayload = {
        emergencyContact: {
          name: emergencyContactData.name.trim(),
          relationship: emergencyContactData.relationship?.trim() || null,
          contactNumber: emergencyContactData.contactNumber.trim(),
          email: emergencyContactData.email?.trim() || null
        }
      };

      if (emergencyContactDialogMode === 'add') {
        const response = await profileService.addEmergencyContact(profile.id, contactPayload);
        setProfile(prev => ({
          ...prev,
          emergencyContacts: [...(prev.emergencyContacts || []), response.data]
        }));
        ToastQueue.positive(t('profile.overview.emergencyContactDialog.addSuccess'), { timeout: 3000 });
      } else {
        await profileService.updateEmergencyContact(profile.id, emergencyContactData.id, contactPayload);
        setProfile(prev => ({
          ...prev,
          emergencyContacts: prev.emergencyContacts.map(c =>
            c.id === emergencyContactData.id ? {
              ...c,
              name: contactPayload.emergencyContact.name,
              relationship: contactPayload.emergencyContact.relationship,
              contactNumber: contactPayload.emergencyContact.contactNumber,
              email: contactPayload.emergencyContact.email
            } : c
          )
        }));
        ToastQueue.positive(t('profile.overview.emergencyContactDialog.updateSuccess'), { timeout: 3000 });
      }

      setEmergencyContactDialogOpen(false);
    } catch (err) {
      console.error('Failed to save emergency contact:', err);
      ToastQueue.negative(t('profile.overview.emergencyContactDialog.error'), { timeout: 3000 });
    } finally {
      setIsSavingEmergencyContact(false);
    }
  }, [emergencyContactData, emergencyContactDialogMode, profile, profileService, hasEmergencyContactValidationErrors, t]);

  // Handle save education (add or update)
  const handleSaveEducation = useCallback(async () => {
    if (hasEducationValidationErrors || !profile) return;

    try {
      setIsSavingEducation(true);

      // Format dates as ISO strings for backend
      const startDateString = educationData.startDate
        ? educationData.startDate.toString()
        : null;
      const endDateString = educationData.endDate
        ? educationData.endDate.toString()
        : null;

      // Backend expects: { education: { institution, degree, fieldOfStudy, startDate, endDate, grade, certificateNumber } }
      const educationPayload = {
        education: {
          institution: educationData.institution.trim(),
          degree: educationData.degree.trim(),
          fieldOfStudy: educationData.fieldOfStudy?.trim() || null,
          startDate: startDateString,
          endDate: endDateString,
          grade: educationData.grade?.trim() || null,
          certificateNumber: educationData.certificateNumber?.trim() || null
        }
      };

      if (educationDialogMode === 'add') {
        const response = await profileService.addEducation(profile.id, educationPayload);
        setProfile(prev => ({
          ...prev,
          educations: [...(prev.educations || []), response.data]
        }));
        ToastQueue.positive(t('profile.overview.educationDialog.addSuccess'), { timeout: 3000 });
      } else {
        await profileService.updateEducation(profile.id, educationData.id, educationPayload);
        setProfile(prev => ({
          ...prev,
          educations: prev.educations.map(e =>
            e.id === educationData.id ? {
              ...e,
              institution: educationPayload.education.institution,
              degree: educationPayload.education.degree,
              fieldOfStudy: educationPayload.education.fieldOfStudy,
              startDate: educationPayload.education.startDate,
              endDate: educationPayload.education.endDate,
              grade: educationPayload.education.grade,
              certificateNumber: educationPayload.education.certificateNumber
            } : e
          )
        }));
        ToastQueue.positive(t('profile.overview.educationDialog.updateSuccess'), { timeout: 3000 });
      }

      setEducationDialogOpen(false);
    } catch (err) {
      console.error('Failed to save education:', err);
      ToastQueue.negative(t('profile.overview.educationDialog.error'), { timeout: 3000 });
    } finally {
      setIsSavingEducation(false);
    }
  }, [educationData, educationDialogMode, profile, profileService, hasEducationValidationErrors, t]);

  // Handle save bank account (add or update)
  const handleSaveBankAccount = useCallback(async () => {
    if (hasBankAccountValidationErrors || !profile) return;

    try {
      setIsSavingBankAccount(true);

      // Backend expects: { bankAccount: { bankName, accountType, accountHolderName, accountNumber, branchCode } }
      const bankAccountPayload = {
        bankAccount: {
          bankName: bankAccountData.bankName.trim(),
          accountType: bankAccountData.accountType.trim(),
          accountHolderName: bankAccountData.accountHolderName.trim(),
          accountNumber: bankAccountData.accountNumber.trim(),
          branchCode: bankAccountData.branchCode?.trim() || null
        }
      };

      if (bankAccountDialogMode === 'add') {
        const response = await profileService.addBankAccount(profile.id, bankAccountPayload);
        setProfile(prev => ({
          ...prev,
          bankAccounts: [...(prev.bankAccounts || []), response.data]
        }));
        ToastQueue.positive(t('profile.overview.bankAccountDialog.addSuccess'), { timeout: 3000 });
      } else {
        await profileService.updateBankAccount(profile.id, bankAccountData.id, bankAccountPayload);
        setProfile(prev => ({
          ...prev,
          bankAccounts: prev.bankAccounts.map(b =>
            b.id === bankAccountData.id ? {
              ...b,
              bankName: bankAccountPayload.bankAccount.bankName,
              accountType: bankAccountPayload.bankAccount.accountType,
              accountHolderName: bankAccountPayload.bankAccount.accountHolderName,
              accountNumber: bankAccountPayload.bankAccount.accountNumber,
              branchCode: bankAccountPayload.bankAccount.branchCode
            } : b
          )
        }));
        ToastQueue.positive(t('profile.overview.bankAccountDialog.updateSuccess'), { timeout: 3000 });
      }

      setBankAccountDialogOpen(false);
    } catch (err) {
      console.error('Failed to save bank account:', err);
      ToastQueue.negative(t('profile.overview.bankAccountDialog.error'), { timeout: 3000 });
    } finally {
      setIsSavingBankAccount(false);
    }
  }, [bankAccountData, bankAccountDialogMode, profile, profileService, hasBankAccountValidationErrors, t]);

  // Handle save identification (add or update)
  const handleSaveIdentification = useCallback(async () => {
    if (hasIdentificationValidationErrors || !profile) return;

    try {
      setIsSavingIdentification(true);

      // Backend expects: { identification: { type, value } }
      const identificationPayload = {
        identification: {
          type: identificationData.type.trim(),
          value: identificationData.value.trim()
        }
      };

      if (identificationDialogMode === 'add') {
        const response = await profileService.addIdentification(profile.id, identificationPayload);
        setProfile(prev => ({
          ...prev,
          identifications: [...(prev.identifications || []), response.data]
        }));
        ToastQueue.positive(t('profile.overview.identificationDialog.addSuccess'), { timeout: 3000 });
      } else {
        await profileService.updateIdentification(profile.id, identificationData.id, identificationPayload);
        setProfile(prev => ({
          ...prev,
          identifications: prev.identifications.map(i =>
            i.id === identificationData.id ? {
              ...i,
              type: identificationPayload.identification.type,
              value: identificationPayload.identification.value
            } : i
          )
        }));
        ToastQueue.positive(t('profile.overview.identificationDialog.updateSuccess'), { timeout: 3000 });
      }

      setIdentificationDialogOpen(false);
    } catch (err) {
      console.error('Failed to save identification:', err);
      ToastQueue.negative(t('profile.overview.identificationDialog.error'), { timeout: 3000 });
    } finally {
      setIsSavingIdentification(false);
    }
  }, [identificationData, identificationDialogMode, profile, profileService, hasIdentificationValidationErrors, t]);

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
        <Heading level={2} styles={headingStyle}>{t('profile.overview.title')}</Heading>

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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noAddresses')}</Text>
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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noEmergencyContacts')}</Text>
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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noEducation')}</Text>
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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noBankAccounts')}</Text>
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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noIdentifications')}</Text>
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
                <Text styles={emptyMessageStyle}>{t('profile.overview.noConsents')}</Text>
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
              isPrimaryActionDisabled={isDeleting}
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
                    isInvalid={!!validationErrors.firstName}
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
                    isInvalid={!!validationErrors.gender}
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
                    isInvalid={!!validationErrors.dob}
                    description={t('profile.overview.editPersonalDialog.dobFormat')}
                  />
                  <TextField
                    label={t('profile.overview.fields.email')}
                    type="email"
                    value={editData.email || ''}
                    onChange={(value) => handleFieldChange('email', value)}
                    errorMessage={validationErrors.email}
                    isInvalid={!!validationErrors.email}
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

        {/* Address Dialog (Add/Edit) */}
        <DialogContainer onDismiss={() => setAddressDialogOpen(false)}>
          {addressDialogOpen && (
            <Dialog>
              <Heading slot="title">
                {addressDialogMode === 'add'
                  ? t('profile.overview.addressDialog.addTitle')
                  : t('profile.overview.addressDialog.editTitle')}
              </Heading>
              <Content>
                <Form>
                  <Picker
                    label={t('profile.overview.fields.addressType')}
                    isRequired
                    necessityIndicator="icon"
                    selectedKey={addressData.type}
                    onSelectionChange={(key) => handleAddressFieldChange('type', key)}
                    placeholder={t('profile.overview.addressDialog.selectType')}
                  >
                    {addressTypeOptions.map((option) => (
                      <PickerItem key={option.id} id={option.id}>
                        {option.name}
                      </PickerItem>
                    ))}
                  </Picker>
                  <TextField
                    label={t('profile.overview.fields.addressLine1')}
                    isRequired
                    necessityIndicator="icon"
                    value={addressData.line1}
                    onChange={(value) => handleAddressFieldChange('line1', value)}
                    placeholder={t('profile.overview.addressDialog.line1Placeholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.addressLine2')}
                    value={addressData.line2 || ''}
                    onChange={(value) => handleAddressFieldChange('line2', value)}
                    placeholder={t('profile.overview.addressDialog.line2Placeholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.suburb')}
                    value={addressData.suburb || ''}
                    onChange={(value) => handleAddressFieldChange('suburb', value)}
                  />
                  <TextField
                    label={t('profile.overview.fields.stateProvince')}
                    value={addressData.stateProvince || ''}
                    onChange={(value) => handleAddressFieldChange('stateProvince', value)}
                  />
                  <TextField
                    label={t('profile.overview.fields.country')}
                    isRequired
                    necessityIndicator="icon"
                    value={addressData.country}
                    onChange={(value) => handleAddressFieldChange('country', value)}
                    placeholder={t('profile.overview.addressDialog.countryPlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.postcode')}
                    value={addressData.postcode || ''}
                    onChange={(value) => handleAddressFieldChange('postcode', value)}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setAddressDialogOpen(false)}>
                  {t('profile.overview.addressDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleSaveAddress}
                  isPending={isSavingAddress}
                  isDisabled={hasAddressValidationErrors}
                >
                  {addressDialogMode === 'add'
                    ? t('profile.overview.addressDialog.add')
                    : t('profile.overview.addressDialog.update')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>

        {/* Emergency Contact Dialog (Add/Edit) */}
        <DialogContainer onDismiss={() => setEmergencyContactDialogOpen(false)}>
          {emergencyContactDialogOpen && (
            <Dialog>
              <Heading slot="title">
                {emergencyContactDialogMode === 'add'
                  ? t('profile.overview.emergencyContactDialog.addTitle')
                  : t('profile.overview.emergencyContactDialog.editTitle')}
              </Heading>
              <Content>
                <Form>
                  <TextField
                    label={t('profile.overview.fields.contactName')}
                    isRequired
                    necessityIndicator="icon"
                    value={emergencyContactData.name}
                    onChange={(value) => handleEmergencyContactFieldChange('name', value)}
                    placeholder={t('profile.overview.emergencyContactDialog.namePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.relationship')}
                    value={emergencyContactData.relationship || ''}
                    onChange={(value) => handleEmergencyContactFieldChange('relationship', value)}
                    placeholder={t('profile.overview.emergencyContactDialog.relationshipPlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.phone')}
                    isRequired
                    necessityIndicator="icon"
                    type="tel"
                    value={emergencyContactData.contactNumber}
                    onChange={(value) => handleEmergencyContactFieldChange('contactNumber', value)}
                    placeholder={t('profile.overview.emergencyContactDialog.phonePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.email')}
                    type="email"
                    value={emergencyContactData.email || ''}
                    onChange={(value) => handleEmergencyContactFieldChange('email', value)}
                    placeholder={t('profile.overview.emergencyContactDialog.emailPlaceholder')}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setEmergencyContactDialogOpen(false)}>
                  {t('profile.overview.emergencyContactDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleSaveEmergencyContact}
                  isPending={isSavingEmergencyContact}
                  isDisabled={hasEmergencyContactValidationErrors}
                >
                  {emergencyContactDialogMode === 'add'
                    ? t('profile.overview.emergencyContactDialog.add')
                    : t('profile.overview.emergencyContactDialog.update')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>

        {/* Education Dialog (Add/Edit) */}
        <DialogContainer onDismiss={() => setEducationDialogOpen(false)}>
          {educationDialogOpen && (
            <Dialog>
              <Heading slot="title">
                {educationDialogMode === 'add'
                  ? t('profile.overview.educationDialog.addTitle')
                  : t('profile.overview.educationDialog.editTitle')}
              </Heading>
              <Content>
                <Form>
                  <TextField
                    label={t('profile.overview.fields.institution')}
                    isRequired
                    necessityIndicator="icon"
                    value={educationData.institution}
                    onChange={(value) => handleEducationFieldChange('institution', value)}
                    placeholder={t('profile.overview.educationDialog.institutionPlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.degree')}
                    isRequired
                    necessityIndicator="icon"
                    value={educationData.degree}
                    onChange={(value) => handleEducationFieldChange('degree', value)}
                    placeholder={t('profile.overview.educationDialog.degreePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.fieldOfStudy')}
                    value={educationData.fieldOfStudy || ''}
                    onChange={(value) => handleEducationFieldChange('fieldOfStudy', value)}
                    placeholder={t('profile.overview.educationDialog.fieldOfStudyPlaceholder')}
                  />
                  <DatePicker
                    label={t('profile.overview.fields.startDate')}
                    value={educationData.startDate}
                    onChange={(value) => handleEducationFieldChange('startDate', value)}
                  />
                  <DatePicker
                    label={t('profile.overview.fields.endDate')}
                    value={educationData.endDate}
                    onChange={(value) => handleEducationFieldChange('endDate', value)}
                  />
                  <TextField
                    label={t('profile.overview.fields.grade')}
                    value={educationData.grade || ''}
                    onChange={(value) => handleEducationFieldChange('grade', value)}
                    placeholder={t('profile.overview.educationDialog.gradePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.certificateNumber')}
                    value={educationData.certificateNumber || ''}
                    onChange={(value) => handleEducationFieldChange('certificateNumber', value)}
                    placeholder={t('profile.overview.educationDialog.certificateNumberPlaceholder')}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setEducationDialogOpen(false)}>
                  {t('profile.overview.educationDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleSaveEducation}
                  isPending={isSavingEducation}
                  isDisabled={hasEducationValidationErrors}
                >
                  {educationDialogMode === 'add'
                    ? t('profile.overview.educationDialog.add')
                    : t('profile.overview.educationDialog.update')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>

        {/* Bank Account Dialog (Add/Edit) */}
        <DialogContainer onDismiss={() => setBankAccountDialogOpen(false)}>
          {bankAccountDialogOpen && (
            <Dialog>
              <Heading slot="title">
                {bankAccountDialogMode === 'add'
                  ? t('profile.overview.bankAccountDialog.addTitle')
                  : t('profile.overview.bankAccountDialog.editTitle')}
              </Heading>
              <Content>
                <Form>
                  <TextField
                    label={t('profile.overview.fields.bankName')}
                    isRequired
                    necessityIndicator="icon"
                    value={bankAccountData.bankName}
                    onChange={(value) => handleBankAccountFieldChange('bankName', value)}
                    placeholder={t('profile.overview.bankAccountDialog.bankNamePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.accountType')}
                    isRequired
                    necessityIndicator="icon"
                    value={bankAccountData.accountType}
                    onChange={(value) => handleBankAccountFieldChange('accountType', value)}
                    placeholder={t('profile.overview.bankAccountDialog.accountTypePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.accountHolderName')}
                    isRequired
                    necessityIndicator="icon"
                    value={bankAccountData.accountHolderName}
                    onChange={(value) => handleBankAccountFieldChange('accountHolderName', value)}
                    placeholder={t('profile.overview.bankAccountDialog.accountHolderNamePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.accountNumber')}
                    isRequired
                    necessityIndicator="icon"
                    value={bankAccountData.accountNumber}
                    onChange={(value) => handleBankAccountFieldChange('accountNumber', value)}
                    placeholder={t('profile.overview.bankAccountDialog.accountNumberPlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.branchCode')}
                    value={bankAccountData.branchCode || ''}
                    onChange={(value) => handleBankAccountFieldChange('branchCode', value)}
                    placeholder={t('profile.overview.bankAccountDialog.branchCodePlaceholder')}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setBankAccountDialogOpen(false)}>
                  {t('profile.overview.bankAccountDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleSaveBankAccount}
                  isPending={isSavingBankAccount}
                  isDisabled={hasBankAccountValidationErrors}
                >
                  {bankAccountDialogMode === 'add'
                    ? t('profile.overview.bankAccountDialog.add')
                    : t('profile.overview.bankAccountDialog.update')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>

        {/* Identification Dialog (Add/Edit) */}
        <DialogContainer onDismiss={() => setIdentificationDialogOpen(false)}>
          {identificationDialogOpen && (
            <Dialog>
              <Heading slot="title">
                {identificationDialogMode === 'add'
                  ? t('profile.overview.identificationDialog.addTitle')
                  : t('profile.overview.identificationDialog.editTitle')}
              </Heading>
              <Content>
                <Form>
                  <TextField
                    label={t('profile.overview.fields.idType')}
                    isRequired
                    necessityIndicator="icon"
                    value={identificationData.type}
                    onChange={(value) => handleIdentificationFieldChange('type', value)}
                    placeholder={t('profile.overview.identificationDialog.typePlaceholder')}
                  />
                  <TextField
                    label={t('profile.overview.fields.idNumber')}
                    isRequired
                    necessityIndicator="icon"
                    value={identificationData.value}
                    onChange={(value) => handleIdentificationFieldChange('value', value)}
                    placeholder={t('profile.overview.identificationDialog.valuePlaceholder')}
                  />
                </Form>
              </Content>
              <ButtonGroup>
                <Button variant="secondary" onPress={() => setIdentificationDialogOpen(false)}>
                  {t('profile.overview.identificationDialog.cancel')}
                </Button>
                <Button
                  variant="accent"
                  onPress={handleSaveIdentification}
                  isPending={isSavingIdentification}
                  isDisabled={hasIdentificationValidationErrors}
                >
                  {identificationDialogMode === 'add'
                    ? t('profile.overview.identificationDialog.add')
                    : t('profile.overview.identificationDialog.update')}
                </Button>
              </ButtonGroup>
            </Dialog>
          )}
        </DialogContainer>
      </div>
    </ProfileLayout >
  );
}
