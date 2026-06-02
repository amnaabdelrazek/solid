<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('dashboard.title') }}</h1>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
                <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">{{ $t('dashboard.welcome') }}</div>
                <div class="text-xl font-semibold text-gray-900 dark:text-white">{{ authStore.userName }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400 capitalize">{{ authStore.role }}</div>
            </div>
            <router-link to="/sessions" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">{{ $t('dashboard.upcoming_sessions') }}</div>
                <div class="text-xl font-semibold text-gray-900 dark:text-white">{{ stats.sessionsCount }}</div>
            </router-link>
            <router-link to="/groups" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">{{ $t('dashboard.my_group') }}</div>
                <div class="text-xl font-semibold text-gray-900 dark:text-white">{{ stats.groupName || $t('dashboard.not_assigned') }}</div>
            </router-link>
            <router-link to="/payments" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                <div class="text-sm text-gray-500 dark:text-gray-400 mb-1">{{ $t('dashboard.payment_history') }}</div>
                <div class="text-xl font-semibold text-gray-900 dark:text-white">{{ $t('dashboard.payments_count', { count: stats.paymentsCount }) }}</div>
            </router-link>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">{{ $t('dashboard.quick_actions') }}</h2>
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                <router-link to="/sessions" class="flex items-center gap-3 p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                    <span class="text-blue-600"><svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg></span>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ $t('dashboard.browse_sessions') }}</span>
                </router-link>
                <router-link to="/groups" class="flex items-center gap-3 p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                    <span class="text-green-600"><svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" /></svg></span>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ $t('nav.groups') }}</span>
                </router-link>
                <router-link to="/profile" class="flex items-center gap-3 p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                    <span class="text-purple-600"><svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg></span>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ $t('dashboard.view_profile') }}</span>
                </router-link>
            </div>
        </div>
    </div>
</template>

<script>
import { useAuthStore } from '../stores/auth';
import { sessionsService } from '../services/sessions';
import { groupsService } from '../services/groups';
import { paymentsService } from '../services/payments';
import { onMounted, reactive } from 'vue';

export default {
    name: 'DashboardView',
    setup() {
        const authStore = useAuthStore();
        const stats = reactive({
            sessionsCount: 0,
            groupName: null,
            paymentsCount: 0,
        });

        onMounted(async () => {
            try {
                const sessionsResp = await sessionsService.index();
                stats.sessionsCount = Array.isArray(sessionsResp.data) ? sessionsResp.data.length : 0;
            } catch {}
            try {
                const groupsResp = await groupsService.myGroup();
                if (groupsResp.data?.name) stats.groupName = groupsResp.data.name;
            } catch {}
            try {
                const paymentsResp = await paymentsService.history();
                stats.paymentsCount = Array.isArray(paymentsResp.data) ? paymentsResp.data.length : 0;
            } catch {}
        });

        return { authStore, stats };
    },
};
</script>
