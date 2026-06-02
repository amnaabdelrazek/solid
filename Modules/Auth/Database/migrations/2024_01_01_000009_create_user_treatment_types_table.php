<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('user_treatment_types', function (Blueprint $table) {
            $table->id();
            $table->foreignId('user_id')->constrained()->cascadeOnDelete();
            $table->foreignId('lookup_value_id')->constrained()->cascadeOnDelete();
            $table->unique(['user_id', 'lookup_value_id']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('user_treatment_types');
    }
};
