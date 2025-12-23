using System;
using System.Data;
using Npgsql;
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class AnnouncementDal
    {
        // Duyuru Ekle
        public void AddAnnouncement(string title, string content, int clubId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "INSERT INTO announcements (club_id, title, content) VALUES (@cid, @title, @content)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", clubId);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Duyuruları Listele
        public DataTable GetAllAnnouncements()
        {
            DataTable dt = new DataTable();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT a.title AS Konu, a.content AS Mesaj, c.club_name AS Kulüp, a.created_at AS Tarih " +
                             "FROM announcements a JOIN clubs c ON a.club_id = c.club_id ORDER BY a.created_at DESC";
                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}