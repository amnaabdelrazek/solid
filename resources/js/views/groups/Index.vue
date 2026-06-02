<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('groups.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('groups.loading') }}</div>

        <div v-else>
            <div v-if="myGroup" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-6">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">{{ $t('groups.my_group') }}</h2>
                <div class="space-y-2">
                    <div class="text-gray-900 dark:text-white font-medium">{{ myGroup.name }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ myGroup.description || '' }}</div>
                </div>
            </div>

            <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
                <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">{{ $t('groups.all_groups') }}</h2>

                <div v-if="groups.length === 0" class="text-gray-500 dark:text-gray-400">{{ $t('groups.empty') }}</div>

                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    <div v-for="group in groups" :key="group.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
                        <h3 class="font-semibold text-gray-900 dark:text-white">{{ group.name }}</h3>
                        <p class="text-sm text-gray-500 dark:text-gray-400 mt-1">{{ group.description || '' }}</p>
                        <button
                            @click="handleSubscribe(group.id)"
                            :disabled="subscribing"
                            class="mt-3 px-3 py-1.5 bg-blue-600 hover:bg-blue-700 text-white rounded text-sm font-medium disabled:opacity-50"
                        >
                            {{ $t('groups.subscribe') }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { groupsService } from '../../services/groups';
import { useAppStore } from '../../stores/app';
import { useI18n } from 'vue-i18n';
import { onMounted, ref } from 'vue';

export default {
    name: 'GroupsIndex',
    setup() {
        const appStore = useAppStore();
        const { t } = useI18n();
        const groups = ref([]);
        const myGroup = ref(null);
        const loading = ref(true);
        const subscribing = ref(false);

        async function fetchGroups() {
            loading.value = true;
            try {
                const [allResp, myResp] = await Promise.allSettled([
                    groupsService.index(),
                    groupsService.myGroup(),
                ]);
                if (allResp.status === 'fulfilled') {
                    groups.value = Array.isArray(allResp.value.data) ? allResp.value.data : [];
                }
                if (myResp.status === 'fulfilled') {
                    myGroup.value = myResp.value.data || null;
                }
            } catch {
                groups.value = [];
            } finally {
                loading.value = false;
            }
        }

        async function handleSubscribe(groupId) {
            subscribing.value = true;
            try {
                const resp = await groupsService.subscribe({ group_id: groupId });
                myGroup.value = resp.data || resp;
                appStore.addToast({ message: t('groups.subscribed'), type: 'success' });
            } catch (err) {
                appStore.addToast({ message: err.response?.data?.message || 'Failed to subscribe', type: 'error' });
            } finally {
                subscribing.value = false;
            }
        }

        onMounted(fetchGroups);

        return { groups, myGroup, loading, subscribing, handleSubscribe };
    },
};
</script>
