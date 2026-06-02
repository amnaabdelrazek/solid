<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('admin.users_title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('admin.users_loading') }}</div>

        <div v-else-if="users.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('admin.users_empty') }}
        </div>

        <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
            <table class="w-full">
                <thead class="bg-gray-50 dark:bg-gray-700">
                    <tr>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('payments.id') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('admin.name') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('admin.mobile') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('admin.role') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('admin.status') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('admin.actions') }}</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
                    <tr v-for="user in users" :key="user.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
                        <td class="px-6 py-4 text-sm text-gray-900 dark:text-white">#{{ user.id }}</td>
                        <td class="px-6 py-4 text-sm text-gray-900 dark:text-white font-medium">{{ user.display_name }}</td>
                        <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">{{ user.mobile_number }}</td>
                        <td class="px-6 py-4 text-sm capitalize">{{ user.role }}</td>
                        <td class="px-6 py-4">
                            <span :class="user.is_active ? 'bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-300' : 'bg-red-100 text-red-800 dark:bg-red-900/50 dark:text-red-300'" class="px-2 py-1 rounded-full text-xs font-medium">
                                {{ user.is_active ? $t('admin.active') : $t('admin.inactive') }}
                            </span>
                        </td>
                        <td class="px-6 py-4">
                            <button @click="handleToggleActive(user)" class="text-sm text-blue-600 hover:text-blue-500">
                                {{ user.is_active ? $t('admin.deactivate') : $t('admin.activate') }}
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import { usersService } from '../../services/users';
import { useAppStore } from '../../stores/app';
import { useI18n } from 'vue-i18n';
import { onMounted, ref } from 'vue';

export default {
    name: 'AdminUsersIndex',
    setup() {
        const appStore = useAppStore();
        const { t } = useI18n();
        const users = ref([]);
        const loading = ref(true);

        async function fetchUsers() {
            loading.value = true;
            try {
                const resp = await usersService.index();
                users.value = Array.isArray(resp.data) ? resp.data : [];
            } catch {
                users.value = [];
            } finally {
                loading.value = false;
            }
        }

        async function handleToggleActive(user) {
            try {
                await usersService.update(user.id, { is_active: !user.is_active });
                user.is_active = !user.is_active;
                appStore.addToast({
                    message: user.is_active ? t('admin.activated') : t('admin.deactivated'),
                    type: 'success',
                });
            } catch (err) {
                appStore.addToast({
                    message: err.response?.data?.message || 'Failed to update user',
                    type: 'error',
                });
            }
        }

        onMounted(fetchUsers);

        return { users, loading, handleToggleActive };
    },
};
</script>
