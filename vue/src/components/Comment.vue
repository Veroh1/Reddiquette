<template>
    <v-card value="rounded" >
        <v-card-title class="text-h6 my-1 post-title">
            <v-sheet color="grey-lighten-1" class="rounded-lg" >
                <p class="px-3">
                    {{ getPostTitle(comment.postID) }}
                </p>
            </v-sheet>
        </v-card-title>
        <v-card-subtitle >Posted by: {{ getUserName(comment.userID) }} | Date Posted: {{ timePassed }} </v-card-subtitle>
        <v-card-text> 
            <v-sheet color="grey-lighten-2" class="comment-content rounded-shaped">
                <p class="px-4">
                    {{ comment.commentContent }} 
                </p>
            </v-sheet>
        </v-card-text>
    </v-card>
    <br>
</template>

<script>
import CommentService from '../services/CommentService'
import PostService from '../services/PostService';
import ForumService from '../services/ForumService';

export default {
    props: ["comment", "post"],
    data() {
        return {
            comments: null,
        };
    },
    methods: {
        getPostTitle() {
            const postId = this.comment.postID;
            const post = this.$store.state.posts.find((post) => post.postID === postId);
            return post ? post.postTitle : 'Post Not Found';
        },
        getUserName() {
            const userId = this.comment.userID;
            const user = this.$store.state.postedUsers.find((user) => user.userId === userId);
            return user ? user.userName : 'User Name Not Found';
        },
        getForumTitle() {
            const forumId = this.comment.forumID;
            const forum = this.$store.state.forums.find((forum) => forum.forumID === forumId);
            return forum ? forum.forumTitle : 'Forum Not Found';
        },
    },
    computed: {
        timePassed() {
            const postedTime = new Date(this.comment.dateCreated);
            let currentTime = new Date();
            let differenceInTime = (currentTime - postedTime) / 1000;
            if (Math.round(differenceInTime) === 1) { return "1 second ago"; }
            else if (Math.round(differenceInTime / 60) < 1) { return `${differenceInTime} seconds ago`; }
            else if (Math.round(differenceInTime / 60) == 1) { return "1 minute ago"; }
            else if (Math.round(differenceInTime / (60 * 60)) == 1) { return "1 hour ago"; }
            else if (Math.round(differenceInTime / (60 * 60 * 24)) == 1) { return "1 day ago"; }
            else if (Math.round(differenceInTime / (60 * 60 * 24 * 30)) == 1) { return "1 month ago"; }
            else if (Math.round(differenceInTime / (60 * 60 * 24 * 365)) == 1) { return "1 year ago"; }
            else { return `${Math.round(differenceInTime / (60 * 60 * 24 * 365))} years ago`; }
        },
    },
}
</script>

<style>
.forum-title {
    height: 30px;
}

.post-title {
    height: 20px;
}

.username {
    height: 20px;
}

.date-created {
    height: 20px;
}

.comment-content {
    min-height: 10%;
}
</style>