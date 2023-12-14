<template>
  <v-content class="post-section">
    <v-sheet class="citation">
      <v-text class="text-h4">{{ getForumTitle }}</v-text>
      <v-row dense class="mx-auto">
        <v-col md="4">
          <div class="my-4 text-subtitle-2">Posted by: {{ getUserName }}</div>
        </v-col>
        <v-col md="4">
          <div class="my-4 text-subtitle-2">Posted about {{ timePassed }}</div>
        </v-col>
        <v-col md="4">
          <div class="my-4 text-subtitle-2">Accumulated Clout: {{ getClout }}</div>
        
        </v-col>
      </v-row>
    </v-sheet>
    <v-card class="postDetails">
      <v-container>
        <div class="d-flex flex-column">
          <v-card-title flat class="d-flex justify-center">{{ post.postTitle }}</v-card-title>
          <div class="d-flex">
            <v-img class="float-left" :width="300" :height="200" aspect-ratio="16/9" cover :src="post.imagePath"></v-img>
            <v-card-text class="d-flex w-66 ">{{ post.postContent }} </v-card-text>
          </div>
        </div>
        <div class="comments-section">
          <v-textarea v-model="comment.commentContent" color="#FF4500" label="Post content" class="post-content"></v-textarea>
          <v-chip-group class="ma-2">
            <v-chip class="green" label size="small" @click="upVote">
              <i class="fa-solid fa-up-long pr-2"></i>{{ this.storedUpvotes }} Upvotes
            </v-chip>

            <v-chip class="red" label size="small" @click="downVote">
              <i class="fa-solid fa-down-long pr-2"></i>{{ this.storedDownvotes }} Downvotes
            </v-chip>
          </v-chip-group>

        </div>
        <v-snackbar v-model="voteSnackbar" :timeout="timeout">
          {{ text }}

          <template v-slot:actions>
            <v-btn color="blue" variant="text" @click="voteSnackbar = false">
              Close
            </v-btn>
          </template>
        </v-snackbar>
      </v-container>
      <div v-if="comments" class="comments">
        <Comment  v-for="(comment, index) in comments" :key="index" :comment="comment" />
      </div>
    </v-card>
    <!-- <v-divider :thickness="4" color="info"></v-divider> -->
    <div class="d-flex w-66 pa-5 ml-10 comment-button ">
      <v-btn @click="createComment" block size="x-large">Make a Comment</v-btn>
    </div>
  </v-content>
</template>
    
<script>

import VoteService from '../services/VoteService';
import CommentService from '../services/CommentService';
import Comment from '../components/Comment.vue';

