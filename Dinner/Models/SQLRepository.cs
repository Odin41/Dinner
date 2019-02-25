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

        public bool CheckDevice(int id)
        {
            logger.Info("Проверяется наличие устройства [ " + id + " ] в базе.");
            try
            {
                Device device = db.Devices.Find(id);
                if (device != null)
                {
                    logger.Info("Устройство [ " + device.Id + " ] успешно найдено.");
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error("Проверка наличия устройства [ " + id + " ] в базе завершилось с ошибкой.");
                throw ex;
            }
            logger.Warn("Устройство [ " + id + " ] не найдено в базе.");
            return false;
        }


        public async Task<bool> CheckDeviceAsync(int id)
        {
            logger.Info("Проверяется наличие устройства [ " + id + " ] в базе.");
            try
            {
                Device device = await db.Devices.FindAsync(id);
                if (device != null)
                {
                    logger.Info("Устройство [ " + device.Id + " ] успешно найдено.");
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                logger.Error("Проверка наличия устройства [ " + id + " ] в базе завершилось с ошибкой.");
                throw ex;
            }
            logger.Warn("Устройство [ " + id + " ] не найдено в базе.");
            return false;
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
            int result = -1;
            try
            {
                logger.Info("Получение устройств с билетами.");

                var devices = id == null || id < 0 ? db.Devices.Include(dt => dt.Tickets)
                  : db.Devices.Include(dt => dt.Tickets).Where(d => d.RoomId == id);

                if (devices != null && devices.Count() > 0)
                {
                    var selDevice = devices.Select(d => new
                    {
                        d.Id,
                        Count = d.Tickets.Where(t => t.CloseTime == null).Count()
                    });

                    logger.Info("Получение устройств с открытыми билетами успешно завершено.");

                    logger.Info("Сортировка полученных данных по количеству открытых билетов на устройство.");
                    var sortingDevices = selDevice.OrderBy(sd => sd.Count); //сортировка по возрастанию 
                    logger.Info("Сортировка полученных данных успешно завершено.");

                    logger.Info("Получение первой записи из отсортированных данных.");
                    var device = sortingDevices.FirstOrDefault();

                    if (device != null)
                    {
                        logger.Info("Получение первой записи из отсортированных данных успешно завершено.");
                        logger.Info("Получен идентификатор устройства [ " + device.Id + " ] с минимальным поличеством записей [ " + device.Count + " ].");
                        result = device.Id;
                    }
                    else
                    {
                        logger.Warn("Не удалось получить идентификатор устройства из отсортированных данных.");
                    }
                }
                else
                {
                    logger.Info("Открытых билетов нет.");
                    logger.Info("Получаем любое устройство удовлетворяющее запрос.");
                    var device = id == null || id < 0 ?
                        db.Devices.FirstOrDefault() :
                        db.Devices.Where(d => d.RoomId == id).FirstOrDefault();
                    logger.Info("Устройство удовлетворяющее запрос успешно получено. Илдентификатор полученного устройства: " + device.Id + ".");
                    result = device.Id;

                }
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение идентификатора устройства с минимальным количеством открытых билетов выполнилось с ошибкой. Параметр id: " + id + ".");
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Получение идентификатора устройства с минимальным количеством открытых билетов асинхронно
        /// </summary>
        /// <param name="id">Идентификатор комнаты, при отрицательном значение или null выборка по всем устройствам</param>
        /// <returns>Идентификатор устройства</returns>
        private async Task<int> GetDeviceWithMinimumOpenTicketsAsync(int? id)
        {
            logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов.");
            int result = -1;
            try
            {

                logger.Info("Получение устройств с билетами.");

                var devices = id == null || id < 0 ? db.Devices.Include(dt => dt.Tickets)
                  : db.Devices.Include(dt => dt.Tickets).Where(d => d.RoomId == id) ;

                if (devices != null && devices.Count() > 0)
                {
                    var selDevice = devices.Select(d => new
                    {
                        d.Id,
                        Count = d.Tickets.Where(t => t.CloseTime == null).Count()
                    });

                    logger.Info("Получение устройств с открытыми билетами успешно завершено.");

                    logger.Info("Сортировка полученных данных по количеству открытых билетов на устройство.");
                    var sortingDevices = selDevice.OrderBy(sd => sd.Count); //сортировка по возрастанию 
                    logger.Info("Сортировка полученных данных успешно завершено.");

                    logger.Info("Получение первой записи из отсортированных данных.");
                    var device = await sortingDevices.FirstOrDefaultAsync();

                    if (device != null)
                    {
                        logger.Info("Получение первой записи из отсортированных данных успешно завершено.");
                        logger.Info("Получен идентификатор устройства [ " + device.Id + " ] с минимальным поличеством записей [ " + device.Count + " ].");
                        result = device.Id;
                    }
                    else
                    {
                        logger.Warn("Не удалось получить идентификатор устройства из отсортированных данных.");
                    }
                }
                else
                {

                    logger.Info("Открытых билетов нет.");
                    logger.Info("Получаем любое устройство удовлетворяющее запрос.");
                    Task<Device> device = id == null || id < 0 ?
                        db.Devices.FirstOrDefaultAsync() :
                        db.Devices.Where(d => d.RoomId == id).FirstOrDefaultAsync();

                    logger.Info("Устройство удовлетворяющее запрос успешно получено. Илдентификатор полученного устройства: " + device.Result.Id + ".");
                    result = device.Result.Id;
                }
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException ex)
            {
                logger.Error(ex, "Получение идентификатора устройства с минимальным количеством открытых билетов выполнилось с ошибкой. Параметр id: " + id + ".");
                throw ex;
            }
            return result;
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