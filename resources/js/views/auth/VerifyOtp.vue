<template>
    <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('auth.verify_title') }}</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">{{ $t('auth.otp_sent_to') }}</p>
        <form @submit.prevent="handleVerify" class="space-y-4">
            <div class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded" v-if="error">
                {{ error }}
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.otp_code') }}</label>
                <input
                    v-model="form.otp"
                    type="text"
                    required
                    maxlength="6"
                    placeholder="000000"
                    class="w-full px-3 py-2 text-center text-2xl tracking-widest border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500"
                />
            </div>
            <button type="submit" :disabled="loading" class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium disabled:opacity-50">
                {{ loading ? $t('auth.verifying') : $t('auth.verify_btn') }}
            </button>
        </form>
    </div>
</template>

<script>
import { useAuthStore } from '../../stores/auth';
import { useAppStore } from '../../stores/app';

export default {
    name: 'VerifyOtpView',
    data() {
        return {
            form: { otp: '' },
            error: null,
            loading: false,
        };
    },
    methods: {
        async handleVerify() {
            this.error = null;
            this.loading = true;

            try {
                await this.authStore.verifyOtp(this.form);
                this.appStore.addToast({ message: this.$t('auth.verify_success'), type: 'success' });
                this.$router.push('/dashboard');
            } catch (err) {
                this.error = err.response?.data?.message || 'Verification failed. Try again.';
            } finally {
                this.loading = false;
            }
        },
    },
    setup() {
        const authStore = useAuthStore();
        const appStore = useAppStore();
        return { authStore, appStore };
    },
};
</script>
