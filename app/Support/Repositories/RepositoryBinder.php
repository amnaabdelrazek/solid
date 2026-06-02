<?php

namespace App\Support\Repositories;

use Illuminate\Support\Facades\File;
use Nwidart\Modules\Facades\Module;

class RepositoryBinder
{
    public function bind($app): void
    {
        foreach (Module::all() as $module) {

            $moduleName = $module->getName();
            $repositoryPath = base_path("Modules/{$moduleName}/Repositories");

            if (! File::exists($repositoryPath)) {
                continue;
            }

            foreach (File::allFiles($repositoryPath) as $file) {

                $fileName = $file->getFilenameWithoutExtension();

                // اشتغل على الـ interfaces فقط
                if (! str_ends_with($fileName, 'Interface')) {
                    continue;
                }

                $baseName = str_replace('Interface', '', $fileName);

                $interface = "Modules\\{$moduleName}\\Repositories\\{$fileName}";
                $repository = "Modules\\{$moduleName}\\Repositories\\{$baseName}";
                $eloquentRepository = "Modules\\{$moduleName}\\Repositories\\Eloquent{$baseName}";

                if (class_exists($eloquentRepository)) {
                    $app->bind($interface, $eloquentRepository);
                } elseif (class_exists($repository)) {
                    $app->bind($interface, $repository);
                } else {
                    throw new \Exception("No implementation found for {$interface}");
                }
            }
        }
    }
}
