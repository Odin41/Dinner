using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using NLog;

namespace Dinner.Models
{
    public class SQLRepository : IRepository
    {
        private ApplicationContext db;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SQLRepository()
        {
            db = new ApplicationContext();
        }

        public void CreateTicket(Ticket ticket)
        {
            db.Tickets.Add(ticket);
        }

        public void Dispose()
        {
            db.Dispose();
        }
       

        public IEnumerable<Device> GetDevices()
        {
           return db.Devices.ToList();
        }

        public int GetNumberInQueue(Ticket ticket)
        {
            try
            {
                logger.Info("Получение номера билета в очереди для билета с идентификатором [ " + ticket.Id + " ] из базы.");
                int count = db.Tickets.Count(t => t.CloseTime == null && t.Id < ticket.Id && t.DeviceId == ticket.DeviceId);
                return ++count;
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение номера билета в очереди для билета с идентификатором[" + ticket.Id + "] из базы выполнилось с ошибкой.");
                throw ex;
            }
            
        }

        public async Task<int> GetNumberInQueueAsync(Ticket ticket)
        {
            try
            {
                logger.Info("Получение номера билета в очереди для билета с идентификатором [ " + ticket.Id + " ] из базы.");
                int count = await db.Tickets.CountAsync(t => t.CloseTime == null && t.Id < ticket.Id && t.DeviceId == ticket.DeviceId);
                return ++count;
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение номера билета в очереди для билета с идентификатором[" + ticket.Id + "] из базы выполнилось с ошибкой.");
                throw ex;
            }
        }

        public IEnumerable<Room> GetRooms()
        {
            return db.Rooms.ToList();
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            try
            {
                logger.Info("Получение пользователя [ "+ userName +" ] из базы.");
                return await db.Users.FirstOrDefaultAsync(n => n.UserName == userName);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение пользователя ["+ userName +"] из базы выполнилось с ошибкой.");
                throw ex;
            }
            
        }

        public ApplicationUser GetUser(string userName)
        {
            try
            {
                logger.Info("Получение пользователя [ " + userName + " ] из базы.");
                return db.Users.FirstOrDefault(n => n.UserName == userName);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение пользователя [" + userName + "] из базы выполнилось с ошибкой.");
                throw ex;
            }
        }

        public Ticket GetUserTicket(ApplicationUser user)
        {
            try
            {
                logger.Info("Получение открытого билета для пользователя [" + user.UserName + "] из базы.");
                return db.Tickets.Include(t => t.Device).Include(d => d.Device.Room).FirstOrDefault(n => n.UserId == user.Id && n.CloseTime == null);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение открытого билета для пользователя [" + user.UserName + "] из базы выполнилось с ошибкой.");
                throw ex;
            }
            
        }

        public async Task<Ticket> GetUserTicketAsync(ApplicationUser user)
        {
            try
            {
                logger.Info("Получение открытого билета для пользователя [" + user.UserName + "] из базы.");
                return await db.Tickets.Include(t => t.Device).Include(d => d.Device.Room).FirstOrDefaultAsync(n => n.UserId == user.Id && n.CloseTime == null);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение открытого билета для пользователя [" + user.UserName + "] из базы выполнилось с ошибкой.");
                throw ex;
            }
        }

        public int GetDeviceId()
        {
            return GetDeviceWithMinimumOpenTickets(null);
        }

        public async Task<int> GetDeviceIdAsync()
        {
            return await GetDeviceWithMinimumOpenTicketsAsync(null);
        }

