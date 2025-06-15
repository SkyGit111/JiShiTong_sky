import Router from 'vue-router'
import Home from '@/components/Home'
import Login from '@/components/Login'
import Register from '@/components/Register'
import AccountManagement from '@/components/AccountManagement'
import CreateRequest from '@/components/CreateRequest'
import ViewRequest from '@/components/ViewRequest'
import ManageRequest from '@/components/ManageRequest'
import AIAnalysis from '@/components/AIAnalysis'
import ViewMessage from '@/components/ViewMessage'
import Vue from 'vue'
import axios from 'axios'
import serverURL from '../server.config'
import store from '../store'
import { MessageBox, Message } from 'element-ui'

Vue.use(Router)

const router = new Router({
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: Login,
      meta: { requiresAuth: false }
    },
    {
      path: '/register',
      name: 'Register',
      component: Register,
      meta: { requiresAuth: false }
    },
    {
      path: '/createrequest',
      name: 'CreateRequest',
      component: CreateRequest,
      meta: { requiresAuth: true }
    },
    {
      path: '/viewrequest',
      name: 'ViewRequest',
      component: ViewRequest,
      meta: { requiresAuth: true }
    },
    {
      path: '/managerequest',
      name: 'ManageRequest',
      component: ManageRequest,
      meta: { requiresAuth: true }
    },
    {
      path: '/aianalysis',
      name: 'AIAnalysis',
      component: AIAnalysis,
      meta: { requiresAuth: true }
    },
    {
      path: '/viewmessage',
      name: 'ViewMessage',
      component: ViewMessage,
      meta: { requiresAuth: true }
    },
    {
      path: '/',
      name: 'Home',
      component: Home,
      meta: { requiresAuth: true }
    },
    {
      path: '/accountmanagement',
      name: 'AccountManagement',
      component: AccountManagement,
      meta: { requiresAuth: true }
    }
  ]
})

async function validateToken (token) {
  try {
    const response = await axios.post(serverURL + '/v1/user/check', { }, {
      headers: { Authorization: `Bearer ${token}` }
    })
    return response.status === 200
  } catch (error) {
    return false
  }
}

function tokenGuardBeforeOperation (token) {
  if (!validateToken(token)) {
    MessageBox.alert('抱歉，您的登录状态已超时，请重新登录。', '提示:', {
      confirmButtonText: '好的',
      callback: () => {
        Message({
          type: 'info',
          message: '请您重新登录',
          duration: 2000
        })
        store.dispatch('logout')
        router.push('/login')
      }
    })
  }
}

router.beforeEach((to, from, next) => {
  const token = store.state.user.token
  if (to.matched.some(record => record.meta.requiresAuth === true)) {
    if (token) {
      const isValid = validateToken(token)
      if (isValid) {
        next()
      } else {
        MessageBox.alert('抱歉，您的登录状态已超时，请重新登录。', '提示:', {
          confirmButtonText: '好的',
          callback: () => {
            Message({
              type: 'info',
              message: '请您重新登录',
              duration: 2000
            })
            store.dispatch('logout')
            next('/login')
          }
        })
      }
    } else {
      MessageBox.alert('抱歉，您尚未登录，请先登录。', '提示:', {
        confirmButtonText: '好的',
        callback: () => {
          Message({
            type: 'info',
            message: '请您登录',
            duration: 2000
          })
          next('/login')
        }
      })
    }
  } else {
    next()
  }
})

export default router
export { validateToken }
export { tokenGuardBeforeOperation }
