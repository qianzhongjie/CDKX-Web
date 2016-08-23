using OSharp.Data.Entity;
using CDKX.Services.Core.Models.User;


namespace CDKX.Services.Implement.ModelConfigs.User
{
    /// <summary>
    /// 实体映射类——功能用户映射信息
    /// </summary>
    public class FeedBackConfiguration : EntityConfigurationBase<FeedBack, int>
    {
        public FeedBackConfiguration()
        {
            HasRequired(p => p.UserInfo);
        }
    }
}