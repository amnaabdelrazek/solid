<?php

namespace Modules\Recommendations\Http\Controllers;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\Request;

class RecommendationsController extends ApiController
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        return view('recommendations::index');
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('recommendations::create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request) {}

    /**
     * Show the specified resource.
     */
    public function show($id)
    {
        return view('recommendations::show');
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit($id)
    {
        return view('recommendations::edit');
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, $id) {}

    /**
     * Remove the specified resource from storage.
     */
    public function destroy($id) {}
}