        public int GetDeviceIdByRoom(int id)
        {
            return GetDeviceWithMinimumOpenTickets(id);
        }

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством открытых билетов
        /// </summary>
        /// <param name="id">Идентификатор комнаты, при отрицательном значение или null выборка по всем устройствам</param>
        /// <returns>Идентификатор устройства</returns>
        private int GetDeviceWithMinimumOpenTickets(int? id)
        {
            logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов.");
            try
            {
                int result = -1;
                logger.Info("Получение открытых билетов.");
                var tickets = id == null || id < 0 ? db.Tickets.Where(t => t.CloseTime == null) : //Выборка открытых билетов для всех устройств
                    db.Tickets.Where(t => t.Device.RoomId == id && t.CloseTime == null); //Выборка открытых билетов в указанной комнате

                if (tickets != null)
                {
                    logger.Info("Получение открытых билетов успешно завершено.");

                    logger.Info("Группировка полученных данных по идентификатору устройства.");
                    var groupTickets = tickets.GroupBy(d => d.DeviceId); //группировка по устройству

                    logger.Info("Группировка полученных данных по идентификатору устройства успешно завершено. Количество полученных записей: " + groupTickets.Count());

                    logger.Info("Создание нового типа с полями key, count.");
                    var groupDevices = groupTickets.Select(gd => new //создаие нового объекта (deviceid, ticetsCount)
                    {
                        gd.Key,
                        Count = gd.Count()
                    });
                    logger.Info("Создание нового типа с полями key, count успешно завершено.");

                    logger.Info("Сортировка полученных данных.");
                    var sortingDevices = groupDevices.OrderBy(sd => sd.Count); //сортировка по возрастанию 
                    logger.Info("Сортировка полученных данных успешно завершено.");



                    logger.Info("Получение первой записи из из отсортированных данных.");
                    var device = sortingDevices.FirstOrDefault();


                    if (device != null)
                    {
                        logger.Info("Получение первой записи из отсортированных данных успешно завершено.");
                        logger.Info("Получен идентификатор устройства [ " + device.Key + " ] с минимальным поличеством записей [ " + device.Count + " ].");
                        result = device.Key;
                    }
                    else
                    {
                        logger.Warn("Не удалось получить идентификатор устройства из отсортированных данных.");
                    }
                }
                {
                    logger.Info("Открытых билетов нет.");
                }

                return result;
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение идентификатора устройства с минимальным количеством открытых билетов выполнилось с ошибкой. Параметр id: " + id + ".");
                throw ex;
            }
        }

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством открытых билетов асинхронно
        /// </summary>
        /// <param name="id">Идентификатор комнаты, при отрицательном значение или null выборка по всем устройствам</param>
        /// <returns>Идентификатор устройства</returns>
        private async Task<int> GetDeviceWithMinimumOpenTicketsAsync(int? id)
        {
            logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов.");
            try
            {
                int result = -1;
            logger.Info("Получение открытых билетов.");
            var tickets = id == null || id < 0  ? db.Tickets.Where(t => t.CloseTime == null) : //Выборка открытых билетов для всех устройств
                db.Tickets.Where(t => t.Device.RoomId == id && t.CloseTime == null); //Выборка открытых билетов в указанной комнате

            if (tickets != null)
            {
                logger.Info("Получение открытых билетов успешно завершено.");

                logger.Info("Группировка полученных данных по идентификатору устройства.");
                var groupTickets = tickets.GroupBy(d => d.DeviceId); //группировка по устройству

                logger.Info("Группировка полученных данных по идентификатору устройства успешно завершено. Количество полученных записей: " + groupTickets.Count());

                logger.Info("Создание нового типа с полями key, count.");
                var groupDevices = groupTickets.Select(gd => new //создаие нового объекта (deviceid, ticetsCount)
                {
                    gd.Key,
                    Count = gd.Count()
                });
                logger.Info("Создание нового типа с полями key, count успешно завершено.");

                logger.Info("Сортировка полученных данных.");
                var sortingDevices = groupDevices.OrderBy(sd => sd.Count); //сортировка по возрастанию 
                logger.Info("Сортировка полученных данных успешно завершено.");



                logger.Info("Получение первой записи из из отсортированных данных.");
                var device = await sortingDevices.FirstOrDefaultAsync();


                if (device != null)
                {
                    logger.Info("Получение первой записи из отсортированных данных успешно завершено.");
                    logger.Info("Получен идентификатор устройства [ " + device.Key + " ] с минимальным поличеством записей [ " + device.Count + " ].");
                    result = device.Key;
                }
                else
                {
                    logger.Warn("Не удалось получить идентификатор устройства из отсортированных данных.");
                }
            }
            {
                logger.Info("Открытых билетов нет.");
            }

            return result;
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение идентификатора устройства с минимальным количеством открытых билетов выполнилось с ошибкой. Параметр id: " + id + ".");
                throw ex;
            }
        }

        public async Task<int> GetDeviceIdByRoomAsync(int id)
        {
            return await GetDeviceWithMinimumOpenTicketsAsync(id);
        }

        public void Save()
        {
            logger.Info("Сохранение всех изменений в базе.");
            try
            {
                db.SaveChanges();
                logger.Info("Сохранение всех изменений в базе прошло успешно.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Сохранение всех изменений в базе прошло с ошибкой.");
            }
        }

        public IQueryable<Ticket> GetAllOpenTickets()
        {
            try
            {
                logger.Info("Получение списка всех открытых билетов.");
                var tickets = db.Tickets.Include(t => t.Device)
                                    .Include(d => d.Device.Room)
                                    .Include(u => u.User)
                               .Where(t => t.CloseTime == null);
                return tickets;
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение списка всех открытых билетов выполнилось с ошибкой.");
                throw ex;
            }

        }

        public void CloseTicket(int id)
        {
            db.Tickets.Find(id).CloseTime = DateTime.Now;
        }

        public async Task<int> CloseTicketAsync(int id)
        {
            var ticket = await db.Tickets.FindAsync(id);
            ticket.CloseTime = DateTime.Now;
            return id;
        }


    }
}