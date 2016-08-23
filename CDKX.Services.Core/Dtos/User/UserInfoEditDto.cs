using System;
using CDKX.Services.Core.Models.User;
using OSharp.Core.Data;

namespace CDKX.Services.Core.Dtos.User
{
    public class UserInfoEditDto : IEditDto<int>
    {
        public int Id { get; set; }

        public string HeadPic { get; set; }

        public Sex Sex { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public string PhoneNumber { get; set; }

        public string Birthday { get; set; }

        public string Qq { get; set; }
    }
}
