<template>
    <div class="min-h-screen bg-gray-50 dark:bg-gray-900" :dir="$i18n.locale === 'ar' ? 'rtl' : 'ltr'">
        <Navbar @toggle-sidebar="appStore.toggleSidebar()" />
        <div class="flex">
            <Sidebar v-if="appStore.sidebarOpen" />
            <main class="flex-1 p-6">
                <slot />
            </main>
        </div>
        <div class="fixed top-4 end-4 z-50 space-y-2">
            <div
                v-for="toast in appStore.toasts"
                :key="toast.id"
                :class="[
                    'px-4 py-3 rounded-lg shadow-lg text-white text-sm max-w-sm transition-all',
                    toast.type === 'success' ? 'bg-green-600' : '',
                    toast.type === 'error' ? 'bg-red-600' : '',
                    toast.type === 'info' ? 'bg-blue-600' : '',
                ]"
            >
                {{ toast.message }}
            </div>
        </div>
    </div>
</template>

<script>
import Navbar from '../components/Navbar.vue';
import Sidebar from '../components/Sidebar.vue';
import { useAppStore } from '../stores/app';

export default {
    name: 'DefaultLayout',
    components: { Navbar, Sidebar },
    setup() {
        const appStore = useAppStore();
        return { appStore };
    },
};
</script>
