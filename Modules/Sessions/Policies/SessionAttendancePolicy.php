<?php

declare(strict_types=1);

namespace Modules\Sessions\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\Sessions\Models\SessionAttendance;

class SessionAttendancePolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:SessionAttendance');
    }

    public function view(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('View:SessionAttendance');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:SessionAttendance');
    }

    public function update(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('Update:SessionAttendance');
    }

    public function delete(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('Delete:SessionAttendance');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:SessionAttendance');
    }

    public function restore(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('Restore:SessionAttendance');
    }

    public function forceDelete(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('ForceDelete:SessionAttendance');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:SessionAttendance');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:SessionAttendance');
    }

    public function replicate(AuthUser $authUser, SessionAttendance $sessionAttendance): bool
    {
        return $authUser->can('Replicate:SessionAttendance');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:SessionAttendance');
    }
}
