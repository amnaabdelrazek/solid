<template>
    <div class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 px-4">
        <div class="w-full max-w-md">
            <div class="text-center mb-8">
                <h1 class="text-3xl font-bold text-gray-900 dark:text-white">
                    {{ $t('app.name') }}
                </h1>
            </div>
            <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
                <slot />
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'AuthLayout',
};
</script>
