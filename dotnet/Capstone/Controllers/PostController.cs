using Microsoft.AspNetCore.Mvc;
using Capstone.DAO;
using Capstone.Exceptions;
using Capstone.Models;
using Capstone.Security;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using AutoMapper;
using Capstone.DTO;

namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostDao postDao;

        // Initializes a new instance of the PostController class.
        // 
        // Parameters:
        //   postDao: The IPostDao object used to interact with the post data.
        //   mapper: The IMapper object used for object mapping.
        public PostController(IPostDao postDao)
        {
            this.postDao = postDao;
        }

        // Retrieves all posts and returns them as a list of PostDto objects.
        [HttpGet("/posts")]
        public IActionResult GetAllPosts()
        {
            try
            {
                var posts = postDao.GetAllPosts();

                return Ok(posts);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve posts: {e.Message}");
            }
        }

        // Retrieves a post by its unique identifier.
        //
        // Parameters:
        //   id: The ID of the post to retrieve.
        //
        // Returns: An IActionResult containing the post with the specified ID, if found.
        [HttpGet("/posts/{id}")]
        public IActionResult GetPostById(int id)
        {
            try
            {
                var post = postDao.GetPostByID(id);
                if (post == null)
                {
                    return NotFound($"No post found with ID {id}.");
                }

                return Ok(post);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to find post by ID {id}: {e.Message}");
            }
        }

        // Retrieves posts by forum ID.
        //
        // Parameters:
        //   id:
        //     The ID of the forum.
        //
        // Returns: An IActionResult object representing the HTTP response.
        [HttpGet("/posts/forum/{id}")]
        public IActionResult GetPostsByForumId(int id)
        {
            try
            {
                var posts = postDao.GetPostsByForumId(id);
                if (posts == null)
                {
                    return NotFound($"No posts found with Forum ID {id}.");
                }

                return Ok(posts);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve posts with Forum ID {id}: {e.Message}");
            }
        }


        //Creates a new post.

        //Parameters:
        //   post: The post object containing the details of the post to be created.

        // Returns:
        //     An IActionResult representing the HTTP response for the create post operation.
        //   If the post is created successfully, it returns a CreatedAtAction result with the created post object.
        //   If an error occurs during the creation of the post, it returns a StatusCode result with a 500 status code
        //   and an error message indicating the failure.
        [HttpPost("/posts")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult CreatePost(Post post)
        {
            try
            {
                var createdPost = postDao.CreatePost(post);
                return CreatedAtAction(nameof(GetPostById), new { id = createdPost.PostID }, createdPost);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to create post: {e.Message}");
            }
        }

        // Updates a post with the specified ID.
        //
        // Parameters:
        //   id: The ID of the post to update.
        //   post: The updated post object.
        //
        // Returns: An IActionResult representing the result of the update operation.
        [HttpPut("/posts/{id}")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult UpdatePost(int id, Post post)
        {
            if (id != post.PostID)
            {
                return BadRequest("Post ID mismatch");
            }
            try
            {
                var updatedPost = postDao.UpdatePost(post);
                if (updatedPost == null)
                {
                    return NotFound($"No post found.");
                }
                return Ok(updatedPost);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to update post: {e.Message}");
            }
        }

        // Deletes a post with the specified ID.
        //
        // Parameters:
        //   id: The ID of the post to delete.
        //
        // Returns:
        //   An IActionResult representing the result of the deletion operation.
        [HttpDelete("/posts/{id}")]
        [Authorize(Roles = "admin, moderator")]
        public IActionResult DeletePost(int id)
        {
            try
            {
                var deletedPost = postDao.DeletePost(id);
                if (deletedPost == null)
                {
                    return NotFound($"No post found.");
                }
                return Ok(deletedPost);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to delete post: {e.Message}");
            }
        }
    }
}