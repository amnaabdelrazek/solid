import api from './api';

export const lookupsService = {
    substances() {
        return api.get('/lookups/substances');
    },

    byType(type) {
        return api.get(`/lookups/${type}`);
    },
};
