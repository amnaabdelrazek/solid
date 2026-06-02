import api from './api';

export const settingsService = {
    index() {
        return api.get('/v1/settings');
    },

    show(key) {
        return api.get(`/v1/settings/${key}`);
    },

    update(key, value) {
        return api.put(`/v1/settings/${key}`, { value });
    },
};
