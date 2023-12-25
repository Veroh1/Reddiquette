import { createStore as _createStore } from 'vuex';
import axios from 'axios';
import AuthService from '../services/AuthService';
import ForumService from '../services/ForumService';
import PostService from '../services/PostService';
import CommentService from '../services/CommentService';

export function createStore(currentToken, currentUser, currentForum) {
  let store = _createStore({

    state: {
      posts: [],
      comments: [],
      forums: [],
      token: currentToken || '',
      user: currentUser || {},
      isAuthenticated: !!currentToken,
      postedUsers: [],
    },
    mutations: {
      SET_AUTH_TOKEN(state, token) {
        state.token = token;
        state.isAuthenticated = !!token;
        localStorage.setItem('token', token);
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
      },
      SET_USER(state, user) {
        state.user = user;
        localStorage.setItem('user', JSON.stringify(user));
      },
      LOGOUT(state) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        state.token = '';
        state.user = {};
        state.isAuthenticated = false;
        axios.defaults.headers.common = {};
      },
      SET_FORUMS(state, forums) {
        state.forums = forums;
      },
      SET_POSTS(state, posts) {
        state.posts = posts;
      },
      SET_COMMENTS(state, comments) {
        state.comments = comments;
      },
      SET_USERS(state, listedUsers) {
        state.postedUsers = listedUsers;
      }

    },

    actions: {
      async login({ commit }, user) {
        try {
          const response = await AuthService.login(user);
          // Handle successful login (commit mutations, set state, etc.)
          commit('SET_AUTH_TOKEN', response.token);
          commit('SET_USER', response.user);
        } catch (error) {
          // Handle login failure
          throw new Error('Login failed');
        }
      },
      // fetchForums(context) {
      //   // Use ForumService to call api

      //   ForumService.getForums().then(response => {
      //     context.commit("SET_FORUMS", response.data);
      //   });
      // }
    }
  });
  return store;
}
