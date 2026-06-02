<?php

namespace App\Support\Batch;

use App\Enums\QueueEnum;
use Illuminate\Bus\Batch;
use Illuminate\Bus\PendingBatch;
use Illuminate\Support\Facades\DB;

class Bus extends \Illuminate\Support\Facades\Bus
{
    public static function findBatchByName(string $batchName, $queue = null): PendingBatch|Batch
    {

        $batch = DB::table('job_batches')->where('name', $batchName)->first();

        if ($batch) {
            return static::findBatch($batch->id);
        }

        return Bus::batch([])->name($batchName)->onQueue($queue ?? QueueEnum::DEFAULT->value);
    }
}
