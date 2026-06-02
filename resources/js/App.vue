<template>
    <component :is="layout">
        <router-view />
    </component>
</template>

<script>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import AuthLayout from './layouts/Auth.vue';
import DefaultLayout from './layouts/Default.vue';

export default {
    name: 'App',
    components: { AuthLayout, DefaultLayout },
    setup() {
        const route = useRoute();
        const layout = computed(() => route.meta?.layout || DefaultLayout);
        return { layout };
    },
};
</script>