export default {
  props: ["post", "reply"],
  components: {
   Comment
  },
  data() {

    const currentDate = new Date();
    return {
      comments: null,
      comment: {
        userID: this.$store.state.user.id,
        commentContent: '',
        dateCreated: currentDate.toISOString(),
        // forumID: this.post.forumId,
        // forumID: this.post.forumId,
        postID: this.post.postID,
      },
      voteSnackbar: false,
      text: "You must be logged in to vote.",
      timeout: 2000,
      posts: '',
      isUpvoted: false,
      isDownvoted: false,
      storedUpvotes: 0,
      storedDownvotes: 0,
      postingErrors: false,
      postingErrorMsg: 'There were problems creating this comment'
    };
  },
  methods: {
    async createComment() {
      try {
        this.comment.forumID = this.posts.id;
        this.comment.UserId = this.$store.state.user.userId;

        const response = await CommentService.createComment(this.comment);
        if (response.status >= 200 && response.status < 300) {
          this.$router.push({
            path: `/`,
            query: { posted: 'success' },
          });
        } else {
          // Handle unexpected response status
          console.error('Unexpected response status:', response.status);
        }
      } catch (error) {
        this.postingErrors = true;
        const response = error.response;
        if (response && response.status === 400) {
          this.postingErrorMsg = 'Bad Request: Validation Errors';
        } else {
          // Handle other errors
          console.error('Error creating post:', error);
        }
      }
    },
    updateCommentContent(content) {
      this.comment.commentContent = content;
    },
    async fetchComments(postID) {
      try {
        const response = await CommentService.getComments(postID);
        this.comments = response.data.filter(comment => comment.postID === postID);
      } catch (error) {
        console.error('Error fetching comments:', error);
      }
    },
    // getReply(postId){
    //   const reply = this.$store.state.Reply.find((reply) => reply.postId === postId);
    //   return reply ? reply.body : 'No Comments Yet!';
    // },
    updateVotes() {
      VoteService.GetAllPostVotesbyId(this.post.postID)
        .then(response => {
          this.storedUpvotes = response.data.Upvotes;
          this.storedDownvotes = response.data.Downvotes
        });
    },
    async upVote() {
      if (this.$store.state.isAuthenticated) {
        if (this.isUpvoted) {
          const response = await VoteService.DeletePostVote(this.$route.params.id, this.$store.state.user.userId,)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isUpvoted = false;
          }
        }
        else if (this.isDownvoted) {
          const vote = {
            UserID: this.$store.state.user.userId,
            TargetID: this.$route.params.id,
            Increment: 1
          }
          const response = await VoteService.UpdatePostVote(vote.TargetID,vote)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isDownvoted = false;
            this.isUpvoted = true;
          }
        }
        else {
          const vote = {
            UserID: this.$store.state.user.userId,
            TargetID: this.$route.params.id,
            Increment: 1
          }
          const response = await VoteService.CreatePostVote(vote)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isUpvoted = true;
          }
          //TODO: write catch eventually
        }
      }
      else {this.voteSnackbar = true;}
    },
    async downVote() {
      if (this.$store.state.isAuthenticated) {
        if (this.isDownvoted) {
          const response = await VoteService.DeletePostVote(this.$route.params.id, this.$store.state.user.userId,)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isUpvoted = false;
          }
        }
        else if (this.isUpvoted) {
          const vote = {
            UserID: this.$store.state.user.userId,
            TargetID: this.$route.params.id,
            Increment: -1
          }
          const response = await VoteService.UpdatePostVote(vote.TargetID,vote)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isDownvoted = true;
            this.isUpvoted = false;
          }
        }
        else {
          const vote = {
            userId: this.$store.state.user.userId,
            targetID: this.$route.params.id,
            increment: -1
          }
          const response = await VoteService.CreatePostVote(vote)
          if (response.status >= 200 && response.status < 300) {
            this.updateVotes();
            this.isDownvoted = true;
          }
          //TODO: write catch eventually
        }
      }
      else {this.voteSnackbar = true;}
    },
  },
  computed: {
    timePassed() {
      const postedTime = new Date(this.post.dateCreated);
      let currentTime = new Date();
      let differenceInTime = (currentTime - postedTime) / 1000;
      if (Math.round(differenceInTime) === 1) { return "1 second ago" }
      else if (Math.round(differenceInTime / 60) < 1) { return `${differenceInTime} seconds ago` }
      else if (Math.round(differenceInTime / 60) == 1) { return "1 minute ago" }
      else if (Math.round(differenceInTime / 60) < 60) { return `${Math.round(differenceInTime / 60)} minutes ago` }
      else if (Math.round(differenceInTime / (60 * 60)) == 1) { return "1 hour ago" }
      else if (Math.round(differenceInTime / (60 * 60)) < 24) { return `${Math.round(differenceInTime / (3600))} hours ago` }
      else if (Math.round(differenceInTime / (60 * 60 * 24)) == 1) { return "1 day ago" }
      else if (Math.round(differenceInTime / (60 * 60 * 24) < 30)) { return `${Math.round(differenceInTime / (60 * 60 * 24))} days ago` }
      else if (Math.round(differenceInTime / (60 * 60 * 24 * 30) == 1)) { return "1 month ago" }
      else if (Math.round(differenceInTime / (60 * 60 * 24 * 30) < 12)) { return `${Math.round(differenceInTime / (60 * 60 * 24 * 30))} months ago` }
      else if (Math.round(differenceInTime / (60 * 60 * 24 * 365) == 1)) { return "1 year ago" }
      else return `${Math.round(differenceInTime / (60 * 60 * 24 * 365))} years ago`
    },
    getForumTitle() {
      const forumId = this.post.forumId;
      const forum = this.$store.state.forums.find((forum) => forum.forumId === forumId);
      return forum ? forum.forumTitle : 'Forum Not Found';
    },
    getUserName() {
      const userId = this.post.userID;
      const user = this.$store.state.postedUsers.find((user) => user.userId === userId);
      return user ? user.userName : 'User Name Not Found';
    },
    getUpvotes() {
      return this.storedUpvotes;
    },
    getDownvotes() {
      return this.storedDownvotes;
    },
    getClout() {
      const clout = this.storedUpvotes - this.storedDownvotes;
      return clout;
    },
  },
  created() {
    this.fetchComments(this.post.postID);
    VoteService.GetAllPostVotesbyId(this.$route.params.id)
      .then(response => {
        this.storedUpvotes = response.data.upvotes;
        this.storedDownvotes = response.data.downvotes;
      });
    if (this.$store.state.isAuthenticated) {
      VoteService.GetPostVoteByID(this.$route.params.id, this.$store.state.user.userId)
        .then(response => {
          if (response.data.increment === 1) { this.isUpvoted = true; }
          else if (response.data.increment === -1) { this.isDownvoted = true; }
        })
    }
  },
  actions: {
    upVotePost() {
      this.upVote();
    },
    downVotePost() {
      this.downVote();
    }
  },
}

</script>
    
<style>
.post-content {
  width: 80%;
}

.comments-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  margin-top: 20px;

  /* Adjust as needed */
}

/* .citation {
  margin: 2.5% auto auto auto;
  padding-left: 2%;
  padding-right: 33%;
}
.postDetails {
  margin-top: 2.5%;
}
#textArea {
  background-color: rgb(222, 222, 222);
} */
</style>