import React, { useState } from 'react';
import { Box, BlockStack, InlineStack, Text, Form, TextField, Button, Link, Banner } from '@shopify/polaris';
import { authService } from '../services/AuthService';

const ForgotPassword = ({ onSwitchToLogin }) => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    if (!email.trim()) {
      setError('Please enter your email address');
      setLoading(false);
      return;
    }

    if (!/\S+@\S+\.\S+/.test(email)) {
      setError('Please enter a valid email address');
      setLoading(false);
      return;
    }

    try {
      await authService.forgotPassword(email);
      setSuccess(true);
    } catch (err) {
      setError(err.message || 'Failed to send password reset email. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box padding="400" background="bg" borderRadius="200" maxWidth="400px" margin="0 auto">
      <BlockStack gap="300">
        <BlockStack gap="200" align="center">
          <Text variant="headingLg" as="h1">Reset password</Text>
          <Text color="subdued">Enter your email to receive reset instructions</Text>
        </BlockStack>

        {error && (
          <Banner tone="critical" onDismiss={() => setError('')}>
            {error}
          </Banner>
        )}

        {success ? (
          <Banner tone="success" onDismiss={() => setSuccess(false)}>
            Password reset instructions have been sent to your email address. Please check your inbox.
          </Banner>
        ) : (
          <Form onSubmit={handleSubmit}>
            <BlockStack gap="base">
              <TextField
                label="Email Address"
                type="email"
                value={email}
                onChange={(value) => setEmail(value)}
                required
                autoComplete="email"
                placeholder="Enter your email address"
              />

              <BlockStack gap="200">
                <Button
                  submit
                  primary
                  loading={loading}
                  fullWidth
                >
                  {loading ? 'Sending...' : 'Send reset instructions'}
                </Button>

                <InlineStack gap="200" align="center" justify="center">
                  <Link url="#" onClick={(e) => { e.preventDefault(); onSwitchToLogin(); }}>
                    Back to sign in
                  </Link>
                </InlineStack>
              </BlockStack>
            </BlockStack>
          </Form>
        )}
      </BlockStack>
    </Box>
  );
};

export default ForgotPassword;