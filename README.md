# **VideoHosting**
Это платформа видеохостинга, построенная на микросервисной архитектуре. Она поддерживает:

-  Аутентификацию пользователей  
-  Загрузку и обработку видео (генерация миниатюр и HLS-потоков)
-  Предоставление доступа к видео через HLS-поток
-  Маршрутизацию через API-шлюз на базе YARP  

---

##  Архитектура проекта

Платформа состоит из следующих компонентов:

###  ApiGateway

- Выполняет маршрутизацию HTTP-запросов с помощью **YARP** (Yet Another Reverse Proxy)
- Проксирует запросы по путям:
  - `/auth-api/*` → **AuthService**
  - `/video-api/*` → **VideoService**
  - `/stream-api/*` → **StreamService**

###  AuthService

- Обрабатывает регистрацию и аутентификацию пользователей
- Выдаёт **JWT-токены** для доступа к защищённым эндпойнтам

###  VideoService

- Управляет загрузкой видео и их метаданными
- Отправляет сообщения `VideoCreated` в **RabbitMQ** для запуска обработки

###  StreamingService

- Подписывается на сообщения `HlsGenerated`, `VideoDeleted` из **RabbitMQ**
- Кэширует сегменты HLS-потока в Redis
- Предоставляет доступ к **HLS-потокам** для обработанных видео


###  ProcessingService

- Подписывается на сообщения `VideoCreated` из **RabbitMQ**
- Выполняет обработку видео с помощью **FFmpeg**:
  - Генерация **HLS-потоков**
  - Создание **миниатюр**
- Сохраняет обработанные файлы в локальное хранилище
- Публикует сообщение `VideoProcessed` в очередь, обновляя статус видео

###  Shared

- Содержит общие интерфейсы и контракты:
  - `IMessagePublisher` — интерфейс публикации сообщений
  - `VideoCreated`, `VideoProcessed`, `VideoDeleted`, `HlsGemerated` — сообщения для **RabbitMQ**

---

##  Технологический стек

- .NET 8, ASP.NET Core  
- PostgreSQL
- RabbitMQ (**MassTransit**)  
- YARP (API Gateway)
- Redis (Caching)
- FFmpeg (обёртка **NReco.VideoConverter**)  
- Entity Framework Core  

---

