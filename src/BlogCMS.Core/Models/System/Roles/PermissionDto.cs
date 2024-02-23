namespace BlogCMS.Core.Models.System.Roles
{
    public class PermissionDto
    {
        public string RoleId { get; set; }

        public IList<RoleClaimsDto> RoleClaims { get; set; }
    }
}
