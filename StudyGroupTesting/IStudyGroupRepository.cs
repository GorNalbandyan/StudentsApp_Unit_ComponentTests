using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyGroupFeature
{
    public interface IStudyGroupRepository
    {
        Task<IActionResult> CreateStudyGroup(StudyGroup studyGroup);

        Task<IActionResult> GetStudyGroups();
        Task<IActionResult> LeaveStudyGroup(int studyGroupId, int userId);
        Task<IActionResult> JoinStudyGroup(int studyGroupId, int userId);
        Task<IActionResult> SearchStudyGroups(Subject subject);
        Task<StudyGroup> GetStudyGroup(int studyGroupId);
        Task<StudyGroup> GetStudyGroupById(int studyGroupId);
        Task<bool> UpdateStudyGroup(StudyGroup studyGroup);
    }
}
