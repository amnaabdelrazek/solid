<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('notifications.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('notifications.loading') }}</div>

        <div v-else-if="notifications.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('notifications.empty') }}
        </div>

        <div v-else class="space-y-3">
            <div
                v-for="notification in notifications"
                :key="notification.id"
                class="bg-white dark:bg-gray-800 rounded-lg shadow p-4 flex items-start gap-4"
            >
                <div class="flex-1">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ notification.title || 'Notification' }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400 mt-1">{{ notification.body || notification.message || '' }}</div>
                    <div class="text-xs text-gray-400 dark:text-gray-500 mt-2">{{ formatDate(notification.created_at) }}</div>
                </div>
                <button v-if="!notification.read_at" @click="handleMarkRead(notification.id)" class="text-xs text-blue-600 hover:text-blue-500 whitespace-nowrap">
                    {{ $t('notifications.mark_read') }}
                </button>
            </div>
        </div>
    </div>
</template>

<script>
import { notificationsService } from '../../services/notifications';
import { useAppStore } from '../../stores/app';
import { useI18n } from 'vue-i18n';
import { onMounted, ref } from 'vue';

export default {
    name: 'NotificationsIndex',
    setup() {
        const appStore = useAppStore();
        const { t } = useI18n();
        const notifications = ref([]);
        const loading = ref(true);

        function formatDate(dateStr) {
            if (!dateStr) return '—';
            return new Date(dateStr).toLocaleDateString(undefined, {
                year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
            });
        }

        async function fetchNotifications() {
            loading.value = true;
            try {
                const resp = await notificationsService.index();
                notifications.value = Array.isArray(resp.data) ? resp.data : [];
            } catch {
                notifications.value = [];
            } finally {
                loading.value = false;
            }
        }

        async function handleMarkRead(id) {
            try {
                await notificationsService.update(id, { read_at: new Date().toISOString() });
                const n = notifications.value.find((x) => x.id === id);
                if (n) n.read_at = new Date().toISOString();
                appStore.addToast({ message: t('notifications.marked_read'), type: 'success' });
            } catch {
                appStore.addToast({ message: 'Failed to mark as read', type: 'error' });
            }
        }

        onMounted(fetchNotifications);

        return { notifications, loading, formatDate, handleMarkRead };
    },
};
</script>
