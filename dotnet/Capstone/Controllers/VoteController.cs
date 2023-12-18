using Capstone.DAO;
using Capstone.Exceptions;
using Capstone.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;



namespace Capstone.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVotesDao votesDao;
        public VoteController(IVotesDao votesDao)
        {
            this.votesDao = votesDao;
        }
        /// <summary>
        /// Takes in a post id and returns an object containing the upvotes and downvotes
        /// </summary>
        /// <param name="targetId">postID</param>
        /// <returns></returns>
        [HttpGet("/vote/post/{targetID}")]
        public IActionResult GetAllPostVotesById(int targetId)
        {
            try
            {
                var votes = votesDao.GetAllPostVotesById(targetId);
                if (votes == null)
                {
                    return NotFound($"No forum found with ID {targetId}.");
                }
                return Ok(votes);
            }
            catch(DaoException e)
            {
                return StatusCode(404, $"Failed to retrieve votes: {e.Message}");
            }
        }
        /// <summary>
        /// Takes in a comment id and returns an object containing the upvotes and downvotes
        /// </summary>
        /// <param name="targetId">commentID</param>
        /// <returns></returns>
        [HttpGet("/vote/comment/{targetID}")]
        public IActionResult GetAllCommentVotesById(int targetID)
        {
            try
            {
                var votes = votesDao.GetAllCommentVotesById(targetID);
                if (votes == null)
                {
                    return NotFound($"No forum found with ID {targetID}.");
                }
                return Ok(votes);
            }
            catch (DaoException e)
            {
                return StatusCode(404, $"Failed to retrieve votes: {e.Message}");
            }
        }

        [HttpGet("/vote/post/{postId}/user/{userId}")]
        public IActionResult GetPostVoteByID(int postId, int userId)
        {
            try
            {
                var voteHistory = votesDao.GetPostVoteById(postId, userId);
                return Ok(voteHistory);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve vote history: {e.Message}");
            }
        }

        [HttpGet("/vote/comment/{commentId}/user/{userId}")]
        public IActionResult GetCommentVoteByID(int userId, int commentId)
        {
            try
            {
                var voteHistory = votesDao.GetCommentVoteById(userId, commentId);
                return Ok(voteHistory);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to retrieve vote history: {e.Message}");
            }
        }

        [HttpPost("/vote/post")]
        public IActionResult CreatePostVote(Vote vote)
        {
            try
            {
                var userId = vote.UserID;
                var targetID = vote.TargetID;
                var increment = vote.Increment;
                var createdVote = votesDao.CreatePostVote(userId, targetID, increment);
                return Ok(createdVote);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to create vote: {e.Message}");
            }
            
        }

        [HttpPost("/vote/comment")]
        public Vote CreateCommentVote(Vote vote)
        {
            var userId = vote.UserID;
            var targetID = vote.TargetID;
            var increment = vote.Increment;
            var createdVote = votesDao.CreateCommentVote(userId, targetID, increment);
            return createdVote;
        }

        [HttpPut("/vote/comment/{targetID}")]
        public IActionResult UpdateCommentVote(Vote vote)
        {
            try
            {
                var userId = vote.UserID;
                var targetID = vote.TargetID;
                var increment = vote.Increment;
                var updatedVote = votesDao.UpdateCommentVote(userId, targetID, increment);
                return Ok(updatedVote);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to update vote: {e.Message}");
            }

        }

        [HttpPut("/vote/post/{targetID}")]
        public IActionResult UpdatePostVote(Vote vote)
        {
            try
            {
                var userId = vote.UserID;
                var targetID = vote.TargetID;
                var increment = vote.Increment;
                var updatedVote = votesDao.UpdatePostVote(userId, targetID, increment);
                return Ok(updatedVote);
            }
            catch (DaoException e)
            {
                return StatusCode(500, $"Failed to update vote: {e.Message}");
            }
        }

        [HttpDelete("/vote/post/{targetID}/user/{userID}")]
        public Vote DeletePostVote(int targetID, int userID)
        {
            var deletedVote = votesDao.DeletePostVote(userID, targetID);
            return deletedVote;

        }

        [HttpDelete("/vote/comment/{targetID}/user/{userID}")]
        public Vote DeleteCommentVote(int targetID, int userID)
        {
            var deletedVote = votesDao.DeleteCommentVote(userID, targetID);
            return deletedVote;

        }
    }

}
