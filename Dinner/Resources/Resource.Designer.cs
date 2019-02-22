﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Dinner.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь {0} вышел из системы..
        /// </summary>
        public static string AccountExitMessage {
            get {
                return ResourceManager.GetString("AccountExitMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Не удалось получить HttpContext..
        /// </summary>
        public static string AccountHttpContextError {
            get {
                return ResourceManager.GetString("AccountHttpContextError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Действие.
        /// </summary>
        public static string ActionTableHeaderLabel {
            get {
                return ResourceManager.GetString("ActionTableHeaderLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Неверный логин или пароль..
        /// </summary>
        public static string AuthenticationError {
            get {
                return ResourceManager.GetString("AuthenticationError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Закрыть.
        /// </summary>
        public static string ButtonCloseLabel {
            get {
                return ResourceManager.GetString("ButtonCloseLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Вы {0} в очереди к {1} в {2}.
        /// </summary>
        public static string CheckQueueStatusFind {
            get {
                return ResourceManager.GetString("CheckQueueStatusFind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Вы не стоите в очереди.
        /// </summary>
        public static string CheckQueueStatusNotFind {
            get {
                return ResourceManager.GetString("CheckQueueStatusNotFind", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Проверить очередь.
        /// </summary>
        public static string CheckStatusQueue {
            get {
                return ResourceManager.GetString("CheckStatusQueue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выберите страну.
        /// </summary>
        public static string ChooseLang {
            get {
                return ResourceManager.GetString("ChooseLang", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Время закрытия.
        /// </summary>
        public static string ClosedTimeTableHeaderLabel {
            get {
                return ResourceManager.GetString("ClosedTimeTableHeaderLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Подтвердите пароль.
        /// </summary>
        public static string ConfirmPassword {
            get {
                return ResourceManager.GetString("ConfirmPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Время открытия.
        /// </summary>
        public static string CreatedTimeTableHeaderLabel {
            get {
                return ResourceManager.GetString("CreatedTimeTableHeaderLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка базы данных. {0}.\n.
        /// </summary>
        public static string DataBaseErrorMessage {
            get {
                return ResourceManager.GetString("DataBaseErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Требуется миграция..
        /// </summary>
        public static string DataBaseNeedMigrationMessage {
            get {
                return ResourceManager.GetString("DataBaseNeedMigrationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Таблица &quot;Devices&quot; не содержит записей..
        /// </summary>
        public static string DeviceTableEmptyError {
            get {
                return ResourceManager.GetString("DeviceTableEmptyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Устройство.
        /// </summary>
        public static string DeviceTableHeaderLabel {
            get {
                return ResourceManager.GetString("DeviceTableHeaderLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Электронный адрес.
        /// </summary>
        public static string Email {
            get {
                return ResourceManager.GetString("Email", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Укажите электронный адрес.
        /// </summary>
        public static string EmailRequired {
            get {
                return ResourceManager.GetString("EmailRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на English.
        /// </summary>
        public static string EnglishLang {
            get {
                return ResourceManager.GetString("EnglishLang", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Войти.
        /// </summary>
        public static string EnterButtonLabel {
            get {
                return ResourceManager.GetString("EnterButtonLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка на странице {0} \n.
        /// </summary>
        public static string ErrorOnThePageMessage {
            get {
                return ResourceManager.GetString("ErrorOnThePageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка..
        /// </summary>
        public static string ErrorPageHeader {
            get {
                return ResourceManager.GetString("ErrorPageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на При обработке запроса произошла ошибка..
        /// </summary>
        public static string ErrorPageMessage {
            get {
                return ResourceManager.GetString("ErrorPageMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Попробывать снова.
        /// </summary>
        public static string ErrorPageReturnUrl {
            get {
                return ResourceManager.GetString("ErrorPageReturnUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка.
        /// </summary>
        public static string ErrorPageTitle {
            get {
                return ResourceManager.GetString("ErrorPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Выход.
        /// </summary>
        public static string ExitButtonLabel {
            get {
                return ResourceManager.GetString("ExitButtonLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Получение страницы &quot;{0}&quot;. Пользователь: {1}..
        /// </summary>
        public static string GetPageLogMessage {
            get {
                return ResourceManager.GetString("GetPageLogMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Домашняя страница.
        /// </summary>
        public static string HomePageTitle {
            get {
                return ResourceManager.GetString("HomePageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Логин.
        /// </summary>
        public static string Login {
            get {
                return ResourceManager.GetString("Login", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Авторизация.
        /// </summary>
        public static string LoginPageTitle {
            get {
                return ResourceManager.GetString("LoginPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Ошибка в модели..
        /// </summary>
        public static string ModelIsNotValidError {
            get {
                return ResourceManager.GetString("ModelIsNotValidError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пароль.
        /// </summary>
        public static string Password {
            get {
                return ResourceManager.GetString("Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пароли не совпадают.
        /// </summary>
        public static string PasswordConfirmError {
            get {
                return ResourceManager.GetString("PasswordConfirmError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Повторите пароль.
        /// </summary>
        public static string PasswordConfirmRequired {
            get {
                return ResourceManager.GetString("PasswordConfirmRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Укажите пароль.
        /// </summary>
        public static string PasswordRequired {
            get {
                return ResourceManager.GetString("PasswordRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Информация об очереди:.
        /// </summary>
        public static string QueueInfoLabel {
            get {
                return ResourceManager.GetString("QueueInfoLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Управление учередью.
        /// </summary>
        public static string QueueManagerHeader {
            get {
                return ResourceManager.GetString("QueueManagerHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Зарегистрировать.
        /// </summary>
        public static string RegisterButtonLabel {
            get {
                return ResourceManager.GetString("RegisterButtonLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Регистрация.
        /// </summary>
        public static string RegisterPageTitle {
            get {
                return ResourceManager.GetString("RegisterPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Таблица &quot;Rooms&quot; не содержит записей..
        /// </summary>
        public static string RoomTableEmptyError {
            get {
                return ResourceManager.GetString("RoomTableEmptyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Комната.
        /// </summary>
        public static string RoomTableHeaderLabel {
            get {
                return ResourceManager.GetString("RoomTableHeaderLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Русский.
        /// </summary>
        public static string RussianLang {
            get {
                return ResourceManager.GetString("RussianLang", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Встать в очередь.
        /// </summary>
        public static string TakeQueue {
            get {
                return ResourceManager.GetString("TakeQueue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь: {0} не найден..
        /// </summary>
        public static string UserNotFoundError {
            get {
                return ResourceManager.GetString("UserNotFoundError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Пользователь.
        /// </summary>
        public static string UserTableHeaderLabel {
            get {
                return ResourceManager.GetString("UserTableHeaderLabel", resourceCulture);
            }
        }
    }
}
