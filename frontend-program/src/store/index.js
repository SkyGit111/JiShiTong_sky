import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

const store = new Vuex.Store({
  state: {
    user: {
      token: localStorage.getItem('token') || '',
      userid: localStorage.getItem('userid') || null,
      username: localStorage.getItem('username') || '',
      userrole: localStorage.getItem('userrole') || ''
    }
  },
  mutations: {
    SET_USER (state, userData) {
      state.user.userid = userData.user_id
      state.user.token = userData.token
      state.user.username = userData.user_name
      state.user.userrole = userData.user_role
      localStorage.setItem('token', userData.token)
      localStorage.setItem('userid', userData.user_id)
      localStorage.setItem('username', userData.user_name)
      localStorage.setItem('userrole', userData.user_role)
      console.log('SET_USER' + state.user.token)
    },
    LOGOUT (state) {
      state.user.userid = null
      state.user.token = ''
      state.user.username = ''
      state.user.userrole = ''
      localStorage.removeItem('token')
      localStorage.removeItem('userid')
      localStorage.removeItem('username')
      localStorage.removeItem('userrole')
    }
  }
})

export default store
