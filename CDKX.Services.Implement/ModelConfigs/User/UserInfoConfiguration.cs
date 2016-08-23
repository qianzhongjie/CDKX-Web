using OSharp.Data.Entity;
using CDKX.Services.Core.Models.User;


namespace CDKX.Services.Implement.ModelConfigs.User
{
    /// <summary>
    /// 实体映射类——用户信息映射信息
    /// </summary>
    public class UserInfoConfiguration : EntityConfigurationBase<UserInfo, int>
    {
        public UserInfoConfiguration()
        {
            //HasRequired(m => m.SysUser).WithOptional().WillCascadeOnDelete(true);
        }
    }
}