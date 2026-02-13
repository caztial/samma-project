import React, { useState } from 'react';
import { Box, BlockStack, InlineStack, Text, Form, TextField, Button, Link, Banner } from '@shopify/polaris';
import { authService } from '../services/AuthService';

const Login = ({ onSwitchToRegister, onSwitchToForgotPassword, onLoginSuccess }) => {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
    // Clear error when user starts typing
    if (error) setError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const result = await authService.login(formData.email, formData.password);
      onLoginSuccess(result);
    } catch (err) {
      setError(err.message || 'Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box padding="400" background="bg" borderRadius="200" maxWidth="400px" margin="0 auto">
      <BlockStack gap="300">
        <BlockStack gap="200" align="center">
          <Text variant="headingLg" as="h1">Sign in to your account</Text>
          <Text color="subdued">Welcome back! Please enter your credentials.</Text>
        </BlockStack>

        {error && (
          <Banner tone="critical" onDismiss={() => setError('')}>
            {error}
          </Banner>
        )}

        <Form onSubmit={handleSubmit}>
          <BlockStack gap="base">
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
              autoComplete="current-password"
              placeholder="Enter your password"
            />

            <BlockStack gap="200">
              <Button
                submit
                primary
                loading={loading}
                fullWidth
              >
                {loading ? 'Signing in...' : 'Sign in'}
              </Button>

              <InlineStack gap="200" align="center" justify="space-between">
                <Link url="#" onClick={(e) => { e.preventDefault(); onSwitchToForgotPassword(); }}>
                  Forgot password?
                </Link>
                <Text color="subdued">Don't have an account?</Text>
                <Link url="#" onClick={(e) => { e.preventDefault(); onSwitchToRegister(); }}>
                  Sign up
                </Link>
              </InlineStack>
            </BlockStack>
          </BlockStack>
        </Form>
      </BlockStack>
    </Box>
  );
};

export default Login;