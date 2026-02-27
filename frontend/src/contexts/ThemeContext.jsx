import { createContext, useContext, useState, useEffect, useMemo } from 'react';

const ThemeContext = createContext(undefined);
const THEME_STORAGE_KEY = 'dhamma-color-scheme';

export function ThemeProvider({ children }) {
  const [colorScheme, setColorScheme] = useState(() => {
    const saved = localStorage.getItem(THEME_STORAGE_KEY);
    if (saved === 'dark' || saved === 'light') return saved;
    // Respect OS preference as default
    if (window.matchMedia?.('(prefers-color-scheme: dark)').matches) return 'dark';
    return 'light';
  });

  useEffect(() => {
    localStorage.setItem(THEME_STORAGE_KEY, colorScheme);
  }, [colorScheme]);

  const toggleColorScheme = () =>
    setColorScheme(prev => (prev === 'light' ? 'dark' : 'light'));

  const value = useMemo(
    () => ({ colorScheme, setColorScheme, toggleColorScheme }),
    [colorScheme]
  );

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) throw new Error('useTheme must be used within ThemeProvider');
  return context;
}
