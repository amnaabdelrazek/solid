<?php

declare(strict_types=1);

namespace Modules\Lookups\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\Lookups\Models\LookupType;

class LookupTypePolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:LookupType');
    }

    public function view(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('View:LookupType');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:LookupType');
    }

    public function update(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('Update:LookupType');
    }

    public function delete(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('Delete:LookupType');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:LookupType');
    }

    public function restore(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('Restore:LookupType');
    }

    public function forceDelete(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('ForceDelete:LookupType');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:LookupType');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:LookupType');
    }

    public function replicate(AuthUser $authUser, LookupType $lookupType): bool
    {
        return $authUser->can('Replicate:LookupType');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:LookupType');
    }
}
