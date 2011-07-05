using System.ComponentModel.DataAnnotations;
using System.Linq;
using CodeGarten.Data.Access;

namespace CodeGarten.Data.CustomAttributes
{
    internal sealed class UniqueEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            using (var connection = new DataBaseManager())
            {
                var email = value as string;
                return connection.DbContext.Users.Count(u => u.Email == email) == 0;
            }
        }
    }
}