import api from './api';

export const paymentsService = {
    initiate(sessionId) {
        return api.post(`/payments/initiate/${sessionId}`);
    },

    history() {
        return api.get('/payments/history');
    },
};
