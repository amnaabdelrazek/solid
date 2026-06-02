<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('settings.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('settings.loading') }}</div>

        <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
            <table class="w-full">
                <thead class="bg-gray-50 dark:bg-gray-700">
                    <tr>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('settings.key') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('settings.value') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('settings.type') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('settings.actions') }}</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
                    <tr v-for="(setting, key) in settings" :key="key" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
                        <td class="px-6 py-4 text-sm font-medium text-gray-900 dark:text-white">{{ key }}</td>
                        <td class="px-6 py-4">
                            <input
                                v-if="editingKey === key"
                                v-model="editValue"
                                type="text"
                                class="w-full px-2 py-1 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                            />
                            <span v-else class="text-sm text-gray-500 dark:text-gray-400">{{ formatValue(setting.value) }}</span>
                        </td>
                        <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">{{ setting.type }}</td>
                        <td class="px-6 py-4">
                            <div v-if="editingKey === key" class="flex gap-2">
                                <button @click="handleSave(key)" :disabled="saving" class="text-sm text-green-600 hover:text-green-500 font-medium disabled:opacity-50">
                                    {{ $t('settings.save') }}
                                </button>
                                <button @click="editingKey = null" class="text-sm text-gray-600 hover:text-gray-500">{{ $t('settings.cancel') }}</button>
                            </div>
                            <button v-else @click="startEdit(key, setting.value)" class="text-sm text-blue-600 hover:text-blue-500 font-medium">
                                {{ $t('settings.edit') }}
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import { settingsService } from '../../services/settings';
import { useAppStore } from '../../stores/app';
import { onMounted, ref } from 'vue';

export default {
    name: 'SettingsIndex',
    setup() {
        const appStore = useAppStore();
        const settings = ref({});
        const loading = ref(true);
        const editingKey = ref(null);
        const editValue = ref('');
        const saving = ref(false);

        function formatValue(val) {
            if (val === null || val === undefined) return '—';
            if (typeof val === 'object') return JSON.stringify(val);
            return String(val);
        }

        async function fetchSettings() {
            loading.value = true;
            try {
                const resp = await settingsService.index();
                settings.value = resp.data?.settings || resp.settings || {};
            } catch {
                settings.value = {};
            } finally {
                loading.value = false;
            }
        }

        function startEdit(key, value) {
            editingKey.value = key;
            editValue.value = typeof value === 'object' ? JSON.stringify(value) : String(value ?? '');
        }

        async function handleSave(key) {
            saving.value = true;
            try {
                const resp = await settingsService.update(key, editValue.value);
                const existing = settings.value[key];
                existing.value = editValue.value;
                editingKey.value = null;
                appStore.addToast({ message: resp.data?.message || 'Setting updated', type: 'success' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to update', type: 'error' });
            } finally {
                saving.value = false;
            }
        }

        onMounted(fetchSettings);

        return { settings, loading, editingKey, editValue, saving, formatValue, startEdit, handleSave };
    },
};
</script>
