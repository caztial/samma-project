import { useLocale } from './LocaleContext';

/**
 * Custom hook for accessing translations
 * @returns {Object} Translation utilities
 */
export function useTranslation() {
  const { t, locale, setLocale, availableLocales } = useLocale();

  /**
   * Get a nested translation value by dot-notation path
   * @param {string} path - Dot-notation path (e.g., 'login.title')
   * @param {Object} params - Optional parameters for interpolation
   * @returns {string} Translated string
   */
  const translate = (path, params = {}) => {
    const keys = path.split('.');
    let value = t;

    for (const key of keys) {
      if (value && typeof value === 'object' && key in value) {
        value = value[key];
      } else {
        console.warn(`Translation not found: ${path}`);
        return path;
      }
    }

    if (typeof value !== 'string') {
      console.warn(`Translation value is not a string: ${path}`);
      return path;
    }

    // Interpolate parameters if provided
    let result = value;
    Object.keys(params).forEach(param => {
      result = result.replace(new RegExp(`\\{${param}\\}`, 'g'), params[param]);
    });

    return result;
  };

  return {
    t: translate,
    locale,
    setLocale,
    availableLocales
  };
}

export default useTranslation;