using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StudyGroupFeature;

namespace StudentsApp.Tests.IntegrationTests
{
    [TestFixture]
    public class StudyGroupComponentTests
    {
        private StudyGroupController _studyGroupController;
        private Mock<IStudyGroupRepository> _studyGroupRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            _studyGroupController = new StudyGroupController(_studyGroupRepositoryMock.Object);
        }

        #region Positive Cases
        [Test]
        public async Task CreateStudyGroup()
        {
            // Arrange
            var studyGroup = new StudyGroup(1, "Test Study Group", Subject.Math, DateTime.Now);

            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock
                .Setup(repo => repo.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .ReturnsAsync(studyGroup);

            var _studyGroupController = new StudyGroupController(studyGroupRepositoryMock.Object);

            // Act
            var result = await _studyGroupController.CreateStudyGroup(studyGroup);


            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "OkObjectResult is null.");
            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200 but got a different code.");

            var returnedStudyGroup = okResult.Value as StudyGroup;
            Assert.Multiple(() =>
            {

                Assert.IsNotNull(returnedStudyGroup, "Returned study group is null.");
                Assert.AreEqual(studyGroup.StudyGroupId, returnedStudyGroup.StudyGroupId, "StudyGroupId does not match.");
                Assert.AreEqual(studyGroup.Name, returnedStudyGroup.Name, "Name does not match.");
                Assert.AreEqual(studyGroup.Subject, returnedStudyGroup.Subject, "Subject does not match.");
                Assert.AreEqual(studyGroup.CreateDate, returnedStudyGroup.CreateDate, "CreateDate does not match.");
            });
        }

        [Test]
        public async Task GetStudyGroups()
        {
            // Arrange
            var studyGroups = new List<StudyGroup>
            {
                new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now),
                new StudyGroup(2, "Chemistry Study Group", Subject.Chemistry, DateTime.Now.AddMinutes(-10))
            };

