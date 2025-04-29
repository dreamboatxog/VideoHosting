namespace VideoService.Domain.Enums;

public enum VideoStatus
{
        Uploading, // Загружается
        Processing, // Обрабатывается
        Completed,  // Обработка завершена
        Failed      // Ошибка при обработке
}
