<template>
    <header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-3 flex items-center justify-between">
        <div class="flex items-center gap-4">
            <button
                @click="$emit('toggle-sidebar')"
                class="text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white"
            >
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
                </svg>
            </button>
            <router-link to="/dashboard" class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ $t('app.name') }}
            </router-link>
        </div>
        <div class="flex items-center gap-4">
            <button
                @click="toggleLanguage"
                class="text-sm px-2 py-1 rounded border border-gray-300 dark:border-gray-600 text-gray-600 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                :title="$t('nav.language')"
            >
                {{ currentLocale === 'ar' ? 'EN' : 'AR' }}
            </button>

            <router-link
                to="/notifications"
                class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-white"
            >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
            </router-link>
            <div class="relative" v-click-outside="() => dropdownOpen = false">
                <button
                    @click="dropdownOpen = !dropdownOpen"
                    class="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white"
                >
                    <span>{{ authStore.userName || 'User' }}</span>
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </button>
                <div
                    v-if="dropdownOpen"
                    class="absolute end-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700 py-1 z-50"
                >
                    <router-link
                        to="/profile"
                        class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
                        @click="dropdownOpen = false"
                    >
                        {{ $t('nav.profile') }}
                    </router-link>
                    <hr class="border-gray-200 dark:border-gray-700" />
                    <button
                        @click="handleLogout"
                        class="block w-full text-left px-4 py-2 text-sm text-red-600 hover:bg-gray-100 dark:hover:bg-gray-700"
                    >
                        {{ $t('nav.logout') }}
                    </button>
                </div>
            </div>
        </div>
    </header>
</template>

<script>
import { useAuthStore } from '../stores/auth';
import { useRouter } from 'vue-router';
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { setLocale } from '../i18n';

export default {
    name: 'Navbar',
    emits: ['toggle-sidebar'],
    data() {
        return { dropdownOpen: false };
    },
    setup() {
        const authStore = useAuthStore();
        const router = useRouter();
        const { locale } = useI18n();
        const currentLocale = computed(() => locale.value);

        function toggleLanguage() {
            const newLocale = locale.value === 'ar' ? 'en' : 'ar';
            setLocale(newLocale);
        }

        return { authStore, router, currentLocale, toggleLanguage };
    },
    methods: {
        async handleLogout() {
            this.dropdownOpen = false;
            await this.authStore.logout();
            this.router.push('/login');
        },
    },
    directives: {
        clickOutside: {
            mounted(el, binding) {
                el.__clickOutside = (event) => {
                    if (!el.contains(event.target)) {
                        binding.value();
                    }
                };
                document.addEventListener('click', el.__clickOutside);
            },
            unmounted(el) {
                document.removeEventListener('click', el.__clickOutside);
            },
        },
    },
};
</script>
