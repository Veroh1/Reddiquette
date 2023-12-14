<template>
  <v-content>
    <v-navigation-drawer app permanent location="right" class="pa-4" >

      

      <v-list-item class="pt-6">
          <h3>Browse Forums</h3>
      </v-list-item>
      <hr style="border-top: 1px solid #ff4500">      <v-list>
        <v-list-item
          v-for="forum in activeForums"
          :key="forum.forumId"
          v-bind:forums="forum"
          link
        ><router-link :to="{ name: 'forum-view', params: { id: forum.forumId } }">
          <v-list-item-content>
            <v-list-item-title class="text-decoration-none">{{ forum.forumTitle }}</v-list-item-title>
          </v-list-item-content>
        </router-link></v-list-item>
        
      </v-list>

    </v-navigation-drawer>
  </v-content>

  <v-content>
    <div>

    </div>
  </v-content>
</template>
<script>
// import ForumService from '../services/ForumService';

export default {
  props: ['forums'],
  components: {
  },
  // created() {
  //   ForumService.getForums().then(response => {
  //   this.$store.commit("SET_FORUMS", response.data); 
  //   }) 
  // },
  computed:
  {
    activeForums() {
      const allForums = this.$store.state.forums;
      const allPosts = this.$store.state.posts;
      const forumIdsWithRecentPosts = Array.from(new Set(allPosts.map(post => post.forumId)));
      const activeForums = allForums
        .filter(forum => forumIdsWithRecentPosts.includes(forum.forumId))
        .sort((a, b) => {return new Date(b.dateCreated) - new Date(a.dateCreated)[0]})
          return activeForums;
      },
  }
}
</script>
<style>
.content-title {
  margin-top: 50px;
}
a {
  text-decoration: none;
}
</style>