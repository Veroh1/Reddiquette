using Capstone.Exceptions;
using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capstone.DAO
{
    public class ForumDao : IForumDao
    {
        private readonly string connectionString;

        // Initializes a new instance of the ForumDao class.
        //
        // Parameters:
        //   dbConnectionString: The connection string for the database.
        public ForumDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        // Finds a forum in the database based on its ID.
        //
        // Parameters:
        //   id: The ID of the forum to find.
        //
        // Returns: The forum with the specified ID, or null if no forum is found.
        public Forum FindForumByID(int id)
        {
            Forum forum = new();

            string query = "SELECT forum_id, forum_title, forum_description, image_path, date_created " +
                        "FROM forum " +
                        "WHERE forum_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        forum = MapRowToForum(reader);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }
            return forum;
        }

        // Retrieves all forums from the database.
        //
        // Returns: A list of Forum objects representing all the forums retrieved.
        public List<Forum> GetAllForums()
        {
            List<Forum> forumList = new List<Forum>();

            string query = "SELECT forum_id, forum_title, forum_description, image_path, date_created " +
                        "FROM forum";
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Forum forum = MapRowToForum(reader);

                        forumList.Add(forum);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }
            return forumList;
        }

        // Creates a new forum in the database.
        //
        // Parameters:
        //   forum: The forum object containing the details of the forum to be created.
        //
        // Returns: The updated forum object with the assigned forum ID.
        public Forum CreateForum(Forum forum)
        {
            string query = "INSERT INTO forum (forum_title, forum_description, image_path, date_created) " +
                          "VALUES (@Title, @Description, @ImagePath, @DateCreated); " +
                          "SELECT SCOPE_IDENTITY();";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", forum.ForumTitle);
                    cmd.Parameters.AddWithValue("@Description", forum.ForumDescription);
                    cmd.Parameters.AddWithValue("@ImagePath", forum.ImagePath);
                    cmd.Parameters.AddWithValue("@DateCreated", forum.DateCreated);

                    int newForumId = Convert.ToInt32(cmd.ExecuteScalar());

                    forum.ForumId = newForumId;
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return forum;
        }

        // Updates a forum in the database.
        //
        // Parameters:
        //   forum: The forum object to be updated.
        //
        // Returns:  updated forum object.
        public Forum UpdateForum(Forum forum)
        {
            string query = "UPDATE forum " +
                        "SET forum_title = @Title, forum_description = @Description, image_path = @ImagePath " +
                        "WHERE forum_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", forum.ForumTitle);
                    cmd.Parameters.AddWithValue("@Description", forum.ForumDescription);
                    cmd.Parameters.AddWithValue("@ImagePath", forum.ImagePath);
                    cmd.Parameters.AddWithValue("@Id", forum.ForumId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Update failed: Forum not found");
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return forum;
        }

     
        public Forum DeleteForumByID(int id)
        {
            Forum deletedForum = null;

            try
            {
                deletedForum = FindForumByID(id);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            DeleteIfExistsWithTransaction(transaction, "comment_votes", "comment_id IN (SELECT comment_id FROM comment WHERE post_id IN (SELECT post_id FROM posts WHERE forum_id = @Id))", id);
                            DeleteIfExistsWithTransaction(transaction, "comment", "post_id IN (SELECT post_id FROM posts WHERE forum_id = @Id)", id);
                            DeleteIfExistsWithTransaction(transaction, "post_votes", "post_id IN (SELECT post_id FROM posts WHERE post_id IN (SELECT post_id FROM posts WHERE forum_id = @Id))", id);
                            DeleteIfExistsWithTransaction(transaction, "posts", "forum_id = @Id", id);
                            DeleteIfExistsWithTransaction(transaction, "user_forum", "forum_id = @Id", id);
                            DeleteIfExistsWithTransaction(transaction, "forum", "forum_id = @Id", id);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new DaoException("Transaction rolled back due to an error: " + ex.Message, ex);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return deletedForum;
        }
        private void DeleteIfExistsWithTransaction(SqlTransaction transaction, string tableName, string condition, int id)
        {
            string query = $"DELETE FROM {tableName} WHERE {condition}";

            using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                   
                    Console.WriteLine($"Deletion skipped for {tableName} as there were no matching rows.");
                }
            }
        }

        private Forum MapRowToForum(SqlDataReader reader)
        {
            Forum forum = new Forum();

            forum.ForumId = Convert.ToInt32(reader["forum_id"]);
            forum.ForumTitle = reader["forum_title"].ToString();
            forum.ForumDescription = reader["forum_description"].ToString();
            forum.ImagePath = reader["image_path"].ToString();
            forum.DateCreated = Convert.ToDateTime(reader["date_created"]);

            return forum;
        }
    }
}
