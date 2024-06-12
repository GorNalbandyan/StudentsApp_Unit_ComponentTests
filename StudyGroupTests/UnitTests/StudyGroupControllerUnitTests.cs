using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StudyGroupFeature;

namespace StudentsApp.Tests.UnitTests
{

    [TestFixture]
    public class StudyGroupRepositoryTests
    {
        [TestCase(Subject.Math)]
        [TestCase(Subject.Chemistry)]
        [TestCase(Subject.Physics)]
        public async Task CreateStudyGroup_Success(Subject subject)
        {
            // Arrange
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            var studyGroup = new StudyGroup(1, $"Test Study Group for {subject}", subject, DateTime.Now);

            // Act & Assert
            Assert.That(async () => await studyGroupRepositoryMock.Object.CreateStudyGroup(studyGroup), Throws.Nothing);
        }

        [Test]
        public async Task GetStudyGroups_ReturnsOkObjectResult()
        {
            // Arrange
            var studyGroups = new List<StudyGroup>(); // Mock list of study groups
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroups()).ReturnsAsync(studyGroups);
            var studyGroupController = new StudyGroupController(studyGroupRepositoryMock.Object);

            // Act
            var result = await studyGroupController.GetStudyGroups();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result); // Ensure OkObjectResult is returned
        }

        [Test]
        public async Task SearchStudyGroups_ReturnsOkObjectResult()
        {
            // Arrange
            var subject = "Math"; // Mock subject
            var expectedStudyGroups = new List<StudyGroup>(); // Mock list of expected study groups
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.SearchStudyGroups(subject)).ReturnsAsync(expectedStudyGroups);
            var studyGroupController = new StudyGroupController(studyGroupRepositoryMock.Object);

            // Act
            var result = await studyGroupController.SearchStudyGroups(subject);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.IsNotNull((result as OkObjectResult)?.Value);
        }

        [Test]
        public async Task JoinStudyGroup_ReturnsOkResult()
        {
            // Arrange
            var studyGroupId = 1;
            var userId = 1;
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            var studyGroupController = new StudyGroupController(studyGroupRepositoryMock.Object);

            // Act
            var result = await studyGroupController.JoinStudyGroup(studyGroupId, userId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            studyGroupRepositoryMock.Verify(repo => repo.JoinStudyGroup(studyGroupId, userId), Times.Once); // Ensure JoinStudyGroup method is called with the correct parameters
        }

        [Test]
        public async Task LeaveStudyGroup_ReturnsOkResult()
        {
            // Arrange
            var studyGroupId = 1;
            var userId = 1;
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            var studyGroupController = new StudyGroupController(studyGroupRepositoryMock.Object);

            // Act
            var result = await studyGroupController.LeaveStudyGroup(studyGroupId, userId);

            // Assert
            Assert.IsInstanceOf<OkResult>(result); 
            studyGroupRepositoryMock.Verify(repo => repo.LeaveStudyGroup(studyGroupId, userId), Times.Once); // Ensure LeaveStudyGroup method is called with the correct parameters
        }
    }
}