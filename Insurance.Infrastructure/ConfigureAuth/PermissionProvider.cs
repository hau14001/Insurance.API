using System.Collections.Generic;

namespace Insurance.Infrastructure.ConfigureAuth
{
    public class PermissionProvider : IPermissionProvider
    {
        public IReadOnlyList<string> GetAll()
        {
            return new List<string>
            {
                //DeleteUser,
                //CreateUser,
                //ReadUser,
                //UpdateUser,
                //ManageUser,
                //ManageRole,
                //UpdateRole,
                //CreateRole,
                //ManageCategory,
                //ManageCommand,
                //ManageCourseChapter,
                //ManageCourse,
                //ManageCourseLesson,
                //DeleteCourse,
                //ManagePost,
                //ManageRevitToolVersion,
                //ManageToolProduct,
                //ManageTrainer,
                //ManageUserToolLicense,
                //Dashboard,
            };
        }
    }

    public interface IPermissionProvider
    {
        IReadOnlyList<string> GetAll();
    }
}