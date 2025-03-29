# c_sharp_EFcore_Migrations_Seeding

## Создать миграцию

1. В терминале переходим в корень проекта `Infrastructure`
2. Выполняем команду:

```shell
dotnet ef migrations add Init --startup-project ../WebApi/WebApi.csproj
```

Ключ `--startup-project` нужен, т.к. настройки подключения лежат в другом проекте.
