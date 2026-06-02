import api from './api';

export const sessionsService = {
    index() {
        return api.get('/sessions');
    },

    show(id) {
        return api.get(`/sessions/${id}`);
    },

    store(data) {
        return api.post('/sessions', data);
    },

    join(id) {
        return api.post(`/sessions/${id}/join`);
    },

    leave(id) {
        return api.post(`/sessions/${id}/leave`);
    },

    start(id) {
        return api.post(`/sessions/${id}/start`);
    },

    end(id) {
        return api.post(`/sessions/${id}/end`);
    },
};
