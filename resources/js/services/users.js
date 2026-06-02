import api from './api';

export const usersService = {
    index() {
        return api.get('/v1/users');
    },

    show(id) {
        return api.get(`/v1/users/${id}`);
    },

    store(data) {
        return api.post('/v1/users', data);
    },

    update(id, data) {
        return api.put(`/v1/users/${id}`, data);
    },

    destroy(id) {
        return api.delete(`/v1/users/${id}`);
    },
};
