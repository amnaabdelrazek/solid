<template>
    <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('auth.forgot_title') }}</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">{{ $t('auth.forgot_instruction') }}</p>
        <form @submit.prevent="handleForgotPassword" class="space-y-4">
            <div class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded" v-if="error">
                {{ error }}
            </div>
            <div class="text-sm text-green-600 bg-green-50 dark:bg-green-900/50 dark:text-green-300 p-3 rounded" v-if="success">
                {{ success }}
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.mobile') }}</label>
                <input v-model="form.mobile_number" type="text" required placeholder="+201234567890" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500" />
            </div>
            <button type="submit" :disabled="loading" class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium disabled:opacity-50">
                {{ loading ? $t('auth.sending') : $t('auth.send_otp_btn') }}
            </button>
        </form>
        <p class="mt-4 text-center text-sm text-gray-600 dark:text-gray-400">
            <router-link to="/login" class="text-blue-600 hover:text-blue-500 font-medium">{{ $t('auth.back_login') }}</router-link>
        </p>
    </div>
</template>

<script>
import { authService } from '../../services/auth';

export default {
    name: 'ForgotPasswordView',
    data() {
        return {
            form: { mobile_number: '' },
            error: null,
            success: null,
            loading: false,
        };
    },
    methods: {
        async handleForgotPassword() {
            this.error = null;
            this.success = null;
            this.loading = true;

            try {
                await authService.forgotPassword(this.form.mobile_number);
                this.success = 'OTP sent to your mobile number.';
                this.$router.push('/reset-password');
            } catch (err) {
                this.error = err.response?.data?.message || 'Failed to send OTP.';
            } finally {
                this.loading = false;
            }
        },
    },
};
</script>
