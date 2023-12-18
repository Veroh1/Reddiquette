using Capstone.Exceptions;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capstone.DAO
{
    public class VotesDao : IVotesDao
    {
        private readonly string connectionString;

        public VotesDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public ReturnVotes GetAllPostVotesById(int targetId)
        {
            ReturnVotes votes = new ReturnVotes() { TargetID = targetId, Upvotes = 0, Downvotes = 0 };
            IList<Vote> votesList = new List<Vote>();
           

            string query = "SELECT * FROM post_votes WHERE target_id = @postId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("postId", targetId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Vote vote = MapRowToVote(reader);
                        votesList.Add(vote);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }
            foreach (Vote vote in votesList)
            {
                if (vote.Increment == 1)
                {
                    votes.Upvotes += 1;
                }
                else if (vote.Increment == -1)
                {
                    votes.Downvotes += 1;
                }
                else continue;
            }

            return votes;
        }

        public Vote GetPostVoteById(int postId, int userId)
        {
            Vote vote = new Vote();
            string sql = "SELECT * FROM post_votes WHERE user_id = @userId AND target_id = @targetId";
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand command = new SqlCommand(sql, conn);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@targetId", postId);
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            vote = MapRowToVote(reader);
                        }
                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return vote;
        }

        public ReturnVotes GetAllCommentVotesById(int targetID)
        {
            ReturnVotes votes = new ReturnVotes() { TargetID = targetID, Upvotes = 0, Downvotes = 0 };
            IList<Vote> votesList = new List<Vote>();
         
            string query = "SELECT * FROM comment_votes WHERE target_id = @commentId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("commentId", targetID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Vote vote = MapRowToVote(reader);
                        votesList.Add(vote);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }
            foreach (Vote vote in votesList)
            {
                if (vote.Increment == 1)
                {
                    votes.Upvotes += 1;
                }
                if (vote.Increment == -1)
                {
                    votes.Downvotes += 1;
                }
            }

            return votes;
        }

        public Vote GetCommentVoteById(int userId, int commentId)
        {
            Vote vote = null;
            string sql = "SELECT * FROM comment_votes WHERE user_id = @userId AND target_id = @targetId";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@targetId", commentId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        vote = MapRowToVote(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }

            return vote;
        }

        public Vote CreatePostVote(int userId, int targetID, int increment)
        {
            Vote vote = null;

            string sql = "INSERT INTO post_votes(user_id, target_id, inc) " +
                "VALUES (@userId, @targetId, @inc)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@targetId", targetID);
                    cmd.Parameters.AddWithValue("@inc", increment);

                    cmd.ExecuteNonQuery();

                    vote = GetPostVoteById(userId, targetID);

                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }
            return vote;
        }

        public Vote CreateCommentVote(int userId, int targetID, int increment)
        {
            Vote vote = null;

            string sql = "INSERT INTO comment_votes(user_id, target_id, inc) " +
                "VALUES (@userId, @targetId, @inc)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@targetId", targetID);
                    cmd.Parameters.AddWithValue("@inc", increment);

                    cmd.ExecuteNonQuery();

                    vote = GetCommentVoteById(userId, targetID);
                }
            }
            catch (SqlException ex)
            {
                throw new DaoException("SQL exception occurred", ex);
            }
            return vote;
        }

        public Vote UpdateCommentVote(int userId, int commentId, int increment)
        {
            Vote vote = null;
            string query = "UPDATE comment_votes " +
                        "SET inc = @Inc " +
                        "WHERE target_id = @commentId AND user_id = @userId";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Inc", increment);
                    cmd.Parameters.AddWithValue("@commentId", commentId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Update failed: Vote not found");
                    }
                    vote = GetCommentVoteById(userId, commentId);
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return vote;
        }
        
        public Vote UpdatePostVote(int userId, int postId, int increment)
        {
            Vote vote = null;
            string query = "UPDATE post_votes " +
                        "SET inc = @Inc " +
                        "WHERE comment_id = @posttId AND user_id = @userId";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Inc", increment);
                    cmd.Parameters.AddWithValue("@postId", postId);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Update failed: Vote not found");
                    }
                    vote = GetPostVoteById(userId, postId);
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return vote;
        }

        public Vote DeletePostVote(int userId, int targetId)
        {
            Vote deletedVote = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        DeleteIfExistsWithTransaction(transaction, "post_votes", "user_id = @userId AND target_id = @targetId", targetId, userId);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new DaoException("Transaction rolled back due to an error: " + ex.Message, ex);
                    }
                }
            }
            return deletedVote;
        }

        public Vote DeleteCommentVote(int userId, int targetId)
        {
            Vote deletedVote = null;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        DeleteIfExistsWithTransaction(transaction, "comment_votes", "user_id = @userId AND comment_id = @targetId", targetId, userId);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new DaoException("Transaction rolled back due to an error: " + ex.Message, ex);
                    }
                }
            }
            return deletedVote;
        }

        private Vote MapRowToVote(SqlDataReader reader)
        {
            Vote vote = new Vote();
            vote.UserID = Convert.ToInt32(reader["user_id"]);
            vote.TargetID = Convert.ToInt32(reader["target_id"]);
            vote.Increment = Convert.ToInt32(reader["inc"]);
            return vote;
        }

        private void DeleteIfExistsWithTransaction(SqlTransaction transaction, string tableName, string condition, int targetId, int userId)
        {
            string query = $"DELETE FROM {tableName} WHERE {condition}";

            using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@targetId", targetId);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {

                    Console.WriteLine($"Deletion skipped for {tableName} as there were no matching rows.");
                }
            }
        }
    }
}
