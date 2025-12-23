using System;
using Npgsql;
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class UserDal
    {
        // GÜNCELLENDİ: Username parametresi ve Trigger hata yakalama eklendi
        public void RegisterUser(User user, string password)
        {
            using (var conn = DbHelper.GetConnection())
            {
                // SQL sorgusuna 'username' sütununu ekledik
                string sql = "INSERT INTO users (username, full_name, email, password_hash, role, profile_image) " +
                             "VALUES (@username, @name, @email, @pass, 'STUDENT', 'default_user.png')";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", user.Username); // YENİ
                    cmd.Parameters.AddWithValue("@name", user.FullName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@pass", password);

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (PostgresException ex)
                    {
                        // SQL'deki Trigger'dan gelen hata mesajını (RAISE EXCEPTION) yakalayıp fırlatıyoruz
                        throw new Exception(ex.MessageText);
                    }
                }
            }
        }
        // --- PROFİL GÜNCELLEME METOTLARI ---

public void UpdateUserProfile(User user)
{
    using (var conn = DbHelper.GetConnection())
    {
        string sql = "UPDATE users SET full_name = @name, email = @email, username = @username WHERE user_id = @id";
        using (var cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@name", user.FullName);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@id", user.UserId);

            try {
                cmd.ExecuteNonQuery();
            }
            catch (PostgresException ex) {
                // E-posta trigger kontrolü burada da çalışır!
                throw new Exception(ex.MessageText);
            }
        }
    }
}

public void UpdatePassword(int userId, string newPassword)
{
    using (var conn = DbHelper.GetConnection())
    {
        string sql = "UPDATE users SET password_hash = @pass WHERE user_id = @id";
        using (var cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@pass", newPassword);
            cmd.Parameters.AddWithValue("@id", userId);
            cmd.ExecuteNonQuery();
        }
    }
}

        // GÜNCELLENDİ: Artık Giriş Kontrolü Username üzerinden yapılıyor
        public User? Login(string username, string password)
        {
            User? user = null;
            using (var conn = DbHelper.GetConnection())
            {
                // Email parametresi yerine @username kullandık
                string sql = "SELECT user_id, username, full_name, email, role::text FROM users WHERE username = @username AND password_hash = @pass";
                
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@pass", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = Convert.ToInt32(reader["user_id"]),
                                Username = reader["username"].ToString()!, // YENİ
                                FullName = reader["full_name"].ToString()!,
                                Email = reader["email"].ToString()!,
                                Role = reader["role"].ToString()! 
                            };
                        }
                    }
                }
            }
            return user;
        }
    }
}