import api from './api';

export const groupsService = {
    index() {
        return api.get('/groups/');
    },

    myGroup() {
        return api.get('/groups/my');
    },

    subscribe(data) {
        return api.post('/groups/subscribe', data);
    },
};
