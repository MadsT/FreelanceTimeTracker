using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FreelanceTimeTracker.Controllers;
using Moq;
using System.Web.Mvc;
using FreelanceTimeTracker.Models;
using System.Collections.Generic;

namespace FreelanceTimeTracker.Tests.Controllers
{
    [TestClass]
    public class ProjectsServicesControllerTest
    {
        private static readonly string TEST_USER_NAME = "test@unittests.com";

        [TestMethod]
        public void TestIndexReturnsView()
        {
            var repository = new Mock<IProjectServiceRepository>();
            var controller = ProjectServicesController.CreateProjectServiceControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectClientsOneClient()
        {
            var repository = new Mock<IProjectServiceRepository>();

            ProjectService projectService = new ProjectService();
            projectService.ProjectId = 1;
            projectService.Service = new Service()
            {
                Price = 20,
                ServiceiD = 1,
                ServiceName = "Unit test service name"
            };

            repository.Setup(m => m.GetProjectServicesForUserName(TEST_USER_NAME)).Returns(() => new List<ProjectService> { projectService });

            var controller = ProjectServicesController.CreateProjectServiceControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            List<ProjectServiceViewModel> results = result.Model as List<ProjectServiceViewModel>;

            ProjectServiceViewModel psvm = new ProjectServiceViewModel();
            psvm.ProjectId = 1;
            psvm.Service = new Service()
            {
                Price = 20,
                ServiceiD = 1,
                ServiceName = "Unit test service name"
            };


            Assert.AreEqual(psvm.ProjectId, results[0].ProjectId);
            Assert.AreEqual(psvm.Service.Price, results[0].Service.Price);
            Assert.AreEqual(psvm.Service.ServiceiD, results[0].Service.ServiceiD);
            Assert.AreEqual(psvm.Service.ServiceName, results[0].Service.ServiceName);
        }
    }
}
