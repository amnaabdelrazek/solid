<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('recommendations.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('recommendations.loading') }}</div>

        <div v-else-if="recommendations.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('recommendations.empty') }}
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="item in recommendations" :key="item.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">{{ item.title }}</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ item.description }}</p>
                <div v-if="item.type" class="mt-3">
                    <span class="text-xs px-2 py-1 bg-blue-100 text-blue-800 dark:bg-blue-900/50 dark:text-blue-300 rounded-full">{{ item.type }}</span>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { recommendationsService } from '../../services/recommendations';
import { onMounted, ref } from 'vue';

export default {
    name: 'RecommendationsIndex',
    setup() {
        const recommendations = ref([]);
        const loading = ref(true);

        async function fetchRecommendations() {
            loading.value = true;
            try {
                const resp = await recommendationsService.index();
                recommendations.value = Array.isArray(resp.data) ? resp.data : [];
            } catch {
                recommendations.value = [];
            } finally {
                loading.value = false;
            }
        }

        onMounted(fetchRecommendations);

        return { recommendations, loading };
    },
};
</script>
