import axios from 'axios';

// TODO: figure out what the path needs to be
export default {
    GetAllPostVotesbyId(targetID){
        return axios.get(`/vote/post/${targetID}`)
    },
    GetPostVoteByID(targetID, userId) {
        return axios.get(`/vote/post/${targetID}/user/${userId}`)
    },
    GetAllCommentVotesById(targetID){
        return axios.get(`/vote/comment/${targetID}`)
    },
    CreatePostVote(vote){
        return axios.post('/vote/post',vote)
    },
    CreateCommentVote(vote){
        return axios.post('/vote/comment', vote)
    },
    UpdateCommentVote(targetID, vote){
        return axios.put(`/vote/comment/${targetID}`, vote)
    },
    UpdatePostVote(targetID, vote){
        return axios.put(`/vote/comment/${targetID}`, vote)
    },
    DeletePostVote(targetID, UserID){
        return axios.delete(`/vote/post/${targetID}/user/${UserID}`)
    },
    DeleteCommentVote(targetID, userID){
        return axios.delete(`/vote/comment/${targetID}/user/${userID}`)
    }
}