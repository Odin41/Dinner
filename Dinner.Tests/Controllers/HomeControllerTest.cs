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
        [Authorize]
        public void IndexViewResultNotNull()
        {
            //Arrange
            var moq = new Mock<IRepository>();
            moq.Setup(repo => repo.GetRooms()).Returns(GetTestRooms());
            moq.Setup(repo => repo.GetDevices()).Returns(GetTestDevices());
            HomeController homeController = new HomeController(moq.Object);
            
            //Act
            var result = homeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotEmpty(viewResult.ViewBag.rooms);
            Assert.NotEmpty(viewResult.ViewBag.devices);
        }

        private ApplicationUser GetUser(string name)
        {
            
            ApplicationUser user = new ApplicationUser { UserName = "admin@email.com", Email = "admin@email.com" };


            return user;
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
    }
}
