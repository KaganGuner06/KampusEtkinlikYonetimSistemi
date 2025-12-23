using System;
using System.Collections.Generic;
using System.Data; // DataTable için gerekli
using Npgsql; 
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class ClubDal
    {
        // --- 1. TÜM KULÜPLERİ GETİR ---
        public List<Club> GetAllClubs()
        {
            List<Club> clubs = new List<Club>();

            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT * FROM clubs ORDER BY club_name ASC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            clubs.Add(MapReaderToClub(dr));
                        }
                    }
                }
            }
            return clubs;
        }

        // --- 2. ID'ye GÖRE KULÜP GETİR (Detay Sayfası İçin) ---
        public Club GetClubById(int id)
        {
            Club club = null;
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT * FROM clubs WHERE club_id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            club = MapReaderToClub(dr);
                        }
                    }
                }
            }
            return club;
        }

        // --- 3. KULÜP EKLE ---
        public void AddClub(Club club)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = @"INSERT INTO clubs 
                               (club_name, description, full_description, category_id, manager_user_id, is_active, logo_url, cover_url, instagram_link, linkedin_link, requires_approval) 
                               VALUES 
                               (@name, @desc, @fullDesc, @catId, @managerId, true, @logo, @cover, @insta, @linked, @approval)";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", club.ClubName);
                    cmd.Parameters.AddWithValue("@desc", club.Description);
                    cmd.Parameters.AddWithValue("@fullDesc", club.FullDescription ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@catId", club.CategoryId);
                    cmd.Parameters.AddWithValue("@managerId", club.ManagerUserId);
                    cmd.Parameters.AddWithValue("@logo", club.LogoUrl ?? "default_club.png");
                    cmd.Parameters.AddWithValue("@cover", club.CoverUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@insta", club.InstagramLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@linked", club.LinkedinLink ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@approval", club.RequiresApproval);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- 4. KULÜP SİL ---
        public void DeleteClub(int clubId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                // SQL'de ON DELETE CASCADE tanımlı olduğu için üyeler ve etkinlikler otomatik silinir.
                string sql = "DELETE FROM clubs WHERE club_id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", clubId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- 5. KULÜBE KATIL (Öğrenci İçin) ---
        public void JoinClub(int clubId, int userId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                // Zaten üye mi kontrolü
                string check = "SELECT COUNT(*) FROM club_memberships WHERE club_id=@cid AND user_id=@uid";
                using (var cmdCheck = new NpgsqlCommand(check, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@cid", clubId);
                    cmdCheck.Parameters.AddWithValue("@uid", userId);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        throw new Exception("Zaten bu kulübe üyesiniz.");
                }

                string sql = "INSERT INTO club_memberships (club_id, user_id) VALUES (@cid, @uid)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clubId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- 6. KULÜP ÜYELERİNİ GETİR (Tablo İçin) ---
        public DataTable GetClubMembers(int clubId)
        {
            DataTable dt = new DataTable();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = @"
                    SELECT u.full_name AS Ad_Soyad, u.email AS Email, cm.joined_at AS Katilim_Tarihi
                    FROM club_memberships cm
                    JOIN users u ON cm.user_id = u.user_id
                    WHERE cm.club_id = @cid";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clubId);
                    using (var da = new NpgsqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        // --- 7. KULÜP GÜNCELLE ---
public void UpdateClub(Club club)
{
    using (var conn = DbHelper.GetConnection())
    {
        string sql = @"UPDATE clubs SET 
                        club_name = @name, 
                        description = @desc, 
                        full_description = @fullDesc, 
                        category_id = @catId, 
                        logo_url = @logo, 
                        cover_url = @cover, 
                        instagram_link = @insta, 
                        linkedin_link = @linked
                       WHERE club_id = @id";

        using (var cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@name", club.ClubName);
            cmd.Parameters.AddWithValue("@desc", club.Description);
            cmd.Parameters.AddWithValue("@fullDesc", club.FullDescription ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@catId", club.CategoryId);
            cmd.Parameters.AddWithValue("@logo", club.LogoUrl ?? "default_club.png");
            cmd.Parameters.AddWithValue("@cover", club.CoverUrl ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@insta", club.InstagramLink ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@linked", club.LinkedinLink ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", club.ClubId);

            cmd.ExecuteNonQuery();
        }
    }
}

        // --- YARDIMCI METOT ---
        private Club MapReaderToClub(NpgsqlDataReader dr)
        {
            return new Club
            {
                ClubId = Convert.ToInt32(dr["club_id"]),
                ClubName = dr["club_name"].ToString(),
                Description = dr["description"] != DBNull.Value ? dr["description"].ToString() : "",
                FullDescription = dr["full_description"] != DBNull.Value ? dr["full_description"].ToString() : "",
                CategoryId = Convert.ToInt32(dr["category_id"]),
                ManagerUserId = Convert.ToInt32(dr["manager_user_id"]),
                LogoUrl = dr["logo_url"] != DBNull.Value ? dr["logo_url"].ToString() : "default_club.png",
                CoverUrl = dr["cover_url"] != DBNull.Value ? dr["cover_url"].ToString() : null,
                InstagramLink = dr["instagram_link"] != DBNull.Value ? dr["instagram_link"].ToString() : "",
                LinkedinLink = dr["linkedin_link"] != DBNull.Value ? dr["linkedin_link"].ToString() : "",
                RequiresApproval = dr["requires_approval"] != DBNull.Value && (bool)dr["requires_approval"],
                IsActive = (bool)dr["is_active"]
            };
        }
    }
}