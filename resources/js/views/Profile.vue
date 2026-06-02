<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('profile.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('profile.loading') }}</div>

        <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 max-w-2xl">
            <div class="space-y-4">
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('auth.display_name') }}</label>
                    <div class="text-gray-900 dark:text-white font-medium">{{ user.display_name }}</div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.username') }}</label>
                    <div class="text-gray-900 dark:text-white">{{ user.username || '—' }}</div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.mobile') }}</label>
                    <div class="text-gray-900 dark:text-white">{{ user.mobile_number }}</div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.role') }}</label>
                    <div class="text-gray-900 dark:text-white capitalize">{{ user.role }}</div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.language') }}</label>
                    <div class="text-gray-900 dark:text-white">{{ user.preferred_language === 'ar' ? 'العربية' : 'English' }}</div>
                </div>
                <div>
                    <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.member_since') }}</label>
                    <div class="text-gray-900 dark:text-white">{{ formatDate(user.created_at) }}</div>
                </div>
            </div>

            <div v-if="addictionProfile" class="mt-8 pt-6 border-t border-gray-200 dark:border-gray-700">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">{{ $t('profile.addiction_profile') }}</h2>
                <div class="space-y-3">
                    <div>
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.days_clean') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ addictionProfile.days_clean ?? '—' }}</div>
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.had_prior_treatment') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ addictionProfile.had_prior_treatment ? $t('auth.yes') : $t('auth.no') }}</div>
                    </div>
                    <div v-if="addictionProfile.addiction_reason">
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('profile.reason') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ addictionProfile.addiction_reason }}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { useAuthStore } from '../stores/auth';
import { onMounted, ref } from 'vue';

export default {
    name: 'ProfileView',
    setup() {
        const authStore = useAuthStore();
        const user = ref({});
        const addictionProfile = ref(null);
        const loading = ref(true);

        function formatDate(dateStr) {
            if (!dateStr) return '—';
            return new Date(dateStr).toLocaleDateString(undefined, {
                year: 'numeric', month: 'long', day: 'numeric',
            });
        }

        onMounted(async () => {
            try {
                const response = await authStore.fetchMe();
                user.value = response;
                addictionProfile.value = response.addiction_profile || null;
            } catch {
                user.value = authStore.user || {};
            } finally {
                loading.value = false;
            }
        });

        return { user, addictionProfile, loading, formatDate };
    },
};
</script>
