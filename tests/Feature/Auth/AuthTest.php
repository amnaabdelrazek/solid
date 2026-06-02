<?php

namespace Tests\Feature\Auth;

use Illuminate\Foundation\Testing\RefreshDatabase;
use Modules\Auth\Enums\UserRole;
use Modules\User\Models\User;
use Tests\TestCase;

class AuthTest extends TestCase
{
    use RefreshDatabase;

    public function test_user_can_register(): void
    {
        $response = $this->postJson('/api/v1/auth/register', [
            'display_name' => 'Ø£Ø¨Ùˆ Ù…Ø­Ù…Ø¯',
            'mobile_number' => '+201001234567',
            'password' => 'SecurePass123!',
        ]);

        $response->assertStatus(201)
            ->assertJsonPath('data.role', UserRole::Addict->value);

        $this->assertDatabaseHas('users', ['mobile_number' => '+201001234567']);
    }

    public function test_user_can_login(): void
    {
        User::factory()->create([
            'mobile_number' => '+201001234567',
            'password' => bcrypt('SecurePass123!'),
            'role' => UserRole::Addict,
            'is_active' => true,
        ]);

        $response = $this->postJson('/api/v1/auth/login', [
            'mobile_number' => '+201001234567',
            'password' => 'SecurePass123!',
            'device_id' => 'device-abc-123',
        ]);

        $response->assertOk()
            ->assertJsonStructure(['data' => ['token', 'token_type', 'user']]);
    }

    public function test_login_fails_with_wrong_password(): void
    {
        User::factory()->create([
            'mobile_number' => '+201001234567',
            'password' => bcrypt('correct-password'),
            'is_active' => true,
        ]);

        $response = $this->postJson('/api/v1/auth/login', [
            'mobile_number' => '+201001234567',
            'password' => 'wrong-password',
            'device_id' => 'device-abc-123',
        ]);

        $response->assertStatus(401);
    }

    public function test_me_returns_authenticated_user(): void
    {
        $user = User::factory()->create(['is_active' => true]);

        $response = $this->actingAs($user)
            ->getJson('/api/v1/auth/me');

        $response->assertOk()
            ->assertJsonPath('data.id', $user->id);
    }

    public function test_duplicate_mobile_registration_fails(): void
    {
        User::factory()->create(['mobile_number' => '+201001234567']);

        $response = $this->postJson('/api/v1/auth/register', [
            'display_name' => 'Another User',
            'mobile_number' => '+201001234567',
            'password' => 'SecurePass123!',
        ]);

        $response->assertStatus(422)
            ->assertJsonValidationErrors(['mobile_number']);
    }
}
