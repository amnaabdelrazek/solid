<?php

namespace Modules\Sessions\Http\Controllers;

use App\Http\Controllers\Api\ApiController;
use Illuminate\Http\Request;

class SessionsController extends ApiController
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        return view('sessions::index');
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('sessions::create');
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
        return view('sessions::show');
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit($id)
    {
        return view('sessions::edit');
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
