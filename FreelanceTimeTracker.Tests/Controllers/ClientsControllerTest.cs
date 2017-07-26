using FreelanceTimeTracker.Controllers;
using FreelanceTimeTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace FreelanceTimeTracker.Tests.Controllers
{
    [TestClass]
    public class ClientsControllerTest
    {
        private static readonly string TEST_USER_NAME = "test@unittests.com";

        [TestMethod]
        public void TestIndexReturnsView()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);
            
            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectClientsOneClient()
        {
            var repository = new Mock<IClientsRepository>();

            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            repository.Setup(m => m.GetClientsForUserName(TEST_USER_NAME)).Returns(() => new List<Client> { client });

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            List<Client> resultClients = result.Model as List<Client>;
            Assert.AreEqual(client, resultClients[0]);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectClientsMultipleClients()
        {
            var repository = new Mock<IClientsRepository>();

            Client client1 = new Client();
            client1.Address = "UnitTest Address";
            client1.ClientName = "UnitTest ClientName";

            Client client2 = new Client();
            client2.Address = "UnitTest Another Address";
            client2.ClientName = "UnitTest Another ClientName";

            repository.Setup(m => m.GetClientsForUserName(TEST_USER_NAME)).Returns(() => new List<Client> { client1, client2 });

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            
            ViewResult result = controller.Index() as ViewResult;
            List<Client> resultClients = result.Model as List<Client>;
            CollectionAssert.AreEquivalent(new List<Client> { client1, client2 }, resultClients);
        }


        [TestMethod]
        public void TestIndexViewNoClientsForUserName()
        {
            var repository = new Mock<IClientsRepository>();

            repository.Setup(m => m.GetClientsForUserName(TEST_USER_NAME)).Returns(() => null);

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void TestDetailsCanFindClient()
        {
            var repository = new Mock<IClientsRepository>();

            int clientId = 1;
            Client client1 = new Client();
            client1.Address = "UnitTest Address";
            client1.ClientName = "UnitTest ClientName";
            client1.ClientOwner = TEST_USER_NAME;
            client1.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client1);

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(clientId) as ViewResult;

            Client resultClient = result.Model as Client;

            Assert.AreEqual(client1, resultClient);
        }

        [TestMethod]
        public void TestDetailsCanFindClientButDifferentUser()
        {
            var repository = new Mock<IClientsRepository>();

            int clientId = 1;
            Client client1 = new Client();
            client1.Address = "UnitTest Address";
            client1.ClientName = "UnitTest ClientName";
            client1.ClientOwner = "Not "+TEST_USER_NAME;
            client1.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client1);

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(clientId) as ViewResult;  

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDetailsNoClientWithThatId()
        {
            var repository = new Mock<IClientsRepository>();

            int clientId = 1;
            Client client1 = new Client();
            client1.Address = "UnitTest Address";
            client1.ClientName = "UnitTest ClientName";
            client1.ClientOwner = TEST_USER_NAME;
            client1.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => null);

            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(clientId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAddClient()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";

            controller.Create(client);

            repository.Verify(r => r.AddClient(It.IsAny<Client>()), Times.Once);
        }

        [TestMethod]
        public void TestEditClientGETCanFindId()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);
            ViewResult result = controller.Edit(clientId) as ViewResult;

            Assert.AreEqual(client, result.Model);
        }

        [TestMethod]
        public void TestEditClientGETCantFindId()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => null);
            ViewResult result = controller.Edit(clientId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditClientGETCanFindIdOtherClientOwner()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = "Not "+TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);
            ViewResult result = controller.Edit(clientId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditClientPOST()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = TEST_USER_NAME;
            client.ClientID = clientId;

           RedirectToRouteResult result =  controller.Edit(client) as RedirectToRouteResult;

            repository.Verify(r => r.UpdateClient(It.IsAny<Client>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestEditClientPOSTOtherClientOwner()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = "Not "+TEST_USER_NAME;
            client.ClientID = clientId;

            ViewResult resultView = controller.Edit(client) as ViewResult;

            Assert.AreEqual(client, resultView.Model);
        }

        [TestMethod]
        public void TestDeleteGETCanFind()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);
            ViewResult result = controller.Delete(clientId) as ViewResult;

            Assert.AreEqual(client, result.Model);
        }

        [TestMethod]
        public void TestDeleteClientGETCantFindId()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => null);
            ViewResult result = controller.Delete(clientId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteClientGETCanFindIdOtherClientOwner()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = "Not " + TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);
            ViewResult result = controller.Delete(clientId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteClientPOST()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);

            RedirectToRouteResult result = controller.DeleteConfirmed(clientId) as RedirectToRouteResult;

            repository.Verify(r => r.DeleteClient(It.IsAny<Client>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestDeleteClientPOSTOtherClientOwner()
        {
            var repository = new Mock<IClientsRepository>();
            var controller = ClientsController.CreateClientsControllerAs(TEST_USER_NAME, repository.Object);

            int clientId = 1;
            Client client = new Client();
            client.Address = "UnitTest Address";
            client.ClientName = "UnitTest ClientName";
            client.ClientOwner = "Not " + TEST_USER_NAME;
            client.ClientID = clientId;

            repository.Setup(m => m.GetClientById(clientId)).Returns(() => client);

            RedirectToRouteResult result = controller.DeleteConfirmed(clientId) as RedirectToRouteResult;
            repository.Verify(r => r.DeleteClient(It.IsAny<Client>()), Times.Never);

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
