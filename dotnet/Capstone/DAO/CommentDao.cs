using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.Exceptions;

namespace Capstone.DAO
{
    public class CommentDao : ICommentDao
    {
        private readonly string connectionString;
        // Initializes a new instance of the PostDao class.
        // 
        // Parameters:
        //   dbConnectionString: The connection string for the database.
        public CommentDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        
        // Retrieves all posts from the database.
        //
        // Returns a list of Post objects representing the retrieved posts.
        public List<Comment> GetAllComments()
        {
            List<Comment> commentList = new List<Comment>();

            string query = "SELECT comment_id, user_id, post_id, comment_content, up_votes, down_votes, date_created, forum_id " +
                    "FROM comment";
            
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Comment comment = new Comment
                        {
                            CommentID = Convert.ToInt32(reader["comment_id"]),
                            PostID = Convert.ToInt32(reader["post_id"]),
                            UserID = Convert.ToInt32(reader["user_id"]),
                            CommentContent = reader["comment_content"].ToString(),
                            UpVotes = Convert.ToInt32(reader["up_votes"]),
                            DownVotes = Convert.ToInt32(reader["down_votes"]),
                            DateCreated = Convert.ToDateTime(reader["date_created"]),
                            ForumID = Convert.ToInt32(reader["forum_id"]),
                        };

                        commentList.Add(comment);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("Failed to retrieve comments", e);
            }

            return commentList;
        }

        public Comment GetCommentById(int id)
        {
            Comment comment = null;
            
            string query = "SELECT comment_id, user_id, post_id, comment_content, up_votes, down_votes, date_created, forum_id, parent_id " +
                    "FROM comment " +
                    "WHERE comment_id = @Id";

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
                        comment = new Comment
                        {
                            CommentID = Convert.ToInt32(reader["comment_id"]),
                            PostID = Convert.ToInt32(reader["post_id"]),
                            UserID = Convert.ToInt32(reader["user_id"]),
                            CommentContent = reader["comment_content"].ToString(),
                            UpVotes = Convert.ToInt32(reader["up_votes"]),
                            DownVotes = Convert.ToInt32(reader["down_votes"]),
                            DateCreated = Convert.ToDateTime(reader["date_created"]),
                            ForumID = Convert.ToInt32(reader["forum_id"]),
                            ParentID = reader["parent_id"] != DBNull.Value ? Convert.ToInt32(reader["parent_id"]) :(int?)null
                        };
                    }
                }        
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }
            return comment;
        }

        public List<Comment> GetCommentsByPostId(int id)
        {
            List<Comment> commentList = new List<Comment>();

            string query = "SELECT c.comment_id, c.user_id, c.post_id, c.comment_content, c.up_votes, c.down_votes, c.date_created, c.forum_id, c.parent_id " +
                    "FROM comment AS c " +
                    "JOIN posts AS p ON c.post_id = p.post_id " +
                    "WHERE c.post_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Comment comment = new Comment
                        {
                           
                            CommentID = Convert.ToInt32(reader["comment_id"]),
                            PostID = Convert.ToInt32(reader["post_id"]),
                            UserID = Convert.ToInt32(reader["user_id"]),
                            CommentContent = reader["comment_content"].ToString(),
                            UpVotes = Convert.ToInt32(reader["up_votes"]),
                            DownVotes = Convert.ToInt32(reader["down_votes"]),
                            DateCreated = Convert.ToDateTime(reader["date_created"]),
                            ForumID = Convert.ToInt32(reader["forum_id"]),
                            ParentID = reader["parent_id"] != DBNull.Value ? Convert.ToInt32(reader["parent_id"]) : (int?)null
                        };

                        commentList.Add(comment);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return commentList;
        }
        public Comment CreateComment(Comment comment)
        {
            string query = "INSERT INTO comment (user_id, post_id, comment_content, up_votes, down_votes, date_created, forum_id) " +
                        "VALUES (@UserId, @PostId, @CommentContent, @UpVotes, @DownVotes, @DateCreated, @ForumID)" +
                        "SELECT SCOPE_IDENTITY();";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", comment.UserID);
                    cmd.Parameters.AddWithValue("@PostId", comment.PostID);
                    cmd.Parameters.AddWithValue("@CommentContent", comment.CommentContent);
                    cmd.Parameters.AddWithValue("@UpVotes", comment.UpVotes);
                    cmd.Parameters.AddWithValue("@DownVotes", comment.DownVotes);
                    cmd.Parameters.AddWithValue("@DateCreated", comment.DateCreated);
                    cmd.Parameters.AddWithValue("@ForumID", comment.ForumID);

                    int newCommentId = Convert.ToInt32(cmd.ExecuteScalar());

                    comment.CommentID = newCommentId;
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return comment;
        }

        public Comment UpdateComment(Comment comment)
        {
            string query = "UPDATE comment " +
                        "SET user_id = @UserId, post_id = @PostId, comment_content = @CommentContent, up_votes = @UpVotes, down_votes = @DownVotes, date_created = @DateCreated, forum_id = @ForumId " +
                        "WHERE comment_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", comment.UserID);
                    cmd.Parameters.AddWithValue("@PostId", comment.PostID);
                    cmd.Parameters.AddWithValue("@CommentContent", comment.CommentContent);
                    cmd.Parameters.AddWithValue("@UpVotes", comment.UpVotes);
                    cmd.Parameters.AddWithValue("@DownVotes", comment.DownVotes);
                    cmd.Parameters.AddWithValue("@DateCreated", comment.DateCreated);
                    cmd.Parameters.AddWithValue("@ForumId", comment.ForumID);
                    cmd.Parameters.AddWithValue("@Id", comment.CommentID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Comment not found");
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return comment;
        }
        public Comment DeleteComment(int id)
        {
            string query = "DELETE FROM comment WHERE comment_id = @Id";
            Comment deletedComment = null;

            try
            {
                deletedComment = GetCommentById(id);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Deletion failed: Comment not found");
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return deletedComment;
        }
    }
    
}