            _studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroups()).ReturnsAsync(studyGroups);

            // Act
            var result = await _studyGroupController.GetStudyGroups();

            // Assert
            Assert.Multiple(() =>
            {
                var okResult = result as OkObjectResult;
                Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200 but got a different code.");

                var returnedStudyGroups = okResult.Value as List<StudyGroup>;
                Assert.IsNotNull(returnedStudyGroups, "Returned study groups list is null.");
                Assert.AreEqual(studyGroups.Count, returnedStudyGroups.Count, "Returned study groups count does not match.");

                for (int i = 0; i < studyGroups.Count; i++)
                {
                    Assert.AreEqual(studyGroups[i].StudyGroupId, returnedStudyGroups[i].StudyGroupId, $"StudyGroupId does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].Name, returnedStudyGroups[i].Name, $"Name does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].Subject, returnedStudyGroups[i].Subject, $"Subject does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].CreateDate, returnedStudyGroups[i].CreateDate, $"CreateDate does not match for study group at index {i}.");
                }
            });
        }

        [TestCase(Subject.Chemistry)]
        [TestCase(Subject.Math)]
        [TestCase(Subject.Physics)]
        public async Task SearchStudyGroups_ValidSubject(Subject subject)
        {
            // Arrange
            var studyGroups = new List<StudyGroup>
            {
                new StudyGroup(1, $"{subject} Study Group 1",  subject, DateTime.Now),
                new StudyGroup(2, $"{subject} Study Group 2",  subject, DateTime.Now.AddMinutes(-10))
            };

            _studyGroupRepositoryMock.Setup(repo => repo.SearchStudyGroups(subject)).ReturnsAsync(studyGroups);

            var _studyGroupController = new StudyGroupController(_studyGroupRepositoryMock.Object);

            // Act
            var result = await _studyGroupController.SearchStudyGroups(subject);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsInstanceOf<OkObjectResult>(result, "Expected OkObjectResult but got a different result type.");

                var okResult = result as OkObjectResult;
                Assert.IsNotNull(okResult, "OkObjectResult is null.");
                Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200 but got a different code.");

                var returnedStudyGroups = okResult.Value as List<StudyGroup>;
                Assert.IsNotNull(returnedStudyGroups, "Returned study groups list is null.");
                Assert.AreEqual(studyGroups.Count, returnedStudyGroups.Count, "Returned study groups count does not match.");

                for (int i = 0; i < studyGroups.Count; i++)
                {
                    Assert.AreEqual(studyGroups[i].StudyGroupId, returnedStudyGroups[i].StudyGroupId, $"StudyGroupId does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].Name, returnedStudyGroups[i].Name, $"Name does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].Subject, returnedStudyGroups[i].Subject, $"Subject does not match for study group at index {i}.");
                    Assert.AreEqual(studyGroups[i].CreateDate, returnedStudyGroups[i].CreateDate, $"CreateDate does not match for study group at index {i}.");
                }
            });
        }

        [Test]
        public void User_Leave_StudyGroup()
        {
            // Arrange
            var userId = 1;
            var studyGroupId = 1;
            var user = new User(userId, "John Doe");
            var studyGroup = new StudyGroup(studyGroupId, "Math Study Group", Subject.Math, DateTime.Now);
            studyGroup.AddUser(user); 

            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroup(studyGroupId)).ReturnsAsync(studyGroup);

            // Act
            studyGroup.RemoveUser(user); 

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsEmpty(studyGroup.Users, "Ensure user is removed from the study group");
                Assert.IsTrue(studyGroup.Users.All(u => u.Id != userId), "Ensure user is removed from the study group");
            });
        }

        [Test]
        public async Task SimultaneousCreation_SameSubject()
        {
            // Arrange
            var subject = Subject.Math;
            var studyGroupRepository = new StudyGroupRepository();
            var studyGroupService = new StudyGroupService(studyGroupRepository); // Assuming this is the service implementation

            // Act
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++) // Simulate simultaneous creation by starting multiple tasks
            {
                tasks.Add(Task.Run(async () =>
                {
                    await studyGroupService.CreateStudyGroup(new StudyGroup(0, "Study Group " + Guid.NewGuid().ToString().Substring(0, 8), subject, DateTime.Now));
                }));
            }
            await Task.WhenAll(tasks);

            // Assert
            var studyGroups = await studyGroupRepository.GetStudyGroups();
            var createdStudyGroups = studyGroups.Where(sg => sg.Subject == subject);
            Assert.AreEqual(1, createdStudyGroups.Count(), "More than one study group is created for the same subject");
        }
        #endregion

        #region Negative Cases
        [Test]
        public void CreateStudyGroup_ExistingSubject()
        {
            // Arrange
            var existingStudyGroup = new StudyGroup(1, "Existing Study Group", Subject.Math, DateTime.Now); 
            var newStudyGroup = new StudyGroup(2, "New Study Group", Subject.Math, DateTime.Now); 

            var studyGroupServiceMock = new Mock<IStudyGroupService>();
            studyGroupServiceMock.Setup(service => service.CreateStudyGroup(It.IsAny<StudyGroup>()))
                                 .Returns(new Result { Success = false, Message = "Duplicate subject. Study group with Math subject already exists." });

            // Act
            var result = studyGroupServiceMock.Object.CreateStudyGroup(newStudyGroup);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(result.Success, "Ensure failure result is returned when adding new study group with same subject");
                Assert.AreEqual("Duplicate subject. Study group with Math subject already exists.", result.Message, "Ensure appropriate error message is returned");
            });
        }

        [Test]
        public void User_Leave_StudyGroup_UserNotInGroup()
        {
            // Arrange
            var userId = 1;
            var studyGroupId = 1;
            var user = new User(userId, "John Doe");
            var studyGroup = new StudyGroup(studyGroupId, "Math Study Group", Subject.Math, DateTime.Now);

            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroup(studyGroupId)).ReturnsAsync(studyGroup);

            // Act
            studyGroup.RemoveUser(user); 

            // Assert
            Assert.IsFalse(studyGroup.Users.Any(u => u.Id == userId), "Ensure user is not removed from the study group if not present");
            studyGroupRepositoryMock.Verify(repo => repo.UpdateStudyGroup(It.IsAny<StudyGroup>()), Times.Never, "Ensure study group repository is not updated if user is not in the group");
        }

        [Test]
        public void JoinStudyGroup_UserAlreadyJoined_Failure()
        {
            // Arrange
            var userId = 1;
            var studyGroupId = 1;
            var user = new User(userId, "John Doe");
            var studyGroup = new StudyGroup(studyGroupId, "Test Study Group", Subject.Math, DateTime.Now);
            studyGroup.AddUser(user); // User is already joined
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroupById(studyGroupId)).ReturnsAsync(studyGroup);

            // Act
            var result = studyGroup.JoinUser(user);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("User is already joined to the study group.", result.Message);
        }

        [Test]
        public async Task JoinNonExistingStudyGroup_Failure()
        {
            // Arrange
            var userId = 1;
            var nonExistentStudyGroupId = 100; // Non-existent study group ID
            var user = new User(userId, "John Doe");
            var studyGroupRepositoryMock = new Mock<IStudyGroupRepository>();
            studyGroupRepositoryMock.Setup(repo => repo.GetStudyGroupById(nonExistentStudyGroupId)).ReturnsAsync((StudyGroup)null);

            // Act
            var result = await studyGroupRepositoryMock.Object.JoinStudyGroup(nonExistentStudyGroupId, userId);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Study group does not exist.", result.Message);
        }
    }
    #endregion
}