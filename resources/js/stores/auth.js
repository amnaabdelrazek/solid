import { defineStore } from 'pinia';
import { authService } from '../services/auth';
import { setLocale } from '../i18n';

export const useAuthStore = defineStore('auth', {
    state: () => ({
        user: JSON.parse(localStorage.getItem('auth_user') || 'null'),
        token: localStorage.getItem('auth_token') || null,
    }),

    getters: {
        isAuthenticated: (state) => !!state.token,
        role: (state) => state.user?.role || null,
        isAdmin: (state) => state.user?.role === 'admin',
        isInstructor: (state) => state.user?.role === 'instructor',
        isAddict: (state) => state.user?.role === 'addict',
        userName: (state) => state.user?.display_name || state.user?.username || '',
        userLocale: (state) => state.user?.preferred_language || 'ar',
    },

    actions: {
        applyLocale(user) {
            if (user?.preferred_language) {
                setLocale(user.preferred_language);
            }
        },

        async login(credentials) {
            const response = await authService.login(credentials);
            const { token, user } = response.data;
            this.token = token;
            this.user = user;
            localStorage.setItem('auth_token', token);
            localStorage.setItem('auth_user', JSON.stringify(user));
            this.applyLocale(user);
            return user;
        },

        async register(data) {
            const response = await authService.register(data);
            const { token, user } = response.data;
            this.token = token;
            this.user = user;
            localStorage.setItem('auth_token', token);
            localStorage.setItem('auth_user', JSON.stringify(user));
            this.applyLocale(user);
            return user;
        },

        async verifyOtp(data) {
            const response = await authService.verifyOtp(data);
            if (this.user) {
                this.user.is_active = true;
                localStorage.setItem('auth_user', JSON.stringify(this.user));
            }
            return response;
        },

        async fetchMe() {
            try {
                const response = await authService.me();
                if (response.data && response.data.user) {
                    this.user = response.data.user;
                    localStorage.setItem('auth_user', JSON.stringify(this.user));
                    this.applyLocale(this.user);
                } else if (response.data) {
                    this.user = response.data;
                    localStorage.setItem('auth_user', JSON.stringify(this.user));
                    this.applyLocale(this.user);
                }
                return this.user;
            } catch {
                this.logout();
                throw new Error('Session expired');
            }
        },

        async logout() {
            try {
                await authService.logout();
            } catch {
            } finally {
                this.clearAuth();
            }
        },

        clearAuth() {
            this.token = null;
            this.user = null;
            localStorage.removeItem('auth_token');
            localStorage.removeItem('auth_user');
        },
    },
});
