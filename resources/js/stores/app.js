import { defineStore } from 'pinia';

export const useAppStore = defineStore('app', {
    state: () => ({
        sidebarOpen: true,
        toasts: [],
        loading: false,
    }),

    actions: {
        toggleSidebar() {
            this.sidebarOpen = !this.sidebarOpen;
        },

        addToast({ message, type = 'success', duration = 5000 }) {
            const id = Date.now();
            this.toasts.push({ id, message, type });
            setTimeout(() => {
                this.removeToast(id);
            }, duration);
        },

        removeToast(id) {
            this.toasts = this.toasts.filter((t) => t.id !== id);
        },

        setLoading(val) {
            this.loading = val;
        },
    },
});
