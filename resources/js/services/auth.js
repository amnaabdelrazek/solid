import api from './api';

export const authService = {
    register(data) {
        return api.post('/auth/register', data);
    },

    verifyOtp(data) {
        return api.post('/auth/verify', data);
    },

    login(data) {
        return api.post('/auth/login', data);
    },

    logout() {
        return api.post('/auth/logout');
    },

    me() {
        return api.get('/auth/me');
    },

    forgotPassword(mobileNumber) {
        return api.post('/auth/forgot-password', { mobile_number: mobileNumber });
    },

    verifyForgotOtp(data) {
        return api.post('/auth/verify-forgot-otp', data);
    },

    resetPassword(data) {
        return api.post('/auth/reset-password', data);
    },
};
