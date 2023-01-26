using Dapper.QX;
using Dapper.QX.Attributes;
using UserVoice.Database;

namespace UserVoice.Service.Queries
{
    public class ListUsers : Query<User>
    {
        public ListUsers() : base("SELECT * FROM [uservoice].[User] {where} ORDER BY [Name]")
        {
        }

        [Where("(([Role] & @role) = @role)")]
        public Role? Role { get; set; }
    }
}
