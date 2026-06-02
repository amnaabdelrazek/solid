<template>
    <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('auth.register_title') }}</h2>
        <form @submit.prevent="handleRegister" class="space-y-4">
            <div class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded" v-if="error">
                {{ error }}
            </div>
            <div v-if="fieldErrors" class="text-sm text-red-600 bg-red-50 dark:bg-red-900/50 dark:text-red-300 p-3 rounded">
                <p v-for="(msgs, field) in fieldErrors" :key="field">{{ field }}: {{ msgs.join(', ') }}</p>
            </div>

            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.display_name') }}</label>
                <input v-model="form.display_name" type="text" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.mobile') }}</label>
                <input v-model="form.mobile_number" type="text" required placeholder="+201234567890" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.password') }}</label>
                <input v-model="form.password" type="password" required minlength="8" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.preferred_language') }}</label>
                <select v-model="form.preferred_language" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500">
                    <option value="ar">العربية</option>
                    <option value="en">English</option>
                </select>
            </div>

            <hr class="border-gray-200 dark:border-gray-700" />
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">{{ $t('auth.addiction_profile') }}</h3>

            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.addiction_duration') }}</label>
                <select v-model="form.addiction_duration_id" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500">
                    <option value="" disabled>{{ $t('auth.select_duration') }}</option>
                    <option v-for="d in durations" :key="d.id" :value="d.id">{{  d.label }}</option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.education_level') }}</label>
                <select v-model="form.education_level_id" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500">
                    <option value="" disabled>{{ $t('auth.select_education') }}</option>
                    <option v-for="e in educationLevels" :key="e.id" :value="e.id">{{ e.label  }}</option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.had_prior_treatment') }}</label>
                <select v-model="form.had_prior_treatment" required class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500">
                    <option value="" disabled>{{ $t('auth.select') }}</option>
                    <option :value="true">{{ $t('auth.yes') }}</option>
                    <option :value="false">{{ $t('auth.no') }}</option>
                </select>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.substances') }}</label>
                <div class="space-y-1 max-h-32 overflow-y-auto border border-gray-200 dark:border-gray-600 rounded-lg p-2">
                    <label v-for="s in substances" :key="s.id" class="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300">
                        <input type="checkbox" :value="s.id" v-model="form.substance_ids" class="rounded border-gray-300 dark:border-gray-600" />
                        {{ s.label }}
                    </label>
                </div>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.treatment_types') }}</label>
                <div class="space-y-1 max-h-32 overflow-y-auto border border-gray-200 dark:border-gray-600 rounded-lg p-2">
                    <label v-for="t in treatmentTypes" :key="t.id" class="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300">
                        <input type="checkbox" :value="t.id" v-model="form.treatment_type_ids" class="rounded border-gray-300 dark:border-gray-600" />
                        {{ t.label}}
                    </label>
                </div>
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.days_clean') }}</label>
                <input v-model="form.days_clean" type="number" min="0" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500" />
            </div>
            <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">{{ $t('auth.addiction_reason') }}</label>
                <textarea v-model="form.addiction_reason" rows="2" maxlength="1000" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500"></textarea>
            </div>

            <button type="submit" :disabled="loading" class="w-full py-2 px-4 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium disabled:opacity-50">
                {{ loading ? $t('auth.registering') : $t('auth.register_btn') }}
            </button>
        </form>
        <p class="mt-4 text-center text-sm text-gray-600 dark:text-gray-400">
            {{ $t('auth.has_account') }}
            <router-link to="/login" class="text-blue-600 hover:text-blue-500 font-medium">{{ $t('nav.login') }}</router-link>
        </p>
    </div>
</template>

<script>
import { useAuthStore } from '../../stores/auth';
import { useAppStore } from '../../stores/app';
import { lookupsService } from '../../services/lookups';
import { useRouter } from 'vue-router';
import { onMounted, reactive, ref } from 'vue';

export default {
    name: 'RegisterView',
    setup() {
        const authStore = useAuthStore();
        const appStore = useAppStore();
        const router = useRouter();

        const form = reactive({
            display_name: '',
            mobile_number: '',
            password: '',
            preferred_language: localStorage.getItem('locale') || 'ar',
            addiction_duration_id: '',
            education_level_id: '',
            had_prior_treatment: '',
            substance_ids: [],
            treatment_type_ids: [],
            addiction_reason: '',
            days_clean: '',
        });

        const durations = ref([]);
        const educationLevels = ref([]);
        const substances = ref([]);
        const treatmentTypes = ref([]);
        const error = ref(null);
        const fieldErrors = ref(null);
        const loading = ref(false);

        onMounted(async () => {
            try {
                const [dResp, eResp, sResp, tResp] = await Promise.allSettled([
                    lookupsService.byType('addiction_duration'),
                    lookupsService.byType('education_level'),
                    lookupsService.substances(),
                    lookupsService.byType('treatment_type'),
                ]);
                console.log(dResp);
                if (dResp.status === 'fulfilled') durations.value = dResp.value.data.values || [];
                if (eResp.status === 'fulfilled') educationLevels.value = eResp.value.data.values || [];
                if (sResp.status === 'fulfilled') substances.value = sResp.value.data.categories || [];
                if (tResp.status === 'fulfilled') treatmentTypes.value = tResp.value.data.values || [];
            } catch {}
        });

        async function handleRegister() {
            error.value = null;
            fieldErrors.value = null;
            loading.value = true;

            const payload = {
                display_name: form.display_name,
                mobile_number: form.mobile_number,
                password: form.password,
                preferred_language: form.preferred_language,
                addiction_duration_id: Number(form.addiction_duration_id),
                education_level_id: Number(form.education_level_id),
                had_prior_treatment: Boolean(form.had_prior_treatment),
                substance_ids: form.substance_ids.map(Number),
                treatment_type_ids: form.treatment_type_ids.map(Number),
            };
            if (form.addiction_reason) payload.addiction_reason = form.addiction_reason;
            if (form.days_clean !== '' && form.days_clean !== null) payload.days_clean = Number(form.days_clean);

            try {
                await authStore.register(payload);
                appStore.addToast({ message: 'Registration successful! Please verify your OTP.', type: 'success' });
                router.push('/verify');
            } catch (err) {
                if (err.response?.status === 422 && err.response?.data?.errors) {
                    fieldErrors.value = err.response.data.errors;
                } else {
                    error.value = err.response?.data?.message || 'Registration failed. Please try again.';
                }
            } finally {
                loading.value = false;
            }
        }

        return { form, durations, educationLevels, substances, treatmentTypes, error, fieldErrors, loading, handleRegister };
    },
};
</script>
