<template>
    <div class="h-screen w-screen flex flex-col bg-gray-900">
        <div class="flex items-center justify-between px-4 py-2 bg-gray-800 text-white shrink-0">
            <div class="flex items-center gap-3">
                <button @click="goBack" class="text-gray-300 hover:text-white text-sm">&larr; Exit</button>
                <h1 class="text-sm font-medium">{{ roomName || 'Loading...' }}</h1>
            </div>
            <div class="flex items-center gap-3 text-xs text-gray-400">
                <span>{{ sessionLabel }}</span>
                <span>{{ userLabel }}</span>
                <span>{{ serverUrl }}</span>
            </div>
        </div>
        <div v-if="loading" class="flex-1 flex items-center justify-center text-gray-400 text-sm">Loading session...</div>
        <div v-else ref="container" class="flex-1"></div>
    </div>
</template>

<script>
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { sessionsService } from '../../services/sessions';
import { useAuthStore } from '../../stores/auth';
import { useAppStore } from '../../stores/app';
import { useI18n } from 'vue-i18n';

export default {
    name: 'JitsiTest',
    setup() {
        const route = useRoute();
        const router = useRouter();
        const authStore = useAuthStore();
        const appStore = useAppStore();
        const { t } = useI18n();
        const container = ref(null);
        const roomName = ref('');
        const loading = ref(true);
        let api = null;

        const serverUrl = import.meta.env.VITE_JITSI_SERVER_URL || 'https://meet.jit.si';
        const sessionLabel = `Session #${route.params.id}`;
        const userLabel = computed(() => authStore.user?.display_name || authStore.user?.name || '');

        function goBack() {
            router.push(`/sessions/${route.params.id}`);
        }

        function initJitsi() {
            if (!container.value) return;

            const domain = new URL(serverUrl).hostname;
            const options = {
                roomName,
                width: '100%',
                height: '100%',
                parentNode: container.value,
                userInfo: {
                    displayName: authStore.user?.display_name || authStore.user?.name || 'Anonymous',
                    email: authStore.user?.email || '',
                },
                configOverrides: {
                    startWithAudioMuted: true,
                    startWithVideoMuted: true,
                    disableDeepLinking: true,
                    prejoinPageEnabled: false,
                    enableClosePage: false,
                    disableInviteFunctions: true,
                    requireDisplayName: false,
                },
                interfaceConfigOverrides: {
                    TOOLBAR_ALWAYS_VISIBLE: true,
                    SHOW_JITSI_WATERMARK: false,
                    SHOW_WATERMARK_FOR_GUESTS: false,
                    DISABLE_JOIN_LEAVE_NOTIFICATIONS: true,
                    FILM_STRIP_MAX_HEIGHT: 120,
                },
            };

            const script = document.createElement('script');
            script.src = `${serverUrl}/external_api.js`;
            script.async = true;
            script.onload = () => {
                try {
                    api = new JitsiMeetExternalAPI(domain, options);
                    api.addListener('videoConferenceJoined', () => {
                        console.log('Joined Jitsi room');
                    });
                    api.addListener('readyToClose', () => {
                        goBack();
                    });
                } catch (e) {
                    console.error('Jitsi init error:', e);
                    appStore.addToast({ message: 'Failed to load Jitsi: ' + e.message, type: 'error' });
                }
            };
            script.onerror = () => {
                appStore.addToast({ message: `Failed to load Jitsi script from ${serverUrl}`, type: 'error' });
            };
            document.head.appendChild(script);
        }

        onMounted(async () => {
            try {
                const resp = await sessionsService.show(route.params.id);
                roomName.value = resp.data?.data?.session?.jitsi_room_name || `session-${route.params.id}`;
            } catch {
                roomName.value = `session-${route.params.id}`;
            } finally {
                loading.value = false;
            }
            await nextTick();
            if (roomName.value) initJitsi();
        });

        onUnmounted(() => {
            if (api) {
                try { api.dispose(); } catch {}
            }
        });

        return { container, roomName: roomName, serverUrl, sessionLabel, userLabel, goBack, loading };
    },
};
</script>
