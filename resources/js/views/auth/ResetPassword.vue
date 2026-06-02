<template>
    <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('auth.reset_title') }}</h2>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-6">{{ $t('auth.reset_instruction') }}</p>
        <form @submit.prevent="handleReset" class="space-y-4">
            <div class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded" v-if="error">
                {{ error }}
            </div>
            <div class="text-sm text-green-600 bg-green-50 dark:bg-green-900/50 dark:text-green-300 p-3 rounded" v-if="success">
                {{ success }}
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.mobile') }}</label>
                <input v-model="form.mobile_number" type="text" required placeholder="+201234567890" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.otp_code') }}</label>
                <input v-model="form.otp" type="text" required maxlength="6" class="w-full px-3 py-2 text-center text-xl tracking-widest border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.new_password') }}</label>
                <input v-model="form.password" type="password" required minlength="8" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white" />
            </div>
            <button type="submit" :disabled="loading" class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium disabled:opacity-50">
                {{ loading ? $t('auth.resetting') : $t('auth.reset_btn') }}
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
    name: 'ResetPasswordView',
    data() {
        return {
            form: { mobile_number: '', otp: '', password: '' },
            error: null,
            success: null,
            loading: false,
        };
    },
    methods: {
        async handleReset() {
            this.error = null;
            this.success = null;
            this.loading = true;

            try {
                const verifyResp = await authService.verifyForgotOtp({
                    mobile_number: this.form.mobile_number,
                    otp: this.form.otp,
                });
                const resetToken = verifyResp.data?.reset_token || verifyResp.reset_token;

                await authService.resetPassword({
                    reset_token: resetToken,
                    password: this.form.password,
                });

                this.success = this.$t('auth.reset_success');
                setTimeout(() => this.$router.push('/login'), 1500);
            } catch (err) {
                this.error = err.response?.data?.message || 'Failed to reset password.';
            } finally {
                this.loading = false;
            }
        },
    },
};
</script>
