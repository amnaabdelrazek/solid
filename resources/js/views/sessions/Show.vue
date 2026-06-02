<template>
    <div>
        <div class="mb-4">
            <router-link to="/sessions" class="text-sm text-blue-600 hover:text-blue-500">&larr; {{ $t('sessions.back') }}</router-link>
        </div>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('common.loading') }}</div>

        <div v-else-if="!session.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('sessions.not_found') }}
        </div>

        <template v-else>
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-6">
                <div class="flex items-center justify-between mb-4">
                    <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                        {{ $t('sessions.title') }} #{{ session.session_number }}
                    </h1>
                    <span :class="statusBadge(session.status)" class="px-3 py-1 rounded-full text-sm font-medium">
                        {{ $t('status.' + session.status) || session.status }}
                    </span>
                </div>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('sessions.type') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ session.session_type }}</div>
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('sessions.scheduled') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ formatDate(session.scheduled_at) }}</div>
                    </div>
                    <div v-if="session.started_at">
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('sessions.started') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ formatDate(session.started_at) }}</div>
                    </div>
                    <div v-if="session.duration_minutes">
                        <label class="block text-sm font-medium text-gray-500 dark:text-gray-400">{{ $t('sessions.duration') }}</label>
                        <div class="text-gray-900 dark:text-white">{{ $t('sessions.minutes', { minutes: session.duration_minutes }) }}</div>
                    </div>
                </div>

                <div class="mt-6 flex gap-3" v-if="isInstructorOrAdmin">
                    <button v-if="session.status === 'scheduled'" @click="handleStart" :disabled="actionLoading" class="px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg text-sm font-medium disabled:opacity-50">
                        {{ $t('sessions.start') }}
                    </button>
                    <button v-if="session.status === 'active'" @click="handleEnd" :disabled="actionLoading" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded-lg text-sm font-medium disabled:opacity-50">
                        {{ $t('sessions.end') }}
                    </button>
                </div>

                <div class="mt-4 flex gap-3">
                    <button v-if="session.status === 'scheduled' || session.status === 'active'" @click="handleJoin" :disabled="actionLoading" class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium disabled:opacity-50">
                        {{ $t('sessions.join') }}
                    </button>
                    <button v-if="session.status === 'active'" @click="handleLeave" :disabled="actionLoading" class="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg text-sm font-medium disabled:opacity-50">
                        {{ $t('sessions.leave') }}
                    </button>
                </div>
            </div>
        </template>
    </div>
</template>

<script>
import { sessionsService } from '../../services/sessions';
import { useAppStore } from '../../stores/app';
import { useAuthStore } from '../../stores/auth';
import { useI18n } from 'vue-i18n';
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';

export default {
    name: 'SessionsShow',
    setup() {
        const route = useRoute();
        const authStore = useAuthStore();
        const appStore = useAppStore();
        const { t } = useI18n();
        const session = ref({});
        const loading = ref(true);
        const actionLoading = ref(false);
        const isInstructorOrAdmin = computed(() => authStore.isInstructor || authStore.isAdmin);

        function statusBadge(status) {
            const colors = {
                scheduled: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/50 dark:text-yellow-300',
                active: 'bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-300',
                completed: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
            };
            return colors[status] || 'bg-gray-100 text-gray-800';
        }

        function formatDate(dateStr) {
            if (!dateStr) return '—';
            return new Date(dateStr).toLocaleDateString(undefined, {
                weekday: 'short', year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
            });
        }

        async function fetchSession() {
            loading.value = true;
            try {
                const resp = await sessionsService.show(route.params.id);
                session.value = resp.data || resp;
            } catch {
                session.value = {};
            } finally {
                loading.value = false;
            }
        }

        async function handleJoin() {
            actionLoading.value = true;
            try {
                await sessionsService.join(route.params.id);
                appStore.addToast({ message: t('sessions.joined'), type: 'success' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to join', type: 'error' });
            } finally {
                actionLoading.value = false;
            }
        }

        async function handleLeave() {
            actionLoading.value = true;
            try {
                await sessionsService.leave(route.params.id);
                appStore.addToast({ message: t('sessions.left'), type: 'info' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to leave', type: 'error' });
            } finally {
                actionLoading.value = false;
            }
        }

        async function handleStart() {
            actionLoading.value = true;
            try {
                await sessionsService.start(route.params.id);
                session.value.status = 'active';
                appStore.addToast({ message: t('sessions.started_msg'), type: 'success' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to start', type: 'error' });
            } finally {
                actionLoading.value = false;
            }
        }

        async function handleEnd() {
            actionLoading.value = true;
            try {
                await sessionsService.end(route.params.id);
                session.value.status = 'completed';
                appStore.addToast({ message: t('sessions.ended_msg'), type: 'success' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to end', type: 'error' });
            } finally {
                actionLoading.value = false;
            }
        }

        onMounted(fetchSession);

        return { session, loading, actionLoading, isInstructorOrAdmin, statusBadge, formatDate, handleJoin, handleLeave, handleStart, handleEnd };
    },
};
</script>
