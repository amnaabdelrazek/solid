<template>
    <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('payments.title') }}</h1>

        <div v-if="loading" class="text-gray-500 dark:text-gray-400">{{ $t('payments.loading') }}</div>

        <div v-else-if="payments.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center text-gray-500 dark:text-gray-400">
            {{ $t('payments.empty') }}
        </div>

        <div v-else class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
            <table class="w-full">
                <thead class="bg-gray-50 dark:bg-gray-700">
                    <tr>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('payments.id') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('payments.amount') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('payments.status') }}</th>
                        <th class="px-6 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">{{ $t('payments.date') }}</th>
                    </tr>
                </thead>
                <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
                    <tr v-for="payment in payments" :key="payment.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
                        <td class="px-6 py-4 text-sm text-gray-900 dark:text-white">#{{ payment.id }}</td>
                        <td class="px-6 py-4 text-sm text-gray-900 dark:text-white">{{ payment.amount || '—' }}</td>
                        <td class="px-6 py-4">
                            <span :class="statusBadge(payment.status)" class="px-2 py-1 rounded-full text-xs font-medium">
                                {{ $t('status.' + payment.status) || payment.status }}
                            </span>
                        </td>
                        <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">{{ formatDate(payment.created_at) }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import { paymentsService } from '../../services/payments';
import { onMounted, ref } from 'vue';

export default {
    name: 'PaymentsHistory',
    setup() {
        const payments = ref([]);
        const loading = ref(true);

        function statusBadge(status) {
            const colors = {
                pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/50 dark:text-yellow-300',
                completed: 'bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-300',
                failed: 'bg-red-100 text-red-800 dark:bg-red-900/50 dark:text-red-300',
                refunded: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
            };
            return colors[status] || 'bg-gray-100 text-gray-800';
        }

        function formatDate(dateStr) {
            if (!dateStr) return '—';
            return new Date(dateStr).toLocaleDateString(undefined, {
                year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit',
            });
        }

        async function fetchPayments() {
            loading.value = true;
            try {
                const resp = await paymentsService.history();
                payments.value = Array.isArray(resp.data) ? resp.data : [];
            } catch {
                payments.value = [];
            } finally {
                loading.value = false;
            }
        }

        onMounted(fetchPayments);

        return { payments, loading, statusBadge, formatDate };
    },
};
</script>
