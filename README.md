
## Настройка User Secrets для Email:

```bash
dotnet user-secrets set "MonitoringSettings:Email:Username" "your-email@gmail.com"
dotnet user-secrets set "MonitoringSettings:Email:Password" "your-app-password"
```

### Конфигурация

Основные настройки находятся в `appsettings.json`:
- ServiceUrl: URL сервиса для мониторинга
- CheckIntervalSeconds: интервал проверки в секундах
- LogFilePath: путь к файлу логов
- Email: настройки SMTP (кроме учетных данных)

## Запуск

```bash
dotnet run
```