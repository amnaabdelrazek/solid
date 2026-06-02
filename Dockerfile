# ======================
# Stage 1: Build
# ======================
FROM php:8.4-cli AS builder

RUN apt-get update && apt-get install -y \
    git \
    curl \
    libpng-dev \
    libonig-dev \
    libxml2-dev \
    libzip-dev \
    libicu-dev \
    unzip \
    nodejs \
    npm \
    && docker-php-ext-install pdo_mysql mbstring exif pcntl bcmath gd intl zip \
    && rm -rf /var/lib/apt/lists/*

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /var/www

# Cache npm dependencies
COPY package.json package-lock.json* ./
RUN npm ci || npm install

# Cache composer dependencies
COPY composer.json composer.lock* ./
RUN composer install --no-dev --optimize-autoloader --no-scripts --prefer-dist --ignore-platform-reqs

# Copy all source files (including artisan)
COPY . .

# Ensure required directories exist
RUN mkdir -p bootstrap/cache storage/logs storage/framework/cache storage/framework/sessions storage/framework/views

# Run package discovery (skipped by --no-scripts)
RUN php artisan package:discover

# Setup .env with SQLite for build-time artisan commands
RUN cp .env.example .env \
    && sed -i 's|^DB_CONNECTION=.*|DB_CONNECTION=sqlite|' .env \
    && sed -i '/^# DB_HOST/d;/^# DB_PORT/d;/^# DB_DATABASE/d;/^# DB_USERNAME/d;/^# DB_PASSWORD/d' .env \
    && mkdir -p database \
    && touch database/database.sqlite \
    && php artisan key:generate --force

# Run migrations and seed database
RUN php artisan migrate --force && \
    php artisan migrate --force --path=Modules/Auth/Database/migrations && \
    php artisan migrate --force --path=Modules/Groups/Database/migrations && \
    php artisan migrate --force --path=Modules/Lookups/Database/migrations && \
    php artisan migrate --force --path=Modules/Notifications/Database/migrations && \
    php artisan migrate --force --path=Modules/Payments/Database/migrations && \
    php artisan migrate --force --path=Modules/Recommendations/Database/migrations && \
    php artisan migrate --force --path=Modules/Sessions/Database/migrations && \
    php artisan migrate --force --path=Modules/User/Database/migrations && \
    php artisan db:seed

# Build frontend assets
RUN npm run build

# Cache Laravel configurations
# Remove fat dev directories to avoid copying them to the runtime container
RUN rm -rf node_modules .git tests storage/logs/*

# ======================
# Stage 2: Runtime
# ======================
FROM php:8.4-fpm-alpine

# Install essential runtime libraries and PHP extensions
RUN apk add --no-cache \
    nginx \
    curl \
    libpng \
    libpng-dev \
    oniguruma \
    oniguruma-dev \
    libxml2 \
    libxml2-dev \
    libzip \
    libzip-dev \
    icu \
    icu-dev \
    tzdata \
    bash \
    supervisor \
    && docker-php-ext-install pdo_mysql mbstring exif pcntl bcmath gd intl zip \
    && apk del libpng-dev oniguruma-dev libxml2-dev libzip-dev icu-dev \
    && mkdir -p /var/log/supervisor

WORKDIR /var/www

# Copy the lean application folder directly from builder
COPY --from=builder /var/www /var/www

# Create needed dirs
RUN mkdir -p \
    storage/logs \
    storage/framework/cache \
    storage/framework/sessions \
    storage/framework/views \
    bootstrap/cache

# Permissions
RUN chown -R www-data:www-data /var/www \
    && chmod -R 775 storage \
    && chmod -R 775 bootstrap/cache

# Copy Supervisor configuration
COPY docker/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# ======================
# NGINX CONFIG (FIXED)
# ======================
RUN cat <<'EOF' > /etc/nginx/http.d/default.conf
server {
    listen 80;
    server_name _;
    root /var/www/public;

    index index.php;
    charset utf-8;

    error_log /var/log/nginx/error.log;
    access_log /var/log/nginx/access.log;

    location / {
        try_files $uri $uri/ /index.php?$query_string;
    }

    location = /favicon.ico { access_log off; log_not_found off; }
    location = /robots.txt { access_log off; log_not_found off; }

    error_page 404 /index.php;

    location ~ \.php$ {
        fastcgi_pass 127.0.0.1:9000;
        fastcgi_param SCRIPT_FILENAME $realpath_root$fastcgi_script_name;
        include fastcgi_params;
    }

    location ~ /\.(?!well-known).* {
        deny all;
    }
}
EOF

# ======================
# START SCRIPT
# ======================
RUN cat <<'EOF' > /start.sh
#!/bin/sh

echo "Running migrations..."
php artisan migrate --force

echo "Caching configurations..."
php artisan optimize

echo "Starting Supervisor..."
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf
EOF

RUN chmod +x /start.sh

EXPOSE 80

CMD ["/start.sh"]
