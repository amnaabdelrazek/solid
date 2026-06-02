<?php

declare(strict_types=1);

namespace Modules\User\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\User\Models\Substance;

class SubstancePolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:Substance');
    }

    public function view(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('View:Substance');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:Substance');
    }

    public function update(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('Update:Substance');
    }

    public function delete(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('Delete:Substance');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:Substance');
    }

    public function restore(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('Restore:Substance');
    }

    public function forceDelete(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('ForceDelete:Substance');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:Substance');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:Substance');
    }

    public function replicate(AuthUser $authUser, Substance $substance): bool
    {
        return $authUser->can('Replicate:Substance');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:Substance');
    }
}
