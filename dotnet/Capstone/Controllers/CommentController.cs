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

    public class CommentController : ControllerBase
    {
        private readonly ICommentDao commentDao;

        // Initializes a new instance of the CommentController class.
        // 
        // Parameters:
        //   commentDao: The ICommentDao object used to interact with the comment data.
        //   mapper: The IMapper object used for object mapping.
        public CommentController(ICommentDao commentDao)
        {
            this.commentDao = commentDao;
        }

        // Retrieves all comments and returns them as a list of CommentDto objects.
        [HttpGet("/comment")]
        public IActionResult GetAllComments()
        {
            try
            {
                var comments = commentDao.GetAllComments();

                return Ok(comments);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve comments: {e.Message}");
            }
        }

        // Retrieves a comment by its unique identifier.
        //
        // Parameters:
        //   id: The ID of the comment to retrieve.
        //
        // Returns: An IActionResult containing the comment with the specified ID, if found.
        [HttpGet("/comment/{id}")]
        public IActionResult GetCommentById(int id)
        {
            try
            {
                var comment = commentDao.GetCommentById(id);
                if (comment == null)
                {
                    return NotFound($"No comment found with ID {id}.");
                }

                return Ok(comment);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve comment: {e.Message}");
            }
        }

        [HttpGet("/comment/posts/{id}")]
        public IActionResult GetCommentsByPostId(int id)
        {
            try
            {
                var comments = commentDao.GetCommentsByPostId(id);
                if (comments == null)
                {
                    return NotFound($"No comments found with Post ID {id}.");
                }

                return Ok(comments);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve comments with Post Id {id}: {e.Message}");
            }
        }

        // Creates a new comment.
        //
        // Parameters:
        //   comment: The comment object containing the details of the comment to be created.
        //
        // Returns:
        //     An IActionResult representing the HTTP response for the create comment operation.
        //   If the comment is created successfully, it returns a CreatedAtAction result with the created comment object.
        //   If an error occurs during the creation of the comment, it returns a StatusCode result with a 500 status code
        //   and an error message indicating the failure.
        [HttpPost("/comment")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult CreateComment(Comment comment)
        {
            try
            {
                var createdComment = commentDao.CreateComment(comment);
                return CreatedAtAction(nameof(GetCommentById), new { id = createdComment.CommentID }, createdComment);
            }
            catch (DaoException e) 
            {
                return StatusCode(500, $"Failed to create comment: {e.Message}");
            }
        }

        // Updates a comment with the specified ID.
        //
        // Parameters:
        //   id: The ID of the comment to update.
        //   comment: The updated comment object.
        //
        // Returns: An IActionResult representing the result of the update operation.
        [HttpPut("/comment/{id}")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult UpdateComment(int id, Comment comment)
        {
            if (id != comment.CommentID)
            {
                return BadRequest("Comment ID mismatch");
            }
            try
            {
                var updatedComment = commentDao.UpdateComment(comment);
                if (updatedComment == null)
                {
                    return NotFound($"No comment found.");
                }
                return Ok(updatedComment);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to update comment: {e.Message}");
            }
        }

        // Deletes a comment with the specified ID.
        //
        // Parameters:
        //   id: The ID of the comment to delete.
        //
        // Returns:
        //   An IActionResult representing the result of the deletion operation.
        [HttpDelete("/comment/{id}")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult DeleteComment(int id)
        {
            try
            {
                var deletedComment = commentDao.DeleteComment(id);
                if (deletedComment == null)
                {
                    return NotFound($"No comment found.");
                }
                return Ok(deletedComment);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to delete comment: {e.Message}");
            }
        }
    }
}