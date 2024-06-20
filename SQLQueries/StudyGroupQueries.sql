SELECT sg.*
FROM StudyGroups sg
JOIN StudyGroupUsers sgu ON sg.StudyGroupId = sgu.StudyGroupId
JOIN Users u ON sgu.UserId = u.UserId
WHERE u.Name LIKE 'M%'
ORDER BY sg.CreateDate;

--StudyGroupUsers (sgu): This table acts as the junction table that connects StudyGroups and Users. 
--Assumingly, it  contains two columns: StudyGroupId and UserId establishing the many-to-many relationship.