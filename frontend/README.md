# Samma Project Frontend

This is the frontend application for the Samma Project, a meditation session management system built with React and Shopify Polaris Web Components.

## Features

- **Authentication System**: Login, registration, and password reset functionality
- **Role-based Dashboards**: Different interfaces for Participants, Admins, and Presenters
- **Modern UI**: Built with Shopify Polaris Web Components for consistent design
- **Responsive Design**: Works on desktop, tablet, and mobile devices
- **State Management**: Simple state management with React hooks and context
- **Routing**: Client-side routing with React Router

## Tech Stack

- **React 18**: Modern React with hooks and functional components
- **Shopify Polaris Web Components**: Design system and UI components
- **React Router**: Client-side routing
- **Axios**: HTTP client for API communication
- **CSS-in-JS**: Styled components for custom styling

## Project Structure

```
frontend/
├── public/                 # Static assets
├── src/
│   ├── components/         # Shared components
│   ├── features/           # Feature-based organization
│   │   └── auth/          # Authentication feature
│   │       ├── components/ # Auth-specific components
│   │       └── services/   # Auth API services
│   ├── hooks/             # Custom React hooks
│   ├── pages/             # Page components
│   ├── services/          # API services
│   ├── store/             # State management
│   ├── types/             # TypeScript type definitions
│   ├── utils/             # Utility functions
│   ├── App.jsx            # Main application component
│   ├── App.css            # Global styles
│   └── index.js           # Application entry point
├── package.json
└── README.md
```

## Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SammaProject/frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Environment Setup**
   Create a `.env` file in the frontend root directory:
   ```env
   REACT_APP_API_URL=http://localhost:5275/api
   ```

## Development

### Starting the Development Server

```bash
npm start
```

The application will start on `http://localhost:3000`.

### Available Scripts

- `npm start` - Start the development server
- `npm run build` - Build the application for production
- `npm test` - Run tests (if configured)
- `npm run eject` - Eject from Create React App (not recommended)

## API Integration

The frontend is designed to integrate with the backend API. Currently, the AuthService provides mock implementations that will need to be connected to the actual backend endpoints.

### API Endpoints

The following endpoints are expected from the backend:

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/forgot-password` - Password reset request
- `POST /api/auth/reset-password` - Password reset confirmation
- `GET /api/auth/me` - Get current user info
- `POST /api/auth/refresh` - Token refresh

## Authentication Flow

1. **Login**: Users enter email and password
2. **Token Storage**: Access and refresh tokens stored in localStorage
3. **Protected Routes**: Routes check authentication status
4. **Token Refresh**: Automatic token refresh on 401 errors
5. **Logout**: Clears stored tokens and redirects to login

## Role-based Access

The application supports three user roles:

- **Participant**: View sessions, track progress, download materials
- **Presenter**: Create sessions, manage materials, view analytics
- **Admin**: Full system management, user management, system monitoring

## Styling

The application uses Shopify Polaris Web Components for consistent UI design. Custom styles are added in `App.css` for:

- Responsive design adjustments
- Custom animations and transitions
- Accessibility improvements
- Print styles

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Development Guidelines

### Component Structure

- Use functional components with hooks
- Follow the feature-based organization
- Keep components focused and reusable
- Use Polaris components for consistency

### State Management

- Use React hooks (`useState`, `useEffect`) for local state
- Use context for shared state across components
- Keep state updates minimal and predictable

### Error Handling

- Use try-catch blocks for async operations
- Display user-friendly error messages
- Log errors for debugging

### Performance

- Use memoization for expensive calculations
- Implement lazy loading for large lists
- Optimize image loading
- Minimize re-renders

## Deployment

### Build for Production

```bash
npm run build
```

This creates an optimized build in the `build/` directory.

### Environment Variables

Set the following environment variables for production:

- `REACT_APP_API_URL` - Backend API URL

## Troubleshooting

### Common Issues

1. **CORS Errors**: Ensure backend allows requests from frontend origin
2. **Authentication Failures**: Check token storage and API endpoints
3. **Component Rendering**: Verify Polaris components are properly imported

### Development Tips

- Use browser developer tools for debugging
- Check console for warnings and errors
- Use React DevTools extension for component inspection

## Contributing

1. Follow the existing code style and patterns
2. Write clear, descriptive commit messages
3. Test changes thoroughly before submitting
4. Update documentation for new features

## License

[Add license information here]