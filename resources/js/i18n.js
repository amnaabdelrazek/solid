import { createI18n } from 'vue-i18n';
import en from './lang/en.json';
import ar from './lang/ar.json';

const savedLocale = localStorage.getItem('locale') || 'ar';

export const i18n = createI18n({
    legacy: false,
    locale: savedLocale,
    fallbackLocale: 'en',
    messages: { en, ar },
});

export function setLocale(locale) {
    i18n.global.locale.value = locale;
    localStorage.setItem('locale', locale);
    document.documentElement.dir = locale === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = locale;
}

setLocale(savedLocale);
