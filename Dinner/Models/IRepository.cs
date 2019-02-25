using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Dinner.Models
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Получение модели пользователя по логину
        /// </summary>
        /// <param name="userName">Логин пользователя</param>
        /// <returns>Модель пользователя</returns>
        ApplicationUser GetUser(string userName);

        /// <summary>
        /// Получение модели пользователя по логину асинхронно
        /// </summary>
        /// <param name="userName">Логин пользователя</param>
        /// <returns>Модель пользователя</returns>
        Task<ApplicationUser> GetUserAsync(string userName);

        /// <summary>
        /// Получение модели Ticket из базы по пользователю
        /// </summary>
        /// <param name="user">Модель пользователя</param>
        /// <returns>Модель билета</returns>
        Ticket GetUserTicket(ApplicationUser user);

        /// <summary>
        /// Получение модели Ticket из базы по пользователю асинхронно
        /// </summary>
        /// <param name="user">Модель пользователя</param>
        /// <returns>Модель билета</returns>
        Task<Ticket> GetUserTicketAsync(ApplicationUser user);

        /// <summary>
        /// Получение списка комнат из базы
        /// </summary>
        /// <returns>Список комнат</returns>
        IEnumerable<Room> GetRooms();

        /// <summary>
        /// Получение списка устройств из базы
        /// </summary>
        /// <returns>Список устройств</returns>
        IEnumerable<Device> GetDevices();

        /// <summary>
        /// Проверяется наличие устройства в базе
        /// </summary>
        /// <param name="id">Идентификатор проверяемоо устройства</param>
        /// <returns>Результат проверки</returns>
        bool CheckDevice(int id);

        /// <summary>
        /// Проверяется наличие устройства в базе асинхронно
        /// </summary>
        /// <param name="id">Идентификатор проверяемоо устройства</param>
        /// <returns>Результат проверки</returns>
        Task<bool> CheckDeviceAsync(int id);

        /// <summary>
        /// Создание и сохранение в базу модели Ticket
        /// </summary>
        /// <param name="ticket">Билет для сохранения в базу</param>
        void CreateTicket(Ticket ticket);

        /// <summary>
        /// Закрытие билета по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор билета</param>
        void CloseTicket(int id);

        /// <summary>
        /// Закрытие билета по его идентификатору асинхронно
        /// </summary>
        /// <param name="id">Идентификатор билета, который надо закрыть</param>
        /// <returns>Идентификатор закрытого билета </returns>
        Task<int> CloseTicketAsync(int id);

        /// <summary>
        /// Получение количества незакрытых билетов перед указаным билетом к устройству указанному в билете
        /// </summary>
        /// <param name="ticket">Билет для которого идет проверка</param>
        /// <returns>Количество незакрытых билетов перед текущим</returns>
        int GetNumberInQueue(Ticket ticket);

        /// <summary>
        /// Получение количества незакрытых билетов перед указаным билетом к устройству указанному в билете асинхронно
        /// </summary>
        /// <param name="ticket">Билет для которого идет проверка</param>
        /// <returns>Количество незакрытых билетов перед текущим</returns>
        Task<int> GetNumberInQueueAsync(Ticket ticket);

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством активных билетов
        /// </summary>
        /// <returns>Идентификатор устройства</returns>
        int GetDeviceId();


        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством активных билетов асинхронно
        /// </summary>
        /// <returns>Идентификатор устройства</returns>
        Task<int> GetDeviceIdAsync();

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством активных билетов в указанной комнате
        /// </summary>
        /// <param name="id">Идентификатор комнаты для фильтрации просматриваемых устройств</param>
        /// <returns>Идентификатор устройства</returns>
        int GetDeviceIdByRoom(int id);

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством активных билетов в указанной комнате асинхронно
        /// </summary>
        /// <param name="id">Идентификатор комнаты для фильтрации просматриваемых устройств</param>
        /// <returns>Идентификатор устройства</returns>
        Task<int> GetDeviceIdByRoomAsync(int id);

        /// <summary>
        /// Сохранение всех изменений в базу
        /// </summary>
        void Save();


        /// <summary>
        /// Получение связанной таблицы открытых билетов с устройствами, комнатами и пользователями
        /// </summary>
        /// <returns>Связанный список</returns>
        IQueryable<Ticket> GetAllOpenTickets();

    }
}