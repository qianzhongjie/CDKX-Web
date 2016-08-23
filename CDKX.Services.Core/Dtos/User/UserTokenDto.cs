using CDKX.Services.Core.Models.User;

namespace CDKX.Services.Core.Dtos.User
{
    public class UserTokenDto
    {
        public int Id { get; set; }
        public string NickName { get; set; }

        public string HeadPic { get; set; }

        public string Token { get; set; }

        public Sex Sex { get; set; }
        public string PhoneNo { get; set; }
    }
}
