import { createContext, useContext, useState, useEffect, useMemo } from 'react';
import enUS from './locales/en-US.json';
import siLK from './locales/si-LK.json';

// Available translations
const translations = {
  'en-US': enUS,
  'si-LK': siLK
};

// Available locales for the S2 Provider
export const locales = ['en-US', 'si-LK'];

// Create the context
const LocaleContext = createContext(undefined);

// Local storage key for persisting locale preference
const LOCALE_STORAGE_KEY = 'dhamma-locale';

// Provider component
export function LocaleProvider({ children }) {
  // Initialize locale from localStorage or default to en-US
  const [locale, setLocale] = useState(() => {
    const saved = localStorage.getItem(LOCALE_STORAGE_KEY);
    return saved && translations[saved] ? saved : 'en-US';
  });

  // Persist locale changes to localStorage
  useEffect(() => {
    localStorage.setItem(LOCALE_STORAGE_KEY, locale);
  }, [locale]);

  // Get the current translations
  const t = useMemo(() => translations[locale], [locale]);

  // Function to change locale
  const changeLocale = (newLocale) => {
    if (translations[newLocale]) {
      setLocale(newLocale);
    }
  };

  const value = useMemo(() => ({
    locale,
    setLocale: changeLocale,
    t,
    availableLocales: locales
  }), [locale, t]);

  return (
    <LocaleContext.Provider value={value}>
      {children}
    </LocaleContext.Provider>
  );
}

// Custom hook to use the locale context
export function useLocale() {
  const context = useContext(LocaleContext);
  if (context === undefined) {
    throw new Error('useLocale must be used within a LocaleProvider');
  }
  return context;
}