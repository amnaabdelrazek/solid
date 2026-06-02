import api from './api';

export const recommendationsService = {
    index() {
        return api.get('/recommendations');
    },
};
