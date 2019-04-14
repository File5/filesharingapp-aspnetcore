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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using MockQueryable.Moq;

namespace FileSharingAppTest {
    public class FilesControllerTest {
        [Fact]
        public async void Index_ReturnAViewResult_WithListOfUsers() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            var files = new List<File>() { new File() };
            DbSet<File> filesDbSet = GetQueryableMockDbSet(files).Object;
            dbContext.Setup(c => c.Files).Returns(filesDbSet);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var data = Assert.IsAssignableFrom<List<File>>(viewResult.ViewData.Model);
            Assert.Equal(files.First(), data.First());
        }
        [Fact]
        public async void Details_ReturnAViewResult_WithFile() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            int fileId = 123;
            var files = new List<File>() { new File() { Id = fileId } };
            DbSet<File> filesDbSet = GetQueryableMockDbSet(files).Object;
            dbContext.Setup(c => c.Files).Returns(filesDbSet);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var result = await controller.Details(fileId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var data = Assert.IsAssignableFrom<File>(viewResult.ViewData.Model);
            Assert.Equal(files.First(), data);
        }
        [Fact]
        public void Create_ReturnAViewResult() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            var subjects = new List<Subject>() { new Subject() };
            DbSet<Subject> subjectsDbSet = GetQueryableMockDbSet(subjects).Object;
            dbContext.Setup(c => c.Subjects).Returns(subjectsDbSet);

            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Create_ReturnAViewResult_WithCreatedFile() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            int fileId = 123;
            var file = new File() { Id = fileId };
            var filedata = new Mock<MockFormFile>();
            var files = new List<File>() { file };

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var subjects = new List<Subject>() { new Subject() };
            DbSet<Subject> subjectsDbSet = GetQueryableMockDbSet(subjects).Object;
            dbContext.Setup(c => c.Subjects).Returns(subjectsDbSet);

            var result = controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async void Edit_ReturnAViewResult_WithFile() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            string userId = "userrId";
            AppUser user = new AppUser() { Id = userId };
            int fileId = 123;
            var files = new List<File>() { new File() { Id = fileId, UserId = userId, User = user } };
            Mock<DbSet<File>> filesDbSetMock = GetQueryableMockDbSet(files);
            filesDbSetMock.Setup(i => i.FindAsync(It.IsAny<int>())).ReturnsAsync(files.First());
            dbContext.Setup(c => c.Files).Returns(filesDbSetMock.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var subjects = new List<Subject>() { new Subject() };
            DbSet<Subject> subjectsDbSet = GetQueryableMockDbSet(subjects).Object;
            dbContext.Setup(c => c.Subjects).Returns(subjectsDbSet);

            var result = await controller.Edit(fileId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var data = Assert.IsAssignableFrom<File>(viewResult.ViewData.Model);
            Assert.Equal(files.First(), data);
        }
        [Fact]
        public async void Delete_ReturnAViewResult_WithFile() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            string userId = "userrId";
            AppUser user = new AppUser() { Id = userId };
            int fileId = 123;
            var files = new List<File>() { new File() { Id = fileId, UserId = userId, User = user } };
            Mock<DbSet<File>> filesDbSetMock = GetQueryableMockDbSet(files);
            //filesDbSetMock.Setup(i => i.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<File, bool>>>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(files.First());
            dbContext.Setup(c => c.Files).Returns(filesDbSetMock.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var subjects = new List<Subject>() { new Subject() };
            DbSet<Subject> subjectsDbSet = GetQueryableMockDbSet(subjects).Object;
            dbContext.Setup(c => c.Subjects).Returns(subjectsDbSet);

            var result = await controller.Delete(fileId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var data = Assert.IsAssignableFrom<File>(viewResult.ViewData.Model);
            Assert.Equal(files.First(), data);
        }
        [Fact]
        public async void DeleteConfimed_ReturnAViewResult() {
            var userStore = new Mock<IUserStore<AppUser>>();
            var userManagerMock = new Mock<UserManager<AppUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var dbContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            var controller = new FilesController(dbContext.Object, userManagerMock.Object);

            string userId = "userrId";
            AppUser user = new AppUser() { Id = userId };
            int fileId = 123;
            var files = new List<File>() { new File() { Id = fileId, UserId = userId, User = user } };
            Mock<DbSet<File>> filesDbSetMock = GetQueryableMockDbSet(files);
            filesDbSetMock.Setup(i => i.FindAsync(It.IsAny<int>())).ReturnsAsync(files.First());
            //filesDbSetMock.Setup(i => i.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<File, bool>>>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(files.First());
            dbContext.Setup(c => c.Files).Returns(filesDbSetMock.Object);

            var usersList = new List<AppUser> { new AppUser() };
            userManagerMock.Setup(um => um.Users).Returns(usersList.AsQueryable());
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(usersList.First());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>())).ReturnsAsync(true);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var subjects = new List<Subject>() { new Subject() };
            DbSet<Subject> subjectsDbSet = GetQueryableMockDbSet(subjects).Object;
            dbContext.Setup(c => c.Subjects).Returns(subjectsDbSet);

            var result = await controller.DeleteConfirmed(fileId);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            filesDbSetMock.Verify(i => i.Remove(It.Is<File>(f => f.Id == fileId)));
        }


        private static Mock<DbSet<T>> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class {
            var queryableMock = sourceList.AsQueryable().BuildMock();
            var queryable = queryableMock.Object;

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet;
        }
    }

    public abstract class MockFormFile : IFormFile {
        public abstract string ContentType { get; }
        public abstract string ContentDisposition { get; }
        public abstract IHeaderDictionary Headers { get; }

        public long Length => 0;
        public string Name => "FileName.txt";
        string IFormFile.FileName => "FileName.txt";

        public abstract void CopyTo(System.IO.Stream target);
        public abstract Task CopyToAsync(System.IO.Stream target, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        public abstract System.IO.Stream OpenReadStream();
    }
}
