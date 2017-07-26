using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FreelanceTimeTracker.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using FreelanceTimeTracker.Models;
using static FreelanceTimeTracker.Controllers.ServicesController;

namespace FreelanceTimeTracker.Tests.Controllers
{
    [TestClass]
    public class ServicesControllerTest
    {
        private static readonly string TEST_USER_NAME = "test@unittests.com";

        [TestMethod]
        public void TestIndexReturnsView()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectServicesOneService()
        {
            var repository = new Mock<IServicesRepository>();

            Service service = new Service();
            service.ServiceName = "UnitTest ServiceName";
            repository.Setup(m => m.GetServicesForUserName(TEST_USER_NAME)).Returns(() => new List<Service> { service });

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            List<Service> resultServices = result.Model as List<Service>;
            Assert.AreEqual(service, resultServices[0]);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectServicesMultipleServices()
        {
            var repository = new Mock<IServicesRepository>();

            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";

            Service service2 = new Service();
            service2.ServiceName = "Another UnitTest ServiceName";

            repository.Setup(m => m.GetServicesForUserName(TEST_USER_NAME)).Returns(() => new List<Service> { service1, service2 });

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);


            ViewResult result = controller.Index() as ViewResult;
            List<Service> resultServices = result.Model as List<Service>;
            CollectionAssert.AreEquivalent(new List<Service> { service1, service2 }, resultServices);
        }


        [TestMethod]
        public void TestIndexViewNoServicesForUserName()
        {
            var repository = new Mock<IServicesRepository>();

            repository.Setup(m => m.GetServicesForUserName(TEST_USER_NAME)).Returns(() => null);

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void TestDetailsCanFindService()
        {
            var repository = new Mock<IServicesRepository>();

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ServiceId) as ViewResult;

            Service resultService = result.Model as Service;

            Assert.AreEqual(service1, resultService);
        }

        [TestMethod]
        public void TestDetailsCanFindServiceButDifferentUser()
        {
            var repository = new Mock<IServicesRepository>();

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = "Not " + TEST_USER_NAME;

            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDetailsNoServiceWithThatId()
        {
            var repository = new Mock<IServicesRepository>();

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => null);

            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAddService()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            Service service = new Service();
            service.ServiceName = "UnitTest ServiceName";

            controller.Create(service);

            repository.Verify(r => r.AddService(It.IsAny<Service>()), Times.Once);
        }

        [TestMethod]
        public void TestEditServiceGETCanFindId()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);
            ViewResult result = controller.Edit(ServiceId) as ViewResult;

            Assert.AreEqual(service1, result.Model);
        }

        [TestMethod]
        public void TestEditServiceGETCantFindId()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => null);
            ViewResult result = controller.Edit(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditServiceGETCanFindIdOtherServiceOwner()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = "Not " + TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);
            ViewResult result = controller.Edit(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditServicePOST()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            RedirectToRouteResult result = controller.Edit(service1) as RedirectToRouteResult;

            repository.Verify(r => r.UpdateService(It.IsAny<Service>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestEditServicePOSTOtherServiceOwner()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = "Not " + TEST_USER_NAME;
            service1.ServiceiD = ServiceId;
            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            ViewResult resultView = controller.Edit(service1) as ViewResult;

            Assert.AreEqual(service1, resultView.Model);
        }

        [TestMethod]
        public void TestDeleteGETCanFind()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);
            ViewResult result = controller.Delete(ServiceId) as ViewResult;

            Assert.AreEqual(service1, result.Model);
        }

        [TestMethod]
        public void TestDeleteServiceGETCantFindId()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => null);
            ViewResult result = controller.Delete(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteServiceGETCanFindIdOtherServiceOwner()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = "Not " + TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);
            ViewResult result = controller.Delete(ServiceId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteServicePOST()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            RedirectToRouteResult result = controller.DeleteConfirmed(ServiceId) as RedirectToRouteResult;

            repository.Verify(r => r.DeleteService(It.IsAny<Service>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestDeleteServicePOSTOtherServiceOwner()
        {
            var repository = new Mock<IServicesRepository>();
            var controller = ServicesController.CreateServicesControllerAs(TEST_USER_NAME, repository.Object);

            int ServiceId = 1;
            Service service1 = new Service();
            service1.ServiceName = "UnitTest ServiceName";
            service1.ServiceOwner = "Not " + TEST_USER_NAME;
            service1.ServiceiD = ServiceId;

            repository.Setup(m => m.GetServiceById(ServiceId)).Returns(() => service1);

            RedirectToRouteResult result = controller.DeleteConfirmed(ServiceId) as RedirectToRouteResult;
            repository.Verify(r => r.DeleteService(It.IsAny<Service>()), Times.Never);

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}