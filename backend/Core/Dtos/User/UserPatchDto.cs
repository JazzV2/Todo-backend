namespace backend.Core.Dtos.User
{
    public class UserPatchDto
    {
        public string NewUsername { get; set; } = String.Empty;
        public string NewPassword { get; set; } = String.Empty;
        public string NewEmail { get; set; } = String.Empty;
        public string OldPassword { get; set; }
    }
}
