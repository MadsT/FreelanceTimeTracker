using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using FreelanceTimeTracker.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using FreelanceTimeTracker.Models;

namespace FreelanceTimeTracker.Tests.Controllers
{
    [TestClass]
    public class ProjectsControllerTest
    {
        private static readonly string TEST_USER_NAME = "test@unittests.com";

        [TestMethod]
        public void TestIndexReturnsView()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectProjectsOneProject()
        {
            var repository = new Mock<IProjectsRepository>();

            Project project = new Project();
            project.ProjectName = "UnitTest ProjectName";
            repository.Setup(m => m.GetProjectsForUserName(TEST_USER_NAME)).Returns(() => new List<Project> { project });

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            List<Project> resultProjects = result.Model as List<Project>;
            Assert.AreEqual(project, resultProjects[0]);
        }

        [TestMethod]
        public void TestIndexViewFindsCorrectProjectsMultipleProjects()
        {
            var repository = new Mock<IProjectsRepository>();

            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";

            Project project2 = new Project();
            project2.ProjectName = "Another UnitTest ProjectName";

            repository.Setup(m => m.GetProjectsForUserName(TEST_USER_NAME)).Returns(() => new List<Project> { project1, project2 });

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);


            ViewResult result = controller.Index() as ViewResult;
            List<Project> resultProjects = result.Model as List<Project>;
            CollectionAssert.AreEquivalent(new List<Project> { project1, project2 }, resultProjects);
        }


        [TestMethod]
        public void TestIndexViewNoProjectsForUserName()
        {
            var repository = new Mock<IProjectsRepository>();

            repository.Setup(m => m.GetProjectsForUserName(TEST_USER_NAME)).Returns(() => null);

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNull(result.Model);
        }

        [TestMethod]
        public void TestDetailsCanFindProject()
        {
            var repository = new Mock<IProjectsRepository>();

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ProjectId) as ViewResult;

            Project resultProject = result.Model as Project;

            Assert.AreEqual(project1, resultProject);
        }

        [TestMethod]
        public void TestDetailsCanFindProjectButDifferentUser()
        {
            var repository = new Mock<IProjectsRepository>();

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = "Not "+TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDetailsNoProjectWithThatId()
        {
            var repository = new Mock<IProjectsRepository>();

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => null);

            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            ViewResult result = controller.Details(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAddProject()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            Project project = new Project();
            project.ProjectName = "UnitTest ProjectName";

            controller.Create(project);

            repository.Verify(r => r.AddProject(It.IsAny<Project>()), Times.Once);
        }

        [TestMethod]
        public void TestEditProjectGETCanFindId()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);
            ViewResult result = controller.Edit(ProjectId) as ViewResult;

            Assert.AreEqual(project1, result.Model);
        }

        [TestMethod]
        public void TestEditProjectGETCantFindId()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => null);
            ViewResult result = controller.Edit(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditProjectGETCanFindIdOtherProjectOwner()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = "Not "+TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);
            ViewResult result = controller.Edit(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEditProjectPOST()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            RedirectToRouteResult result = controller.Edit(project1) as RedirectToRouteResult;

            repository.Verify(r => r.UpdateProject(It.IsAny<Project>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestEditProjectPOSTOtherProjectOwner()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = "Not "+TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;
            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            ViewResult resultView = controller.Edit(project1) as ViewResult;

            Assert.AreEqual(project1, resultView.Model);
        }

        [TestMethod]
        public void TestDeleteGETCanFind()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);
            ViewResult result = controller.Delete(ProjectId) as ViewResult;

            Assert.AreEqual(project1, result.Model);
        }

        [TestMethod]
        public void TestDeleteProjectGETCantFindId()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => null);
            ViewResult result = controller.Delete(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteProjectGETCanFindIdOtherProjectOwner()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = "Not "+TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);
            ViewResult result = controller.Delete(ProjectId) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestDeleteProjectPOST()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            RedirectToRouteResult result = controller.DeleteConfirmed(ProjectId) as RedirectToRouteResult;

            repository.Verify(r => r.DeleteProject(It.IsAny<Project>()), Times.Once);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestDeleteProjectPOSTOtherProjectOwner()
        {
            var repository = new Mock<IProjectsRepository>();
            var controller = ProjectsController.CreateProjectsControllerAs(TEST_USER_NAME, repository.Object);

            int ProjectId = 1;
            Project project1 = new Project();
            project1.ProjectName = "UnitTest ProjectName";
            project1.Client = new Client()
            {
                ClientOwner = "Not "+TEST_USER_NAME
            };
            project1.ProjectID = ProjectId;

            repository.Setup(m => m.GetProjectById(ProjectId)).Returns(() => project1);

            RedirectToRouteResult result = controller.DeleteConfirmed(ProjectId) as RedirectToRouteResult;
            repository.Verify(r => r.DeleteProject(It.IsAny<Project>()), Times.Never);

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}