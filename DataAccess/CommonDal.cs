using System;
using System.Collections.Generic;
using Npgsql;
using CampusEventManager.Entities;

namespace CampusEventManager.DataAccess
{
    public class CommonDal
    {
        

        public List<Category> GetAllCategories()
        {
            List<Category> list = new List<Category>();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT category_id, category_name FROM categories ORDER BY category_name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Category 
                            { 
                                CategoryId = Convert.ToInt32(reader["category_id"]),
                                CategoryName = reader["category_name"].ToString()!
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void AddCategory(string categoryName)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "INSERT INTO categories (category_name) VALUES (@name)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", categoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public void DeleteCategory(int id)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "DELETE FROM categories WHERE category_id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        

        public List<Club> GetAllClubs()
        {
            List<Club> list = new List<Club>();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT club_id, club_name FROM clubs ORDER BY club_name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Club 
                            { 
                                ClubId = Convert.ToInt32(reader["club_id"]),
                                ClubName = reader["club_name"].ToString()!
                            });
                        }
                    }
                }
            }
            return list;
        }

        

        public List<User> GetAllStudents()
        {
            List<User> list = new List<User>();
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT user_id, full_name, email FROM users WHERE role = 'STUDENT' ORDER BY full_name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User 
                            { 
                                UserId = Convert.ToInt32(reader["user_id"]),
                                FullName = reader["full_name"].ToString()!,
                                Email = reader["email"].ToString()!
                            });
                        }
                    }
                }
            }
            return list;
        }

        

        public DashboardStats GetDashboardStats()
        {
            DashboardStats stats = new DashboardStats();
            using (var conn = DbHelper.GetConnection())
            {
                
                string sql = "SELECT * FROM view_dashboard_stats";
                
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            
                            
                            stats.TotalClubs = Convert.ToInt32(reader["total_clubs"]);
                            stats.TotalEvents = Convert.ToInt32(reader["active_events"]); 
                            stats.TotalApplications = Convert.ToInt32(reader["pending_apps"]); 
                        }
                    }
                }
            }
            return stats;
        }
    }

    
    public class DashboardStats
    {
        public int TotalClubs { get; set; }
        public int TotalEvents { get; set; }
        public int TotalApplications { get; set; }
    }
}