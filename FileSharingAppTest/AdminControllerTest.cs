using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileSharingApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FileSharingApp.Data;
using Microsoft.AspNetCore.Authorization;
using FileSharingApp.Controllers;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace FileSharingAppTest {
    public class AdminControllerTest {
        [Fact]
        public async void Index_ReturnAViewResult_WithListOfUsers() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new AdminController(userManagerMock.Object, roleManagerMock.Object, dbContext.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<UserWithRole>>(viewResult.ViewData.Model);
            Assert.Equal(usersList.First(), data.First().User);
        }

        [Fact]
        public async void ChangeRole_AddToAdmin_WhenUserIsNotAdmin() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new AdminController(userManagerMock.Object, roleManagerMock.Object, dbContext.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(false);

            var result = await controller.ChangeRole("id");

            userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<AppUser>(), It.Is<string>(i => i.Equals("Admin"))));
        }

        [Fact]
        public async void ChangeRole_RemovesFromAdmin_WhenUserIsNotAdmin() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new AdminController(userManagerMock.Object, roleManagerMock.Object, dbContext.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            var result = await controller.ChangeRole("id");

            userManagerMock.Verify(um => um.RemoveFromRoleAsync(It.IsAny<AppUser>(), It.Is<string>(i => i.Equals("Admin"))));
        }
    }
}
