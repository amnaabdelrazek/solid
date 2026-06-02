<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('lookup_types', function (Blueprint $table) {
            $table->id();
            $table->string('key', 80)->unique();
            $table->string('label_ar', 150);
            $table->string('label_en', 150);
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('lookup_types');
    }
};
