using System;
using Npgsql;
using CampusEventManager.DataAccess;

namespace CampusEventManager.DataAccess
{
    public class FeedbackDal
    {
        
        public void GiveFeedback(int eventId, int userId, int rating, string comment)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "INSERT INTO event_feedbacks (event_id, user_id, rating, comment) VALUES (@eid, @uid, @rate, @com)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@eid", eventId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@rate", rating);
                    cmd.Parameters.AddWithValue("@com", comment);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        
        public bool HasFeedback(int eventId, int userId)
        {
            using (var conn = DbHelper.GetConnection())
            {
                string sql = "SELECT COUNT(*) FROM event_feedbacks WHERE event_id=@eid AND user_id=@uid";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@eid", eventId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    long count = (long)cmd.ExecuteScalar()!;
                    return count > 0;
                }
            }
        }
    }
}