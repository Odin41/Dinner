using System;
using Dinner.Controllers;
using Dinner.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;



namespace Dinner.Tests.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void IndexView_ResultNotNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();

            moq.Setup(repo => repo.GetRooms()).Returns(GetTestRooms());

            moq.Setup(repo => repo.GetDevices()).Returns(GetTestDevices());

            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());

            HomeController homeController = new HomeController(moq.Object);
            
            //Act
            var result = homeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            Assert.NotEmpty(viewResult.ViewBag.rooms);
            Assert.NotEmpty(viewResult.ViewBag.devices);
            Assert.Equal("Index", viewResult.ViewName);
        }


        [Fact]
        public void IndexView_RedirectToLogout_IfUserNotFound()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync("admin")).ReturnsAsync(GetNullUser());
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.Index();

            //Assert
            var redirectResult = Assert.IsType<RedirectToRouteResult>(result.Result);

            Assert.Equal("Logout", redirectResult.RouteValues["action"]);
            Assert.Equal("Account", redirectResult.RouteValues["controller"]);
        }

        [Fact]
        public void IndexView_RedirectErrorPage_IfRoomsEmpty()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetRooms()).Returns(new List<Room>());
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void IndexView_RedirectErrorPage_IfDevicesEmpty()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());
            moq.Setup(repo => repo.GetDevices()).Returns(new List<Device>());
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result.Result);
            Assert.Equal("Error", viewResult.ViewName);
        }

        [Fact]
        public void Index_GetTicketForUserAsync_ResultNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetEmptyUser());
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.GetTicketForUserAsync();

            //Assert
            Assert.Same(result.Result, null);
        }

        [Fact]
        public void Index_GetTicketForUserAsync_ResultNotNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());

            Ticket resultTicket = GetTicket();

            moq.Setup(repo => repo.GetUserTicketAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(resultTicket);
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.GetTicketForUserAsync();

            //Assert
            var methodResult = Assert.IsType<Ticket>(result.Result);
            Assert.Same(methodResult, resultTicket);
        }



        [Fact]
        public void Index_GetTicketStatusAsync_IfGetTicketForUserRetunNotNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();

            ApplicationUser user = GetUser();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(user);

            Ticket resultTicket = GetTicket();

            moq.Setup(repo => repo.GetUserTicketAsync(user)).ReturnsAsync(resultTicket);
            HomeController homeController = new HomeController(moq.Object);

            string resultString_Ru = "Вы 0 в очереди к Device1 в Room1";

            //Act
            var result = homeController.GetTicketStatusAsync();

            //Assert
            var methodResult = Assert.IsType<string>(result.Result);
            Assert.Equal(methodResult, resultString_Ru);
        }


        [Fact]
        public void Index_GetTicketStatusAsync_IfGetTicketForUserRetunNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());
            moq.Setup(repo => repo.GetUserTicketAsync(null)).ReturnsAsync(GetTicket());

            string resultString = Resources.Resource.CheckQueueStatusNotFind;
            HomeController homeController = new HomeController(moq.Object);
            
            

            //Act
            var result = homeController.GetTicketStatusAsync();

            //Assert
            var methodResult = Assert.IsType<string>(result.Result);
            Assert.Equal(methodResult, resultString);
        }


        [Fact]
        public void Index_GetTicketNumberInQueueAsync_ReturnNotNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetUserAsync(null)).ReturnsAsync(GetUser());
            Ticket sendTicket = GetTicket();
            moq.Setup(repo => repo.GetUserTicketAsync(null)).ReturnsAsync(sendTicket);
            moq.Setup(repo => repo.GetNumberInQueueAsync(sendTicket)).ReturnsAsync(GetNumberInQueue());
            string resultString = Resources.Resource.CheckQueueStatusNotFind;
            HomeController homeController = new HomeController(moq.Object);

            //Act
            var result = homeController.GetTicketNumberInQueueAsync(sendTicket);

            //Assert
            var methodResult = Assert.IsType<int>(result.Result);
            Assert.Equal(methodResult, GetNumberInQueue());
        }







        private ApplicationUser GetEmptyUser()
        {
            return null;
        }

        private ApplicationUser GetUser()
        {
            return new ApplicationUser()
            {
                Id = "1",
                UserName = "admin"
            };
        }

        private ApplicationUser GetNullUser()
        {
            return null;
        }

        private List<Room> GetTestRooms()
        {
            var rooms = new List<Room>
            {
                new Room {Id=1, Name ="Room1" },
                new Room {Id=2, Name ="Room2" },
                new Room {Id=3, Name ="Room3" }
            };
            return rooms;
        }

        private List<Device> GetTestDevices()
        {
            var rooms = new List<Device>
            {
                new Device {Id=1, Name ="Device1", RoomId=1 },
                new Device {Id=2, Name ="Device2", RoomId=1 },
                new Device {Id=3, Name ="Device3", RoomId=1 },

                new Device {Id=4, Name ="Device1", RoomId=2 },
                new Device {Id=5, Name ="Device2", RoomId=2 },
                new Device {Id=6, Name ="Device3", RoomId=2 }
            };
            return rooms;
        }

        private List<Ticket> GetTickets()
        {
            var tickets = new List<Ticket>
            {
                new Ticket {Id = 1, DeviceId = 1, UserId = "1" },
                new Ticket {Id = 2, DeviceId = 1, UserId = "2" },
                new Ticket {Id = 3, DeviceId = 2, UserId = "3" },
                new Ticket {Id = 4, DeviceId = 3, UserId = "4" },
                new Ticket {Id = 5, DeviceId = 3, UserId = "5" }
            };
            return tickets;
        }

        private Ticket GetTicket()
        {
            Room room = new Room { Id = 1, Name = "Room1"};
            Device dev = new Device { Id = 1, Name = "Device1" };
            dev.Room = room;
            Ticket ret = new Ticket
            {
                Id = 1,
                DeviceId = 1,
                UserId = "1"
                
            };

            ret.Device = dev;

            return ret;

        }


        private int GetNumberInQueue()
        {
            return 1;
        }
    }
}
