<?php

namespace Modules\User\Models;

use Database\Factories\UserFactory;
use Filament\Models\Contracts\FilamentUser;
use Filament\Models\Contracts\HasName;
use Filament\Panel;
use Illuminate\Database\Eloquent\Attributes\Fillable;
use Illuminate\Database\Eloquent\Attributes\Hidden;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Relations\BelongsToMany;
use Illuminate\Database\Eloquent\Relations\HasMany;
use Illuminate\Database\Eloquent\Relations\HasOne;
use Illuminate\Database\Eloquent\SoftDeletes;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Laravel\Sanctum\HasApiTokens;
use Modules\Auth\Enums\UserRole;
use Modules\Auth\Models\DeviceSession;
use Modules\Groups\Models\Group;
use Modules\Payments\Models\Payment;
use Modules\Payments\Models\PaymentMethod;
use Modules\Sessions\Models\SessionAttendance;
use Spatie\MediaLibrary\HasMedia;
use Spatie\MediaLibrary\InteractsWithMedia;
use Spatie\Permission\Traits\HasRoles;

#[Fillable(['display_name',
    'mobile_number',
    'username',
    'password',
    'role',
    'preferred_language',
    'fcm_token',
    'active_device_id',
    'is_active',
    'bio',
    'avatar_url', ])]
#[Hidden(['password', 'remember_token'])]
class User extends Authenticatable implements FilamentUser, HasMedia, HasName
{
    /** @use HasFactory<UserFactory> */
    use HasApiTokens, HasFactory, HasRoles, InteractsWithMedia, Notifiable, SoftDeletes;

    protected static function newFactory()
    {
        return UserFactory::new();
    }

    protected $fillable = [
        'display_name',
        'mobile_number',
        'username',
        'password',
        'role',
        'preferred_language',
        'fcm_token',
        'active_device_id',
        'is_active',
        'bio',
        'avatar_url',
        'experience',
        'quote',
    ];

    /**
     * Get the attributes that should be cast.
     *
     * @return array<string, string>
     */
    protected function casts(): array
    {
        return [
            'email_verified_at' => 'datetime',
            'password' => 'hashed',
            'role' => UserRole::class,
            'is_active' => 'boolean',
            'experience' => 'array',
        ];
    }

    public function addictionProfile(): HasOne
    {
        return $this->hasOne(AddictionProfile::class);
    }

    public function substances(): BelongsToMany
    {
        return $this->belongsToMany(Substance::class, 'user_substances');
    }

    public function groups(): BelongsToMany
    {
        return $this->belongsToMany(Group::class, 'group_members')
            ->withPivot(['joined_at', 'is_active'])
            ->wherePivot('is_active', 1);
    }

    public function payments(): HasMany
    {
        return $this->hasMany(Payment::class);
    }

    public function paymentMethods(): HasMany
    {
        return $this->hasMany(PaymentMethod::class);
    }

    public function deviceSessions(): HasMany
    {
        return $this->hasMany(DeviceSession::class);
    }

    public function sessionAttendances(): HasMany
    {
        return $this->hasMany(SessionAttendance::class);
    }

    public function isInstructor(): bool
    {
        return $this->role === UserRole::Instructor;
    }

    public function isAdmin(): bool
    {
        return $this->role === UserRole::Admin;
    }

    public function canAccessPanel(Panel $panel): bool
    {
        return $this->isAdmin() || $this->hasRole(config('filament-shield.super_admin.name'));
    }

    public function getFilamentName(): string
    {
        return $this->display_name ?? $this->username ?? '';
    }
}
