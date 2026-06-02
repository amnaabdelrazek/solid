<?php

namespace Modules\Notifications\Http\Controllers;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\Request;
use Modules\Notifications\Http\Resources\NotificationResource;

class NotificationsController extends ApiController
{
    /**
     * Display a listing of the resource.
     */
    public function index(Request $request)
    {
        $notifications = $request->user()->notifications()->paginate(20);

        return $this->apiBody([
            'notifications' => NotificationResource::collection($notifications),
            'pagination' => [
                'total' => $notifications->total(),
                'count' => $notifications->count(),
                'per_page' => $notifications->perPage(),
                'current_page' => $notifications->currentPage(),
                'total_pages' => $notifications->lastPage(),
            ],
        ])->apiResponse();
    }
}
