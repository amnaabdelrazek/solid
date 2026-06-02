<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        Schema::create('groups', function (Blueprint $table) {
            $table->id();
            $table->foreignId('instructor_id')->nullable()->constrained('users')->nullOnDelete();
            $table->foreignId('substance_category_id')->nullable()->constrained()->nullOnDelete();
            $table->string('group_type', 50);
            $table->string('status', 50);
            $table->string('name_ar', 200);
            $table->string('name_en', 200);
            $table->unsignedTinyInteger('min_members')->default(7);
            $table->unsignedTinyInteger('max_members')->default(15);
            $table->softDeletes();
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('groups');
    }
};
