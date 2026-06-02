<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('users', function (Blueprint $table) {
            $table->id();
            $table->string('display_name', 150);
            $table->string('mobile_number', 20)->unique()->nullable();
            $table->string('username', 80)->unique()->nullable();
            $table->string('password');
            $table->string('role', 50);
            $table->string('preferred_language', 5)->default('ar');
            $table->string('fcm_token', 512)->nullable();
            $table->string('active_device_id', 255)->nullable();
            $table->boolean('is_active')->default(true);
            $table->timestamp('email_verified_at')->nullable();
            $table->softDeletes();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('users');
    }
};
