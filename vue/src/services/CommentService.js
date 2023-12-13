import axios from 'axios';

export default {

    getComments() {
        const apiResponse = axios.get('https://localhost:44315/comment');
        console.log("test");
        console.log(apiResponse);
        return apiResponse;
    },
    createComment(comment){
        return axios.post('/comment', comment);
    },
    editComment(comment){
        return axios.put('/comment:${id}', comment);
    },
    deletePost(comment){
        return axios.delete('/comment:${id}',comment);
    }
}