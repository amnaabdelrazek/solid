<template>
    <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('auth.login_title') }}</h2>
        <form @submit.prevent="handleLogin" class="space-y-4">
            <div class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded" v-if="error">
                {{ error }}
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.mobile') }}</label>
                <input
                    v-model="form.mobile_number"
                    type="text"
                    required
                    placeholder="+201234567890"
                    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.password') }}</label>
                <input
                    v-model="form.password"
                    type="password"
                    required
                    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
            </div>
            <div class="flex items-center justify-between text-sm">
                <router-link to="/forgot-password" class="text-blue-600 hover:text-blue-500">
                    {{ $t('auth.forgot_link') }}
                </router-link>
            </div>
            <button
                type="submit"
                :disabled="loading"
                class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium disabled:opacity-50"
            >
                {{ loading ? $t('auth.logging_in') : $t('auth.login_btn') }}
            </button>
        </form>
        <p class="mt-4 text-center text-sm text-gray-600 dark:text-gray-400">
            {{ $t('auth.no_account') }}
            <router-link to="/register" class="text-blue-600 hover:text-blue-500 font-medium">{{ $t('nav.register') }}</router-link>
        </p>
    </div>
</template>

<script>
import { useAuthStore } from '../../stores/auth';
import { useAppStore } from '../../stores/app';

export default {
    name: 'LoginView',
    data() {
        return {
            form: {
                mobile_number: '',
                password: '',
                device_id: '',
                device_info: {},
            },
            error: null,
            loading: false,
        };
    },
    methods: {
        async handleLogin() {
            this.error = null;
            this.loading = true;
            this.form.device_id = `web-${Date.now()}`;
            this.form.device_info = { platform: 'web', userAgent: navigator.userAgent };

            try {
                await this.authStore.login(this.form);
                this.appStore.addToast({ message: this.$t('auth.login_success'), type: 'success' });
                this.$router.push('/dashboard');
            } catch (err) {
                this.error = err.response?.data?.message || 'Login failed. Please try again.';
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
