<?php

namespace App\Filament\Widgets;

use Filament\Widgets\StatsOverviewWidget as BaseWidget;
use Filament\Widgets\StatsOverviewWidget\Stat;
use Modules\Groups\Models\Group;
use Modules\Lookups\Models\LookupType;
use Modules\Payments\Models\Payment;
use Modules\Sessions\Models\Session;
use Modules\User\Models\User;

class StatsOverviewWidget extends BaseWidget
{
    protected static ?int $sort = 1;

    protected function getStats(): array
    {
        return [
            Stat::make('Total Users', User::count())
                ->description('Registered users')
                ->descriptionIcon('heroicon-o-users')
                ->color('info')
                ->chart([7, 3, 10, 5, 15, 12, 20]),

            Stat::make('Active Groups', Group::where('status', 'active')->count())
                ->description('Currently active groups')
                ->descriptionIcon('heroicon-o-user-group')
                ->color('success')
                ->chart([4, 7, 5, 10, 8, 12, 15]),

            Stat::make('Today\'s Sessions', Session::whereDate('scheduled_at', today())->count())
                ->description('Sessions scheduled for today')
                ->descriptionIcon('heroicon-o-calendar-days')
                ->color('warning')
                ->chart([2, 5, 3, 7, 4, 6, 8]),

            Stat::make('Total Revenue', number_format(Payment::where('status', 'paid')->sum('amount'), 2))
                ->description('All time revenue')
                ->descriptionIcon('heroicon-o-currency-dollar')
                ->color('success'),

            Stat::make('Lookup Types', LookupType::count())
                ->description('Defined lookup categories')
                ->descriptionIcon('heroicon-o-list-bullet')
                ->color('gray'),
        ];
    }
}
