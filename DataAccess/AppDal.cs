using System;
using System.Data;
using Npgsql;
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class AppDal
    {
        // 1. ETKİNLİĞE GÖRE BAŞVURULAR (Yönetici Paneli İçin)
        public DataTable GetApplicationsByEvent(int eventId)
        {
            DataTable dt = new DataTable();
            using (var conn = DbHelper.GetConnection())
            {
                // view_event_details view'ının event_applications tablosunu kullandığından emin ol
                string sql = "SELECT * FROM view_event_details WHERE event_id = @id ORDER BY applied_at DESC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", eventId);
                    using (var da = new NpgsqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }

        // 2. BAŞVURU YAP (event_applications tablosuna sabitlendi)
        public void ApplyToEvent(int userId, int eventId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                // Status ENUM hatasını önlemek için '0'::application_status şeklinde gönderiyoruz
                string sql = "INSERT INTO event_applications (user_id, event_id, status) VALUES (@uid, @eid, '0'::application_status)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@eid", eventId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. BAŞVURU KONTROLÜ (event_applications tablosuna sabitlendi)
        public bool IsUserApplied(int userId, int eventId)
        {
            bool result = false;
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT COUNT(*) FROM event_applications WHERE user_id = @uid AND event_id = @eid";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@eid", eventId);
                    long count = Convert.ToInt64(cmd.ExecuteScalar());
                    if (count > 0) result = true;
                }
            }
            return result;
        }

        // 4. BAŞVURU ONAYLA
        public void ApproveApplication(int applicationId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "CALL approve_application(@appId)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@appId", applicationId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 5. BAŞVURULARIM SAYFASI (Görünmeme sorunu burada çözüldü)
        public DataTable GetApplicationsByUser(int userId)
        {
            DataTable dt = new DataTable();
            using (var conn = DbHelper.GetConnection())
            {
                // status::text ekleyerek ENUM/Tür hatasını engelledik
                string sql = @"
                    SELECT 
                        ea.application_id AS Id,
                        e.title AS Etkinlik,
                        ea.status::text AS Durum,
                        ea.applied_at AS Tarih
                    FROM event_applications ea
                    JOIN events e ON ea.event_id = e.event_id
                    WHERE ea.user_id = @uid
                    ORDER BY ea.applied_at DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (var da = new NpgsqlDataAdapter(cmd)) { da.Fill(dt); }
                }
            }
            return dt;
        }

        // 6. BAŞVURU SİL
        public void RemoveApplication(int applicationId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "DELETE FROM event_applications WHERE application_id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", applicationId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}