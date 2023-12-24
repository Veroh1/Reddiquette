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
    public class ForumController : ControllerBase
    {
        private readonly IForumDao forumDao;

        // Initializes a new instance of the ForumController class.
        //
        // Parameters:
        //   forumDao: The IForumDao instance used to access forum data.
        //   mapper: The IMapper instance used for object mapping.
        public ForumController(IForumDao forumDao)
        {
            this.forumDao = forumDao;
        }

        // Retrieves all forums and returns them as a list of ForumDto objects.
        [HttpGet("/forum")]
        public IActionResult GetAllForums()
        {
            try
            {
                var forums = forumDao.GetAllForums();

                return Ok(forums);

            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve forums: {e.Message}");
            }

        }

        // Retrieves a forum by its ID.
        //
        // Parameters:
        //   id: The ID of the forum to retrieve.
        //
        // Returns:
        //   An IActionResult containing the forum with the specified ID, if found.
        [HttpGet("/forum/{id}")]
        public IActionResult GetForumById(int id)
        {
            try
            {
                var forum = forumDao.FindForumByID(id);
                if (forum == null)
                {
                    return NotFound($"No forum found with ID {id}.");
                }
                
                return Ok(forum);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to find forum by ID {id}: {e.Message}");
            }
        }

        // Creates a new forum.
        // 
        // Parameters:
        //   forum: The forum object containing the details of the forum to be created.
        //
        // Returns:
        //   An IActionResult object representing the HTTP response for the create forum operation.
        //   If the forum is created successfully, it returns a CreatedAtAction result with the created forum object.
        //   If an error occurs during the creation of the forum, it returns a StatusCode result with a 500 status code
        //   and an error message indicating the failure.
        [HttpPost]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult CreateForum(Forum forum)
        {
            try
            {
                var createdForum = forumDao.CreateForum(forum);
                return CreatedAtAction(nameof(GetForumById), new { id = createdForum.ForumId }, createdForum);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to create forum: {e.Message}");
            }
        }

        // Updates a forum with the specified ID.
        //
        // Parameters:
        //   id: The ID of the forum to update.
        //   forum: The updated forum object.
        //
        // Returns: 
        //   An IActionResult representing the result of the update operation.
        [HttpPut("/forum/{id}")]
        [Authorize(Roles = "admin, moderator, user")]
        public IActionResult UpdateForum(int id, Forum forum)
        {
            if (id != forum.ForumId)
            {
                return BadRequest("Forum ID mismatch");
            }
            try
            {
                var updatedForum = forumDao.UpdateForum(forum);
                if (updatedForum == null)
                {
                    return NotFound($"No forum found");
                }
                return Ok(updatedForum);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to update forum: {e.Message}");
            }
        }

        // Deletes a forum with the specified ID.
        // 
        // Parameters:
        //   id: The ID of the forum to delete.
        //
        // Returns:
        //   An IActionResult representing the result of the deletion operation.
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, moderator")]
        public IActionResult DeleteForumByID(int id)
        {
            try
            {
                var deletedForum = forumDao.DeleteForumByID(id);
                if (deletedForum == null)
                {
                    return NotFound($"No forum found.");
                }
                return Ok(deletedForum);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to delete forum: {e.Message}");
            }
        }
    }
}