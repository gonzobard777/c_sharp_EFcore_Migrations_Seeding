# c_sharp_EFcore_Migrations_Seeding

## Создать миграцию

1. Открыть в терминале корень проекта `Infrastructure`
2. Выполнить команду:

```shell
dotnet ef migrations add Init --startup-project ../WebApi/WebApi.csproj
```

Ключ `--startup-project` нужен, т.к. настройки подключения лежат в другом проекте.
