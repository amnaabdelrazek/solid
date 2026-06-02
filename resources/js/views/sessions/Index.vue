<template>
    <div>
        <div class="flex items-center justify-between mb-6">
            <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t('sessions.title') }}</h1>
            <div v-if="isInstructorOrAdmin" class="flex gap-2">
                <button @click="showCreateForm = true" class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium">
                    {{ $t('sessions.create') }}
                </button>
            </div>
        </div>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('sessions.loading') }}</div>

        <div v-else-if="sessions.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('sessions.empty') }}
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="session in sessions" :key="session.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-md transition-shadow">
                <div class="flex items-center justify-between mb-2">
                    <span :class="statusBadge(session.status)" class="px-2 py-1 rounded-full text-xs font-medium">
                        {{ $t('status.' + session.status) || session.status }}
                    </span>
                    <span class="text-xs text-gray-500 dark:text-gray-400">#{{ session.session_number }}</span>
                </div>
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-1">{{ session.session_type }}</h3>
                <p class="text-sm text-gray-500 dark:text-gray-400 mb-4">
                    {{ formatDate(session.scheduled_at) }}
                </p>
                <div class="flex items-center justify-between">
                    <span class="text-xs text-gray-500 dark:text-gray-400">{{ $t('sessions.minutes', { minutes: session.duration_minutes || '—' }) }}</span>
                    <router-link :to="`/sessions/${session.id}`" class="text-sm text-blue-600 hover:text-blue-500 font-medium">
                        {{ $t('sessions.details') }} →
                    </router-link>
                </div>
            </div>
        </div>

        <div v-if="showCreateForm" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" @click.self="showCreateForm = false">
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow-xl p-6 w-full max-w-md mx-4">
                <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">{{ $t('sessions.create_title') }}</h2>
                <form @submit.prevent="handleCreate" class="space-y-4">
                    <div v-if="createError" class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 p-3 rounded">{{ createError }}</div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('sessions.session_type') }}</label>
                        <input v-model="createForm.session_type" type="text" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('sessions.scheduled_at') }}</label>
                        <input v-model="createForm.scheduled_at" type="datetime-local" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white" />
                    </div>
                    <div class="flex gap-3 justify-end">
                        <button type="button" @click="showCreateForm = false" class="px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg">{{ $t('sessions.cancel') }}</button>
                        <button type="submit" :disabled="creating" class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-sm font-medium disabled:opacity-50">
                            {{ creating ? $t('sessions.creating') : $t('sessions.create') }}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>

<script>
import { sessionsService } from '../../services/sessions';
import { useAppStore } from '../../stores/app';
import { useAuthStore } from '../../stores/auth';
import { useI18n } from 'vue-i18n';
import { computed, onMounted, reactive, ref } from 'vue';

export default {
    name: 'SessionsIndex',
    setup() {
        const authStore = useAuthStore();
        const appStore = useAppStore();
        const { t } = useI18n();
        const sessions = ref([]);
        const loading = ref(true);
        const showCreateForm = ref(false);
        const creating = ref(false);
        const createError = ref(null);
        const createForm = reactive({ session_type: '', scheduled_at: '' });

        const isInstructorOrAdmin = computed(() => authStore.isInstructor || authStore.isAdmin);

        function statusBadge(status) {
            const colors = {
                scheduled: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/50 dark:text-yellow-300',
                active: 'bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-300',
                completed: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
                cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/50 dark:text-red-300',
            };
            return colors[status] || 'bg-gray-100 text-gray-800';
        }

        function formatDate(dateStr) {
            if (!dateStr) return '—';
            return new Date(dateStr).toLocaleDateString(undefined, {
                weekday: 'short', year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
            });
        }

        async function fetchSessions() {
            loading.value = true;
            try {
                const resp = await sessionsService.index();
                sessions.value = Array.isArray(resp.data) ? resp.data : [];
            } catch {
                sessions.value = [];
            } finally {
                loading.value = false;
            }
        }

        async function handleCreate() {
            createError.value = null;
            creating.value = true;
            try {
                const resp = await sessionsService.store({
                    session_type: createForm.session_type,
                    scheduled_at: createForm.scheduled_at,
                });
                sessions.value.push(resp.data || {});
                showCreateForm.value = false;
                appStore.addToast({ message: t('sessions.created'), type: 'success' });
            } catch (err) {
                createError.value = err.response?.data?.message || 'Failed to create session';
            } finally {
                creating.value = false;
            }
        }

        onMounted(fetchSessions);

        return { sessions, loading, showCreateForm, createForm, creating, createError, isInstructorOrAdmin, statusBadge, formatDate, handleCreate };
    },
};
</script>
