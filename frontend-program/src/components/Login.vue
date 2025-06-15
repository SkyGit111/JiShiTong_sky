<template>
  <div class="login-page">

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="background">
      <img src="@/assets/background.jpg" alt="background" />
    </div>

    <div class="login-container">
      <div class="logo-container">
        <img src="@/assets/defaultavatar.jpg" alt="Logo" class="logo-image" />
      </div>

      <h2>登录到济事通拼单平台</h2>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <input
            type="text"
            id="userid"
            v-model="userid"
            placeholder="输入您的账号"
            required
          />
        </div>
        <div class="form-group">
          <input
            type="password"
            id="password"
            v-model="password"
            placeholder="输入您的密码"
            required
          />
        </div>
        <button type="submit" class="login-button">登录</button>
      </form>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

      <div class="button-container">
        <button @click="goToRegisterPage" class="register-button">注册</button>
      </div>
    </div>

    <div class="footer-text">
      济事通，即时通 - 本平台由Vue和ASP.NET驱动
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import serverURL from '../server.config'
import {Message} from 'element-ui'

export default {
  data () {
    return {
      userid: '',
      password: '',
      errorMessage: '',
      currentDate: this.getCurrentDate() // 初始化时获取当前日期
    }
  },
  mounted () {
    // 每秒更新一次时钟
    setInterval(() => {
      this.currentDate = this.getCurrentDate()
    }, 1000)
  },
  methods: {
    async handleLogin () {
      try {
        Message({
          type: 'info',
          message: '登录中，请稍候',
          duration: 2000
        })
        const loginResponse = await axios.post(serverURL + '/v1/user/login', {
          userId: parseInt(this.userid, 10),
          userPassword: this.password
        }, {
          headers: {
            'Content-Type': 'application/json'
          }
        })
        if (loginResponse.status === 200) {
          this.errorMessage = '登录成功'
          Message(
            {
              message: '登录成功',
              type: 'success',
              duration: 1000
            }
          )
          this.$store.commit('SET_USER', loginResponse.data)
          this.$router.push({name: 'Home', params: {userid: this.userid}})
        }
      } catch (error) {
        if (error.response.status === 400) {
          Message({
            type: 'error',
            message: '登录信息有误',
            duration: 2000
          })
          this.errorMessage = '您输入的登录信息有误,请重试!'
        } else {
          Message(
            {
              message: '服务器错误，请等待修复后再试，或联系管理员',
              type: 'error'
            }
          )
          this.errorMessage = null
        }
      }
    },
    goToRegisterPage () {
      this.$router.push({ name: 'Register' }) // 跳转到注册页面
    },
    getCurrentDate () {
      const now = new Date()
      const year = now.getFullYear()
      const month = String(now.getMonth() + 1).padStart(2, '0')
      const week = ['日', '一', '二', '三', '四', '五', '六'][now.getDay()]
      const day = String(now.getDate()).padStart(2, '0')
      const hours = String(now.getHours()).padStart(2, '0')
      const minutes = String(now.getMinutes()).padStart(2, '0')
      const seconds = String(now.getSeconds()).padStart(2, '0')

      return `现在是 ${year}年${month}月${day}日星期${week} ${hours}:${minutes}:${seconds}`
    }
  }
}
</script>

<style scoped>
.clock {
  position: absolute;
  top: 35px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 18px;
  color: white;
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.5);
}

.login-page {
  display: flex;
  height: 98vh;
  width: 99vw;
  justify-content: center;
  align-items: center;
  position: relative;
}

.background {
  position: absolute; /* 背景绝对定位，覆盖整个页面 */
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: -1; /* 背景位于最底层 */
}

.background img {
  width: 100%; /* 背景图宽度占满容器 */
  height: 100%; /* 背景图高度占满容器 */
  object-fit: cover; /* 保持比例，覆盖区域 */
  object-position: center; /* 背景图居中 */
}

.logo-container {
  display: flex;
  justify-content: center;
  margin-bottom: 20px; /* 为logo和标题之间添加间距 */
}

.logo-image {
  width: 95px;
  height: 95px;
  border-radius: 50%; /* 圆形 */
  object-fit: cover; /* 保持比例，确保图片不变形 */
  border: 0 solid #333; /* 圆形边框 */
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5); /* 添加阴影 */
}

.login-container {
  max-width: 400px;
  width: 100%;
  padding: 40px;
  border-radius: 10px;
  background-color: rgba(255, 255, 255, 0); /* 设置为完全透明 */
}

h2 {
  text-align: center;
  font-size: 24px;
  margin-bottom: 60px;
  color: #ddd;
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.15);
}

.form-group {
  display: flex;
  flex-direction: column;
  margin-bottom: 15px;
}

label {
  text-align: left;
  display: block;
  color: white;
  margin-bottom: 5px;
  font-size: 14px;
}

input::placeholder {
  color: #444; /* 设置为你想要的颜色，例如淡灰色 */
  font: normal 14px/1.5 "MiSans", sans-serif;
}

input {
  width: 100%; /* 确保宽度为100% */
  padding: 10px;
  font-size: 14px;
  border: 1px solid #666;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
  border-radius: 5px;
  box-sizing: border-box; /* 确保包含 padding 和 border 在宽度内 */
  background-color: rgba(255, 255, 255, 0.3); /* 半透明背景 */
  color: #444; /* 输入文本颜色 */
}

.login-button {
  width: 100%;
  padding: 10px;
  font-size: 16px;
  background-image: linear-gradient(45deg, #0256ed, #b20242);
  color: #ddd;
  border: 1px solid #666;
  border-radius: 5px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
  cursor: pointer;
  box-sizing: border-box;
  background-size: 200% 200%; /* 设置背景大小以便能有渐变的移动效果 */
  transition: background-position 0.4s ease; /* 只针对 background-position 设置过渡 */
}

.login-button:hover {
  background-position: 100% 100%; /* 悬停时改变背景位置 */
}

.register-button {
  width: 100%;
  padding: 10px;
  font-size: 16px;
  background-color: #555;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
  color: #ddd;
  border: 1px solid #666;
  border-radius: 5px;
  cursor: pointer;
  box-sizing: border-box;
  transition: background-color 0.4s ease;
}

.register-button:hover {
  background-color: #666666;
}

.error-message {
  color: red;
  font-size: 14px;
  text-align: center;
}

.button-container {
  display: flex;
  justify-content: space-between;
  margin-top: 10px;
}

/* 底部文字样式 */
.footer-text {
  position: absolute;
  bottom: 35px;
  width: 100%;
  text-align: center;
  font-size: 18px;
  color: white;
  background-color: transparent; /* 设置透明背景 */
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
}

</style>
