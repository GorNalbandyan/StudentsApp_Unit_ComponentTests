using NUnit.Framework;
using StudyGroupFeature;

namespace StudentsApp.Tests.UnitTests
{
    [TestFixture]
    public class StudyGroupUnitTests
    {
        private StudyGroup _studyGroup;

        [SetUp]
        public void Setup()
        {
            _studyGroup = new StudyGroup(1, "Math Study", Subject.Math, DateTime.Now);
        }

        [Test]
        public void StudyGroup_Name_IsMandatory()
        {
            // Arrange
            var subject = Subject.Math;
            var createDate = DateTime.Now;

            Assert.Throws<ArgumentNullException>(() => new StudyGroup(1, null, subject, createDate), "Group is created with missing StudyGroup");
        }

        [Test]
        public void StudyGroup_CreationDate_IsRecordedCorrectly()
        {
            // Arrange
            var studyGroupId = 1;
            var name = "Test Study Group";
            var subject = Subject.Math;
            var createDate = DateTime.Now;

            // Act
            var studyGroup = new StudyGroup(studyGroupId, name, subject, createDate);

            // Assert
            Assert.AreEqual(createDate, studyGroup.CreateDate);
        }

        [Test]
        public void Subjects_AreSpecified()
        {
            // Arrange
            var specifiedSubjects = new HashSet<Subject> { Subject.Math, Subject.Chemistry, Subject.Physics };

            // Act
            var subjectsInEnum = Enum.GetValues(typeof(Subject));

            // Assert
            CollectionAssert.AreEquivalent(specifiedSubjects, subjectsInEnum);
        }


        // Positive test cases
        [Test]
        public void CreateStudyGroup_ValidData()
        {
            // Arrange & Act
            var studyGroup = new StudyGroup(1, "Math Study", Subject.Math, DateTime.Now);

            // Assert
            Assert.IsNotNull(studyGroup);
        }

        [Test]
        public void AddUser_ValidUser()
        {
            // Arrange
            var user = new User(123, "John", "Smith", "john@gmail.com");

            // Act
            _studyGroup.AddUser(user);

            // Assert
            Assert.IsTrue(_studyGroup.Users.Contains(user));
        }

        [Test]
        public void RemoveUser_ExistingUser()
        {
            // Arrange
            var user = new User(123, "John", "Smith", "john@gmail.com");
            _studyGroup.AddUser(user);

            // Act
            _studyGroup.RemoveUser(user);

            // Assert
            Assert.IsFalse(_studyGroup.Users.Contains(user));
        }

        // Negative test cases

        [Test]
        public void CreateStudyGroup_InvalidSubject_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new StudyGroup(1, "Math Study", (Subject)100, DateTime.Now));
        }

        [Test]
        public void AddUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            User user = null;

            // Act & Assert
            Assert.Throws<System.NullReferenceException>(() => _studyGroup.AddUser(user));
        }

        [Test]
        public void RemoveUser_NonExistingUser_ThrowsArgumentException()
        {
            // Arrange
            var user = new User(123, "John", "Smith", "john@gmail.com");

            // Act & Assert
            Assert.Throws<System.NullReferenceException>(() => _studyGroup.RemoveUser(user));
        }
    }
}