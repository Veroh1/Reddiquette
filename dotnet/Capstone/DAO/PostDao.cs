using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.Exceptions;
using Microsoft.Extensions.Hosting;

namespace Capstone.DAO
{
    public class PostDao : IPostDao
    {
        private readonly string connectionString;

        // Initializes a new instance of the PostDao class.
        // 
        // Parameters:
        //   dbConnectionString: The connection string for the database.
        public PostDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        // Retrieves a post from the database based on the specified ID.
        //
        // Parameters:
        //     id: The ID of the post to retrieve.
        //
        // Returns: The retrieved Post object.
        public Post GetPostByID(int id)
        {
            Post post = new();

            string query = "SELECT post_id, post_title, user_id, post_content, date_created, forum_id, post_imgurl " +
                    "FROM posts " +
                    "WHERE post_id = @Id";

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
                        post = MapRowToPost(reader);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return post;
        }

        // Retrieves all posts from the database.
        //
        // Returns a list of Post objects representing the retrieved posts.
        public List<Post> GetAllPosts()
        {
            List<Post> postList = new List<Post>();

            string query = "SELECT p.post_id, p.post_title, p.user_id, p.post_content, p.date_created, p.forum_id, p.post_imgurl " +
                    "FROM posts AS p ";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Post post = MapRowToPost(reader);

                        postList.Add(post);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return postList;
        }
        
        // Retrieves a list of posts based on the specified forum ID.
        // 
        // Parameters:
        //   id: The ID of the forum.
        //
        // Returns: A list of Post objects.
        public List<Post> GetPostsByForumId(int id)
        {
            List<Post> postList = new List<Post>();

            string query = "SELECT p.post_id, p.user_id, p.post_title, p.post_content, p.date_created, p.forum_id, p.post_imgurl " +
                        "FROM posts AS p " +
                        "WHERE p.forum_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Post post = MapRowToPost(reader);

                        postList.Add(post);
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return postList;
        }

        // Creates a new post in the database.
        //
        // Parameters:
        //   post: The post object containing the data to be inserted.
        //
        // Returns: The newly created post with the updated PostID.
        public Post CreatePost(Post post)
        {
            string query = "INSERT INTO posts (user_id, post_content, date_created, forum_id, post_title, post_imgurl) " +
                        "VALUES ( @UserID, @PostContent, @DateCreated, @ForumID, @PostTitle, @ImagePath); " +
                        "SELECT SCOPE_IDENTITY();";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    
                    cmd.Parameters.AddWithValue("@UserID", post.UserID);
                    cmd.Parameters.AddWithValue("@PostContent", post.PostContent);
                    cmd.Parameters.AddWithValue("@DateCreated", post.DateCreated);
                    cmd.Parameters.AddWithValue("@ForumID", post.ForumId);
                    cmd.Parameters.AddWithValue("@PostTitle", post.PostTitle);
                    cmd.Parameters.AddWithValue("@ImagePath", post.ImagePath);

                    int newPostId = Convert.ToInt32(cmd.ExecuteScalar());

                    post.PostID = newPostId;
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return post;
        }

        // Updates a post in the database.
        //
        // Parameters:
        //   post: The post object containing the updated information.
        //
        // Returns: The updated post object.
        public Post UpdatePost(Post post)
        {
            string query = "UPDATE posts " +
                        "SET user_id = @UserID, post_content = @PostContent, date_created = @DateCreated, forum_id = @ForumID, post_title = "+ "@PostTitle, post_imgurl = @imgurl " +
                        "WHERE post_id = @Id";

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", post.UserID);
                    cmd.Parameters.AddWithValue("@PostContent", post.PostContent);
                    cmd.Parameters.AddWithValue("@DateCreated", post.DateCreated);
                    cmd.Parameters.AddWithValue("@ForumID", post.ForumId);
                    cmd.Parameters.AddWithValue("@Id", post.PostID);
                    cmd.Parameters.AddWithValue("@PostTitle", post.PostTitle);
                    cmd.Parameters.AddWithValue("@imgurl", post.ImagePath);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new DaoException("Update failed: Post not found");
                    }
                }
            }
            catch (SqlException e)
            {
                throw new DaoException("SQL exception occurred", e);
            }

            return post;
        }

        // Deletes a post with the given ID from the database.
        //
        // Parameters:
        //   id: The ID of the post to be deleted.
        //
        // Returns: The deleted post.
        public Post DeletePost(int id)
        {
            Post deletedPost = null;

            try
            {
                deletedPost = GetPostByID(id);

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            DeleteIfExistsWithTransaction(transaction, "post_votes", "post_id = @PostId", id);
                            DeleteIfExistsWithTransaction(transaction, "comment_votes", "comment_id IN (SELECT comment_id FROM comment WHERE post_id = @PostId)", id);
                            DeleteIfExistsWithTransaction(transaction, "comment", "post_id = @PostId", id);
                            DeleteIfExistsWithTransaction(transaction, "posts", "post_id = @PostId", id);


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

            return deletedPost;
        }
        private void DeleteIfExistsWithTransaction(SqlTransaction transaction, string tableName, string condition, int id)
        {
            string query = $"DELETE FROM {tableName} WHERE {condition}";

            using (var cmd = new SqlCommand(query, transaction.Connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PostId", id);
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {

                    Console.WriteLine($"Deletion skipped for {tableName} as there were no matching rows.");
                }
            }
        }

        private Post MapRowToPost(SqlDataReader reader)
        {
            Post post = new Post();


            post.PostID = Convert.ToInt32(reader["post_id"]);
            post.UserID = Convert.ToInt32(reader["user_id"]);
            post.PostContent = reader["post_content"].ToString();
            post.DateCreated = Convert.ToDateTime(reader["date_created"]);
            post.ForumId = Convert.ToInt32(reader["forum_id"]);
            post.PostTitle = reader["post_title"].ToString();
            post.ImagePath = reader["post_imgurl"].ToString();

            return post;
        }
    }
}