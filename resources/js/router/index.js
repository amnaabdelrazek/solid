import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import AuthLayout from '../layouts/Auth.vue';
import DefaultLayout from '../layouts/Default.vue';
import FullscreenLayout from '../layouts/Fullscreen.vue';

import Login from '../views/auth/Login.vue';
import Register from '../views/auth/Register.vue';
import VerifyOtp from '../views/auth/VerifyOtp.vue';
import ForgotPassword from '../views/auth/ForgotPassword.vue';
import ResetPassword from '../views/auth/ResetPassword.vue';
import Dashboard from '../views/Dashboard.vue';
import Profile from '../views/Profile.vue';
import SessionsIndex from '../views/sessions/Index.vue';
import SessionsShow from '../views/sessions/Show.vue';
import JitsiTest from '../views/sessions/JitsiTest.vue';
import GroupsIndex from '../views/groups/Index.vue';
import PaymentsHistory from '../views/payments/History.vue';
import NotificationsIndex from '../views/notifications/Index.vue';
import RecommendationsIndex from '../views/recommendations/Index.vue';
import AdminUsersIndex from '../views/admin/UsersIndex.vue';
import SettingsIndex from '../views/settings/Index.vue';

const routes = [
    {
        path: '/login',
        component: Login,
        meta: { layout: AuthLayout, guest: true },
    },
    {
        path: '/register',
        component: Register,
        meta: { layout: AuthLayout, guest: true },
    },
    {
        path: '/verify',
        component: VerifyOtp,
        meta: { layout: AuthLayout, guest: true },
    },
    {
        path: '/forgot-password',
        component: ForgotPassword,
        meta: { layout: AuthLayout, guest: true },
    },
    {
        path: '/reset-password',
        component: ResetPassword,
        meta: { layout: AuthLayout, guest: true },
    },
    {
        path: '/',
        redirect: '/dashboard',
    },
    {
        path: '/dashboard',
        component: Dashboard,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/profile',
        component: Profile,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/sessions',
        component: SessionsIndex,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/sessions/:id',
        component: SessionsShow,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/sessions/:id/jitsi-test',
        component: JitsiTest,
        meta: { layout: FullscreenLayout, auth: true },
    },
    {
        path: '/groups',
        component: GroupsIndex,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/payments',
        component: PaymentsHistory,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/notifications',
        component: NotificationsIndex,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/recommendations',
        component: RecommendationsIndex,
        meta: { layout: DefaultLayout, auth: true },
    },
    {
        path: '/admin/users',
        component: AdminUsersIndex,
        meta: { layout: DefaultLayout, auth: true, role: 'admin' },
    },
    {
        path: '/settings',
        component: SettingsIndex,
        meta: { layout: DefaultLayout, auth: true, role: 'admin' },
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

router.beforeEach((to, from, next) => {
    const authStore = useAuthStore();

    if (to.meta.auth && !authStore.isAuthenticated) {
        return next('/login');
    }

    if (to.meta.guest && authStore.isAuthenticated) {
        return next('/dashboard');
    }

    if (to.meta.role && authStore.role !== to.meta.role) {
        return next('/dashboard');
    }

    next();
});

export default router;
