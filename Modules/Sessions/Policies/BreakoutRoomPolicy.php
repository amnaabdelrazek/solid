<?php

declare(strict_types=1);

namespace Modules\Sessions\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\Sessions\Models\BreakoutRoom;

class BreakoutRoomPolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:BreakoutRoom');
    }

    public function view(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('View:BreakoutRoom');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:BreakoutRoom');
    }

    public function update(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('Update:BreakoutRoom');
    }

    public function delete(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('Delete:BreakoutRoom');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:BreakoutRoom');
    }

    public function restore(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('Restore:BreakoutRoom');
    }

    public function forceDelete(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('ForceDelete:BreakoutRoom');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:BreakoutRoom');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:BreakoutRoom');
    }

    public function replicate(AuthUser $authUser, BreakoutRoom $breakoutRoom): bool
    {
        return $authUser->can('Replicate:BreakoutRoom');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:BreakoutRoom');
    }
}
