using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mojo.AuthManager
{
    public class IndetityRepository
    {

        private const string connectionString = "Server=127.0.0.1;User ID=mojo_user;Password=P@ssw0rd@1234;Database=mojo_identity";
        public static async Task<IdentityModel?> ValidateIdentity(string userName, string password)
        {
            IdentityModel identityModel = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sql = string.Format("SELECT * FROM user_credential WHERE UserName = '{0}' AND Password = '{1}';", userName, password);
                using var command = new MySqlCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        identityModel = new IdentityModel();
                        while (reader.Read())
                        {
                            identityModel.UserName = reader["UserName"].ToString();
                            identityModel.RegistrationId = Convert.ToInt32(reader["RegistrationId"].ToString());
                        }
                    }
                }
            }
            return identityModel;
        }
    }
}
