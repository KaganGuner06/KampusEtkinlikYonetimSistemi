using System;
using System.Collections.Generic;
using System.Data; 
using Npgsql;
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class EventDal
    {
        // 1. KULÜBE AİT ETKİNLİKLER
        public DataTable GetEventsByClub(int clubId)
        {
            DataTable dt = new DataTable();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = @"
                    SELECT 
                        e.event_id, 
                        e.title AS Baslik, 
                        e.event_date AS Tarih, 
                        e.location AS Konum,
                        COALESCE((SELECT AVG(rating) FROM event_feedbacks f WHERE f.event_id = e.event_id), 0) AS Puan
                    FROM events e 
                    WHERE e.club_id = @cid 
                    ORDER BY e.event_date DESC";

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

        // 2. TÜM ETKİNLİKLER
        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = @"
                    SELECT e.*, c.club_name,
                    COALESCE((SELECT AVG(rating) FROM event_feedbacks f WHERE f.event_id = e.event_id), 0) AS avg_rating
                    FROM events e
                    JOIN clubs c ON e.club_id = c.club_id
                    ORDER BY e.event_date DESC";
                
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) events.Add(MapToEvent(reader));
                    }
                }
            }
            return events;
        }

        // 3. FİLTRELEME METODU (GÜNCELLENDİ: Arama Parametresi Eklendi)
        public List<Event> GetEventsByFilter(int? categoryId, DateTime? minDate, string searchText = null)
        {
            List<Event> events = new List<Event>();
            using (var conn = DbHelper.GetConnection())
            {
                // Temel Sorgu
                string sql = @"
                    SELECT e.*, c.club_name,
                    COALESCE((SELECT AVG(rating) FROM event_feedbacks f WHERE f.event_id = e.event_id), 0) AS avg_rating
                    FROM events e
                    JOIN clubs c ON e.club_id = c.club_id
                    WHERE 1=1"; // Dinamik WHERE koşulları için başlangıç

                // Parametreleri ekle
                if (categoryId != null && categoryId > 0) 
                    sql += " AND e.category_id = @catId";
                
                if (minDate != null) 
                    sql += " AND e.event_date >= @minDate";

                // Arama filtresi (Büyük/Küçük harf duyarsız arama)
                if (!string.IsNullOrEmpty(searchText))
                    sql += " AND LOWER(e.title) LIKE LOWER(@search)";

                sql += " ORDER BY e.event_date";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (categoryId != null && categoryId > 0) 
                        cmd.Parameters.AddWithValue("@catId", categoryId);
                    
                    if (minDate != null) 
                        cmd.Parameters.AddWithValue("@minDate", minDate);

                    if (!string.IsNullOrEmpty(searchText))
                        cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) events.Add(MapToEvent(reader));
                    }
                }
            }
            return events;
        }

        // 4. TEK ETKİNLİK GETİR
        public Event GetEventById(int id)
        {
            Event evt = null;
            using (var conn = DbHelper.GetConnection())
            {
                string sql = @"
                    SELECT e.*, c.club_name,
                    COALESCE((SELECT AVG(rating) FROM event_feedbacks f WHERE f.event_id = e.event_id), 0) AS avg_rating
                    FROM events e
                    JOIN clubs c ON e.club_id = c.club_id
                    WHERE e.event_id = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) evt = MapToEvent(reader);
                    }
                }
            }
            return evt;
        }

        // --- EKLEME / SİLME / GÜNCELLEME ---
        public void AddEvent(Event e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "INSERT INTO events (club_id, category_id, title, description, event_date, location, quota, poster_url, is_published) VALUES (@cid, @cat, @tit, @desc, @date, @loc, @quota, @post, @pub)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@cid", e.ClubId);
                    cmd.Parameters.AddWithValue("@cat", e.CategoryId);
                    cmd.Parameters.AddWithValue("@tit", e.Title);
                    cmd.Parameters.AddWithValue("@desc", e.Description);
                    cmd.Parameters.AddWithValue("@date", e.EventDate);
                    cmd.Parameters.AddWithValue("@loc", e.Location);
                    cmd.Parameters.AddWithValue("@quota", e.Quota);
                    cmd.Parameters.AddWithValue("@post", e.PosterUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@pub", e.IsPublished);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEvent(Event e)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "UPDATE events SET title=@tit, description=@desc, event_date=@date, location=@loc, quota=@quota, poster_url=@post, is_published=@pub WHERE event_id=@id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tit", e.Title);
                    cmd.Parameters.AddWithValue("@desc", e.Description);
                    cmd.Parameters.AddWithValue("@date", e.EventDate);
                    cmd.Parameters.AddWithValue("@loc", e.Location);
                    cmd.Parameters.AddWithValue("@quota", e.Quota);
                    cmd.Parameters.AddWithValue("@post", e.PosterUrl ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@pub", e.IsPublished);
                    cmd.Parameters.AddWithValue("@id", e.EventId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEvent(int id)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "DELETE FROM events WHERE event_id=@id";
                using (var cmd = new NpgsqlCommand(sql, conn)) { cmd.Parameters.AddWithValue("@id", id); cmd.ExecuteNonQuery(); }
            }
        }

        // --- MAP METODU (GÜVENLİ OKUMA) ---
        private Event MapToEvent(NpgsqlDataReader reader)
        {
            var evt = new Event();
            try { evt.EventId = Convert.ToInt32(reader["event_id"]); } catch {}
            try { evt.ClubId = Convert.ToInt32(reader["club_id"]); } catch {}
            try { evt.CategoryId = Convert.ToInt32(reader["category_id"]); } catch {}
            try { evt.Title = reader["title"].ToString(); } catch {}
            try { evt.Description = reader["description"].ToString(); } catch {}
            try { evt.EventDate = Convert.ToDateTime(reader["event_date"]); } catch {}
            try { evt.Location = reader["location"].ToString(); } catch {}
            try { evt.Quota = Convert.ToInt32(reader["quota"]); } catch {}
            try { evt.IsPublished = Convert.ToBoolean(reader["is_published"]); } catch {}
            try { evt.PosterUrl = reader["poster_url"] != DBNull.Value ? reader["poster_url"].ToString() : "default_event.jpg"; } catch {}
            
            if (HasColumn(reader, "club_name"))
                try { evt.ClubName = reader["club_name"].ToString(); } catch {}

            if (HasColumn(reader, "avg_rating"))
                try { evt.AverageRating = Convert.ToDouble(reader["avg_rating"]); } catch { evt.AverageRating = 0; }

            return evt;
        }

        private bool HasColumn(NpgsqlDataReader dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase)) return true;
            }
            return false;
        }
    }
}