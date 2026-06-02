<?php

declare(strict_types=1);

namespace Modules\User\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\User\Models\AddictionProfile;

class AddictionProfilePolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:AddictionProfile');
    }

    public function view(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('View:AddictionProfile');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:AddictionProfile');
    }

    public function update(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('Update:AddictionProfile');
    }

    public function delete(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('Delete:AddictionProfile');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:AddictionProfile');
    }

    public function restore(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('Restore:AddictionProfile');
    }

    public function forceDelete(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('ForceDelete:AddictionProfile');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:AddictionProfile');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:AddictionProfile');
    }

    public function replicate(AuthUser $authUser, AddictionProfile $addictionProfile): bool
    {
        return $authUser->can('Replicate:AddictionProfile');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:AddictionProfile');
    }
}
