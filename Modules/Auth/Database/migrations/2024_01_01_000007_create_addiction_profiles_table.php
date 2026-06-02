<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('addiction_profiles', function (Blueprint $table) {
            $table->id();
            $table->foreignId('user_id')->unique()->constrained()->cascadeOnDelete();
            $table->foreignId('addiction_duration_id')->constrained('lookup_values');
            $table->foreignId('education_level_id')->constrained('lookup_values');
            $table->boolean('had_prior_treatment')->default(false);
            $table->text('addiction_reason')->nullable();
            $table->unsignedInteger('days_clean')->nullable();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('addiction_profiles');
    }
};
