import api from './api';

export const notificationsService = {
    index() {
        return api.get('/v1/notifications');
    },

    show(id) {
        return api.get(`/v1/notifications/${id}`);
    },

    store(data) {
        return api.post('/v1/notifications', data);
    },

    update(id, data) {
        return api.put(`/v1/notifications/${id}`, data);
    },

    destroy(id) {
        return api.delete(`/v1/notifications/${id}`);
    },
};
