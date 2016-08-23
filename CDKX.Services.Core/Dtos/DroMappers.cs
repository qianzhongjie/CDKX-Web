using AutoMapper;
using CDKX.Services.Core.Models.Identity;
using OSharp.Core.Security;
using CDKX.Services.Core.Dtos.Identity;
using CDKX.Services.Core.Dtos.Security;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.User;

namespace CDKX.Services.Core.Dtos
{
    public partial class DtoMappers
    {
        static partial void MapperRegisterCustom()
        {
            //Identity
            Mapper.CreateMap<SysOrganizationDto, SysOrganization>();
            Mapper.CreateMap<SysUserDto, SysUser>();
            Mapper.CreateMap<SysRoleDto, SysRole>();

            //Security
            Mapper.CreateMap<FunctionDto, Function>();
            Mapper.CreateMap<EntityInfoDto, EntityInfo>();

            //UserInfo
            Mapper.CreateMap<UserInfoRegistDto, UserInfo>();
            Mapper.CreateMap<UserInfoEditDto, UserInfo>();
            
          
        }
    }
}
