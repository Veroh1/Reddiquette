<template>
  <v-app class="grey lighten-2">
    
    <v-app-bar app class="px-3 elevation-4" color="grey-lighten-4" height="72">
      
      <v-app-bar-nav-icon>
        <router-link to="/">
          <v-avatar size="48">
            <v-img src="https://i.imgur.com/bek3ix4.png" width="100" height="100" />
          </v-avatar>
        </router-link>
      </v-app-bar-nav-icon>

      <v-app-bar-title class="font-weight-bold" :style="{ color: '#ff4500' }">ReddiQuette</v-app-bar-title>
      
      <create-post-input />
      <v-spacer></v-spacer>
      <v-btn href="/" class="mx-5 px-3">
        Home
      </v-btn>
      <v-btn
      v-if="isLoggedIn"
      color="primary"
      class="mx-5 px-3"
      :prepend-icon="displayAvatar"
    >
        <v-avatar>
          <img :src="displayAvatar">
        </v-avatar>
        <v-spacer></v-spacer>
        <v-list-item>{{ displayedUsername }}</v-list-item>
      

      <v-menu activator="parent">
        <v-list>
          <v-list-item><router-link to="/MyProfile">My Profile</router-link></v-list-item>
          <v-list-item>New Forum</v-list-item>
          <v-list-item>Settings</v-list-item>
          <v-list-item><router-link to="/logout">Log Out</router-link></v-list-item>
        </v-list>
      </v-menu>
    </v-btn>
    <v-btn
    v-else
    >
    <router-link to="/login" >Log in</router-link>
    </v-btn>

      <!-- <v-list-item :prepend-avatar="displayAvatar">
        <v-list-item-content>
          <v-list-item-title>{{ displayedUsername }}</v-list-item-title>
          <v-list-item-subtitle>
            <router-link v-if="isLoggedIn" to="/logout" class="text-subtitle-2">Log out</router-link>
            <router-link v-else to="/login" class="text-subtitle-2">Log in</router-link>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item> -->
    </v-app-bar>

    <router-view />
  </v-app>
</template>

<script>
import ForumService from '../src/services/ForumService';
import PostService from '../src/services/PostService';
import CommentService from '../src/services/CommentService';
import CreatePostInput from '../src/components/CreatePostInput.vue'
import { VBtn } from 'vuetify/lib/components/index.mjs';

export default {
  created() {
    ForumService.getForums().then(response => {
    this.$store.commit("SET_FORUMS", response.data);
    }  
    );
    PostService.getPosts().then(response =>{
    this.$store.commit("SET_POSTS", response.data);
    }  
    );
    CommentService.getComments().then(response =>{
    this.$store.commit("SET_COMMENTS", response.data);
    }  
    );
    // const script = document.createElement('script');
    // script.src = 'https://s3-us-west-2.amazonaws.com/kaboodle/kaboodle.js';
    // script.type = 'text/javascript';
    // document.body.appendChild(script);
  }, 
  name: 'App',
  components: { CreatePostInput, VBtn },
  data() {
    return {

    }
    
  },
  computed: {
    isLoggedIn() {
      return !!this.$store.state.token;
    },
    user() {
      return this.$store.state.user;
    },
    displayedUsername() {

      if (this.user && this.user.username) {
        return this.user.username;
      } else {
        return 'Anonymous';
      }
    },
    displayAvatar(){
      if (this.user && this.user.avatar) {
        return this.user.avatar;
      } else {
        return 'https://upload.wikimedia.org/wikipedia/commons/thumb/b/b5/Windows_10_Default_Profile_Picture.svg/1024px-Windows_10_Default_Profile_Picture.svg.png';
      }
    }
  }
}


</script>

<style>

div.nav{
  flex-flow: row;
}

input{
  align-items: center;
}
/* .global-padding {
  padding: 80px;
} */
#divider-color {
  color: #ff4500;
}

</style>

