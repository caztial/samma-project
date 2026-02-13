import React, { useState } from 'react';
import { Box, BlockStack, InlineStack, Text, Form, TextField, Button, Link, Banner } from '@shopify/polaris';
import { authService } from '../services/AuthService';

const Register = ({ onSwitchToLogin, onRegisterSuccess }) => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
    // Clear error when user starts typing
    if (error) setError('');
  };

  const validateForm = () => {
    if (!formData.firstName.trim()) {
      setError('First name is required');
      return false;
    }
    if (!formData.lastName.trim()) {
      setError('Last name is required');
      return false;
    }
    if (!formData.email.trim()) {
      setError('Email is required');
      return false;
    }
    if (!/\S+@\S+\.\S+/.test(formData.email)) {
      setError('Please enter a valid email address');
      return false;
    }
    if (!formData.password) {
      setError('Password is required');
      return false;
    }
    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters long');
      return false;
    }
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess(false);

    if (!validateForm()) {
      setLoading(false);
      return;
    }

    try {
      const { confirmPassword, ...registerData } = formData;
      const result = await authService.register(registerData);
      setSuccess(true);
      onRegisterSuccess(result);
    } catch (err) {
      setError(err.message || 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box padding="400" background="bg" borderRadius="200" maxWidth="400px" margin="0 auto">
      <BlockStack gap="300">
        <BlockStack gap="200" align="center">
          <Text variant="headingLg" as="h1">Create account</Text>
          <Text color="subdued">Join us today</Text>
        </BlockStack>

        {error && (
          <Banner tone="critical" onDismiss={() => setError('')}>
            {error}
          </Banner>
        )}

        {success && (
          <Banner tone="success" onDismiss={() => setSuccess(false)}>
            Account created successfully! Please check your email to verify your account.
          </Banner>
        )}

        <Form onSubmit={handleSubmit}>
          <BlockStack gap="base">
            <BlockStack gap="base">
              <TextField
                label="First Name"
                value={formData.firstName}
                onChange={(value) => handleChange('firstName', value)}
                required
                autoComplete="given-name"
                placeholder="Enter your first name"
              />

              <TextField
                label="Last Name"
                value={formData.lastName}
                onChange={(value) => handleChange('lastName', value)}
                required
                autoComplete="family-name"
                placeholder="Enter your last name"
              />
            </BlockStack>

            <TextField
              label="Email"
              type="email"
              value={formData.email}
              onChange={(value) => handleChange('email', value)}
              required
              autoComplete="email"
              placeholder="Enter your email address"
            />

            <TextField
              label="Password"
              type="password"
              value={formData.password}
              onChange={(value) => handleChange('password', value)}
              required
              autoComplete="new-password"
              placeholder="Create a password"
              helpText="Password must be at least 6 characters"
            />

            <TextField
              label="Confirm Password"
              type="password"
              value={formData.confirmPassword}
              onChange={(value) => handleChange('confirmPassword', value)}
              required
              autoComplete="new-password"
              placeholder="Confirm your password"
            />

            <BlockStack gap="200">
              <Button
                submit
                primary
                loading={loading}
                fullWidth
              >
                {loading ? 'Creating account...' : 'Create account'}
              </Button>

              <InlineStack gap="200" align="center" justify="center">
                <Text color="subdued">Already have an account?</Text>
                <Link url="#" onClick={(e) => { e.preventDefault(); onSwitchToLogin(); }}>
                  Sign in
                </Link>
              </InlineStack>
            </BlockStack>
          </BlockStack>
        </Form>
      </BlockStack>
    </Box>
  );
};

export default Register;