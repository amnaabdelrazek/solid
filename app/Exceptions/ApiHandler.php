<?php

namespace App\Exceptions;

use App\Exceptions\ApiException\ApiException;
use Illuminate\Auth\Access\AuthorizationException;
use Illuminate\Auth\AuthenticationException;
use Illuminate\Database\Eloquent\ModelNotFoundException;
use Illuminate\Http\JsonResponse;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Log;
use Illuminate\Validation\ValidationException;
use Symfony\Component\HttpKernel\Exception\HttpException;
use Symfony\Component\HttpKernel\Exception\MethodNotAllowedHttpException;
use Symfony\Component\HttpKernel\Exception\NotFoundHttpException;
use Throwable;

class ApiHandler
{
    private function jsonResponse(int $status, string $message, mixed $errors = null): JsonResponse
    {
        $response = [
            'status' => false,
            'message' => $message,
        ];

        if (! is_null($errors)) {
            $response['errors'] = $errors;
        }

        return response()->json($response, $status);
    }

    public function handle(Throwable $e, Request $request): ?JsonResponse
    {
        // Only handle API requests
        if (! $request->is('api/*') && ! $request->expectsJson()) {
            return null;
        }

        Log::error($e);

        return match (true) {
            $e  instanceof ApiException => $e->toResponse(),
            $e instanceof ValidationException => $this->validation($e),
            $e instanceof AuthenticationException => $this->unauthenticated(),
            $e instanceof AuthorizationException => $this->unauthorized($e),
            $e instanceof ModelNotFoundException || $e->getPrevious() instanceof ModelNotFoundException => $this->modelNotFound($e instanceof ModelNotFoundException ? $e : $e->getPrevious()),
            $e instanceof NotFoundHttpException => $this->notFound($e),
            $e instanceof MethodNotAllowedHttpException => $this->methodNotAllowed(),
            $e instanceof HttpException => $this->http($e),
            default => $this->serverError($e),
        };
    }

    private function validation(ValidationException $e): JsonResponse
    {
        return $this->jsonResponse(
            422,
            trans('messages.validation'),
            $e->errors()
        );
    }

    private function unauthenticated(): JsonResponse
    {
        return $this->jsonResponse(401, trans('messages.unauthenticated'));
    }

    private function unauthorized(AuthorizationException $e): JsonResponse
    {
        return $this->jsonResponse(
            403,
            $e->getMessage() ?: trans('messages.unauthorized')
        );
    }

    private function modelNotFound(ModelNotFoundException $e): JsonResponse
    {
        return $this->jsonResponse(
            404,
            trans('messages.model_not_found', [
                'model' => class_basename($e->getModel()),
            ])
        );
    }

    private function notFound($e): JsonResponse
    {
        return $this->jsonResponse(404, trans('messages.endpoint_not_found'));
    }

    private function methodNotAllowed(): JsonResponse
    {
        return $this->jsonResponse(405, trans('messages.method_not_allowed'));
    }

    private function http(HttpException $e): JsonResponse
    {
        return $this->jsonResponse(
            $e->getStatusCode(),
            $e->getMessage() ?: trans('messages.http_error')
        );
    }

    private function serverError(Throwable $e): JsonResponse
    {
        if (! app()->isProduction()) {
            return $this->jsonResponse(500, $e->getMessage(), [
                'exception' => get_class($e),
                'file' => $e->getFile(),
                'line' => $e->getLine(),
                'trace' => collect($e->getTrace())->take(5)->toArray(), // limit trace
            ]);
        }

        return $this->jsonResponse(500, trans('messages.unexpected_error'));
    }
}
