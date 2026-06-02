<?php

declare(strict_types=1);

namespace Modules\Lookups\Policies;

use Illuminate\Auth\Access\HandlesAuthorization;
use Illuminate\Foundation\Auth\User as AuthUser;
use Modules\Lookups\Models\LookupValue;

class LookupValuePolicy
{
    use HandlesAuthorization;

    public function viewAny(AuthUser $authUser): bool
    {
        return $authUser->can('ViewAny:LookupValue');
    }

    public function view(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('View:LookupValue');
    }

    public function create(AuthUser $authUser): bool
    {
        return $authUser->can('Create:LookupValue');
    }

    public function update(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('Update:LookupValue');
    }

    public function delete(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('Delete:LookupValue');
    }

    public function deleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('DeleteAny:LookupValue');
    }

    public function restore(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('Restore:LookupValue');
    }

    public function forceDelete(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('ForceDelete:LookupValue');
    }

    public function forceDeleteAny(AuthUser $authUser): bool
    {
        return $authUser->can('ForceDeleteAny:LookupValue');
    }

    public function restoreAny(AuthUser $authUser): bool
    {
        return $authUser->can('RestoreAny:LookupValue');
    }

    public function replicate(AuthUser $authUser, LookupValue $lookupValue): bool
    {
        return $authUser->can('Replicate:LookupValue');
    }

    public function reorder(AuthUser $authUser): bool
    {
        return $authUser->can('Reorder:LookupValue');
    }
}
