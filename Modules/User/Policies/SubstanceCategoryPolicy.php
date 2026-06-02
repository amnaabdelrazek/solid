<?php

declare(strict_types=1);

namespace Modules\User\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\User\Models\SubstanceCategory;

class SubstanceCategoryPolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:SubstanceCategory');
    }

    public function view(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('View:SubstanceCategory');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:SubstanceCategory');
    }

    public function update(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('Update:SubstanceCategory');
    }

    public function delete(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('Delete:SubstanceCategory');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:SubstanceCategory');
    }

    public function restore(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('Restore:SubstanceCategory');
    }

    public function forceDelete(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('ForceDelete:SubstanceCategory');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:SubstanceCategory');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:SubstanceCategory');
    }

    public function replicate(AuthUser $authUser, SubstanceCategory $substanceCategory): bool
    {
        return $authUser->can('Replicate:SubstanceCategory');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:SubstanceCategory');
    }
}
