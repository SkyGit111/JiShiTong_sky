<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>济事通</span>
    </div>

    <button type="reset" @click="logout" class="logout-button">退出</button>

    <div class="background">
      <img src="@/assets/background.jpg" alt="background" />
    </div>

    <div class="content-container" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <div class="logo-container">
        <img :src="avatarUrl" alt="Logo" class="logo-image" />
      </div>

      <h2>欢迎来到济事通拼单平台</h2>
    </div>

    <div class="button-container" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <button v-if="store.state.user.userrole === 'student'" @click="goToCreateRequest" class="nav-button nav-button-create">
        <span class="button-title">创建新的拼单</span>
        <img src="@/assets/createrequest.png" alt="Logo" class="create-image">
        <span class="tooltip-text">创建一个新的拼单请求，并发布出去，让其他用户选择。</span>
      </button>
      <button v-if="store.state.user.userrole === 'student'" @click="goToViewRequest" class="nav-button nav-button-view">
        <span class="button-title">查看开放拼单</span>
        <img src="@/assets/viewrequest.png" alt="Logo" class="create-image">
        <span class="tooltip-text">探索系统中所有开放的拼单请求，并加入您意向的拼单。</span>
      </button>
      <button v-if="store.state.user.userrole === 'student'" @click="goToManageRequest" class="nav-button nav-button-manage">
        <span class="button-title">管理个人拼单</span>
        <img src="@/assets/managerequest.png" alt="Logo" class="create-image">
        <span class="tooltip-text">管理个人加入或发起的拼单请求，可以退出、修改或删除。</span>
      </button>
      <button v-if="store.state.user.userrole === 'admin'" @click="goToManageRequest" class="nav-button nav-button-manage">
        <span class="button-title">管理系统拼单</span>
        <img src="@/assets/managerequest.png" alt="Logo" class="create-image">
        <span class="tooltip-text">管理系统内所有的拼单请求，可以进行修改或删除。</span>
      </button>
      <button v-if="store.state.user.userrole === 'student'" @click="goToViewMessage" class="nav-button nav-button-message">
        <span class="button-title">查看拼单消息</span>
        <img src="@/assets/viewmessage.png" alt="Logo" class="create-image">
        <span class="tooltip-text">随时随地，查看系统中有关您的拼单请求的相关动态。</span>
      </button>
      <button v-if="store.state.user.userrole === 'student'" @click="goToAIAnalysis" class="nav-button nav-button-ai">
        <span class="button-title">智能分析推荐</span>
        <img src="@/assets/ai.png" alt="Logo" class="create-image">
        <span class="tooltip-text">使用我们高效的AI工具，轻松查到满足您要求的拼单请求。</span>
      </button>
    </div>

    <div class="footer-text" :class="{ fadeIn: showContent, fadeOut: !showContent }">
       欢迎您，{{ username }}。济事通祝您一切顺利!<button @click="goToAccountManagement" class="account-button">账号管理</button>
    </div>
  </div>
</template>

<script>
import {Message, MessageBox} from 'element-ui'
import store from '../store'
import axios from 'axios'
import serverURL from '../server.config'

export default {
  computed: {
    store () {
      return store
    }
  },
  data () {
    return {
      currentDate: this.getCurrentDate(), // 初始化时获取当前日期
      userid: this.$store.state.user.userid || '未知用户',
      username: this.$store.state.user.username || '未知用户',
      showContent: false,
      avatarUrl: 'https://i.ibb.co/YcyZm2v/default.jpg'
    }
  },
  mounted () {
    // 每秒更新一次时钟
    setInterval(() => {
      this.currentDate = this.getCurrentDate()
    }, 1000)
    // 页面加载后延迟显示内容
    this.showContent = true
  },
  methods: {
    getCurrentDate () {
      const now = new Date()
      const month = String(now.getMonth() + 1).padStart(2, '0')
      const week = ['日', '一', '二', '三', '四', '五', '六'][now.getDay()]
      const day = String(now.getDate()).padStart(2, '0')
      const hours = String(now.getHours()).padStart(2, '0')
      const minutes = String(now.getMinutes()).padStart(2, '0')

      return `${month}月${day}日 星期${week} ${hours}:${minutes}`
    },
    logout () {
      MessageBox.confirm('您确定要退出济事通系统吗?', '提示:', {
        confirmButtonText: '是的',
        cancelButtonText: '取消',
        type: 'confirm'
      }).then(() => {
        Message({
          type: 'success',
          message: '您已退出系统'
        })
        this.$store.commit('LOGOUT')
        this.$router.push({ name: 'Login' })
      }).catch(() => {
        Message({
          type: 'info',
          message: '已取消退出'
        })
      })
    },
    goToCreateRequest () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'CreateRequest' })
      }, 300)
    },
    goToViewRequest () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'ViewRequest' })
      }, 300)
    },
    goToManageRequest () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'ManageRequest' })
      }, 300)
    },
    goToAIAnalysis () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'AIAnalysis' })
      }, 300)
    },
    goToAccountManagement () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'AccountManagement', params: { userid: this.userid } })
      }, 300)
    },
    goToViewMessage () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'ViewMessage' })
      }, 300)
    }
  },
  created: async function () {
    try {
      const userinfoResponse = await axios.get(serverURL + '/v1/user/getinformation', {
        params: {
          userId: parseInt(store.state.user.userid)
        },
        headers: {
          'Authorization': 'Bearer ' + store.state.user.token
        }
      })
      if (userinfoResponse.status === 200) {
        this.avatarUrl = userinfoResponse.data.icon
        this.$forceUpdate()
      }
    } catch (error) {
      MessageBox.alert('获取用户信息失败,请联系系统管理员!', '提示:', {
        confirmButtonText: '好的',
        type: 'error'
      })
    }
  }
}
</script>

<style scoped>
.top-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 42px;
  background-color: rgba(255, 255, 255, 0.65); /* 半透明白色 */
  border-bottom: 1px solid rgba(0, 0, 0, 0.4); /* 添加底部边框 */
  z-index: 0;
}

.clock {
  position: absolute;
  top: 10px;
  right: 15px; /* 时间移到右上角 */
  font-size: 16px;
  color: #222;
  text-shadow: 0 0 1px rgba(0, 0, 0, 0.5);
}

.title {
  position: absolute;
  top: 10px;
  font-size: 16px;
  font-weight: bold;
  color: #222;
  text-shadow: 0 0 1px rgba(0, 0, 0, 0.5);
}

.logout-button {
  position: absolute;
  top: 6px;
  left: 10px;
  padding: 6px 12px; /* 调整按钮内部间距 */
  font-size: 14px;
  font-family: MiSans, "PingFang SC", "Microsoft YaHei", sans-serif;
  color: #222;
  border: none;
  background-color: rgba(255,255,255,0.5); /* 默认背景色 */
  border-radius: 5px; /* 圆角矩形 */
  transition: all 0.3s ease; /* 添加平滑变色效果 */
  cursor: pointer;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
}

.logout-button:hover {
  background-color: rgba(187, 41, 41, 0.9); /* 鼠标悬停时背景色 */
  color: white;
}

.button-container {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 40px;
  width: 89%;
  padding: 50px;
  position: absolute;
  top: 36%;
  border-radius: 20px;
  background-color: rgba(255, 255, 255, 0.0); /* 半透明白色 */
  box-shadow: 0 0 0px rgba(0, 0, 0, 0.5);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.button-container.fadeIn {
  opacity: 1; /* 添加类后，透明度变为1 */
}

.button-container.fadeOut {
  opacity: 0; /* 淡出：透明度变为0 */
}

/* 通用按钮样式 */
.nav-button {
  display: flex;
  flex-direction: column;  /* 设置为垂直排列 */
  justify-content: center; /* 内容垂直居中 */
  align-items: center;     /* 内容水平居中 */
  flex: 1;
  max-width: calc(98vw / 3);
  height: calc(98vh / 3);
  font-size: 18px;
  color: white;
  background-color: rgba(255, 255, 255, 0.1); /* 默认背景 */
  border: 1px solid #aaa;
  border-radius: 20px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  cursor: pointer;
  overflow: hidden; /* 隐藏伪元素溢出部分 */
  position: relative; /* 为伪元素定位 */
  transition: transform 0.5s ease, box-shadow 0.5s ease; /* 平滑过渡效果 */
}

.nav-button::before {
  content: ''; /* 伪元素内容 */
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(45deg, var(--hover-color-start), var(--hover-color-end)); /* 渐变背景 */
  border-radius: 20px; /* 确保与按钮一致 */
  z-index: -1; /* 放置在按钮内容下方 */
  opacity: 0; /* 默认透明 */
  transition: opacity 0.5s ease; /* 平滑透明度过渡 */
}

.nav-button:hover {
  transform: scale3d(1.2, 1.2, 1.2); /* 悬停放大 */
  box-shadow: 0 0 50px rgba(0, 0, 0, 0.6); /* 悬停时增强阴影 */
}

.nav-button:hover::before {
  opacity: 1; /* 鼠标悬停时显示伪元素的背景 */
}

.nav-button:active {
  transform: scale3d(0.98, 0.98, 1); /* 点击缩小 */
  box-shadow: 0 0 8px rgba(0, 0, 0, 0.2); /* 点击时阴影变小 */
  transition: all 0.2s ease; /* 添加平滑过渡效果 */
}

.nav-button .button-title {
  display: inline-block; /* 必须，确保文字是块级元素可平滑移动 */
  position: relative; /* 相对于按钮定位 */
  top: -10%; /* 初始位置：垂直居中 */
  right: 0;
  width: 55%;
  transition: all 0.3s ease; /* 添加平滑过渡效果 */
}

.nav-button:hover .button-title {
  top: -25%; /* 鼠标悬停时文字移动到按钮内部中上方 */
  right: -15%;
  font: bold 1.2em "MiSans", "PingFang SC", "Microsoft YaHei", sans-serif; /* 字体加粗 */
}

/* 按钮颜色设置 */
.nav-button-create {
  --hover-color-start: #1468fd;
  --hover-color-end: #b10497;
}

.nav-button img {
  top: 6%;
  right: 0;
  width: 50px;
  height: 50px;
  position: relative;
  object-fit: cover; /* 保持比例，确保图片不变形 */
  border: none;
  transition: all 0.3s ease; /* 添加平滑过渡效果 */
}

.nav-button:hover img {
  top: -36%;
  right: 33%;
  width: 38px;
  height: 38px;
}

/* 提示文字基础样式 */
.nav-button .tooltip-text {
  position: absolute;
  top: 65%; /* 相对于按钮底部的位置 */
  left: 50%; /* 相对于按钮中心的位置 */
  width: 80%;
  transform: translateX(-50%);
  background-color: transparent;
  color: white; /* 白色文字 */
  font-size: 19px;
  opacity: 0; /* 默认完全透明 */
  transition: opacity 0.3s ease; /* 平滑过渡 */
  text-align: left;
  pointer-events: none; /* 避免鼠标干扰 */
}

/* 鼠标悬停时提示文字变透明 */
.nav-button:hover .tooltip-text {
  opacity: 0.5; /* 悬停时半透明 */
}

.nav-button-view {
  --hover-color-start: #05b1c1;
  --hover-color-end: #d3da10;
}

.nav-button-manage {
  --hover-color-start: #e6102a;
  --hover-color-end: #d68c08;
}

.nav-button-message {
  --hover-color-start: #084ecd;
  --hover-color-end: #0bcfc8;
}

.nav-button-ai {
  --hover-color-start: #8429eb;
  --hover-color-end: #d1460a;
}

.home-page {
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
  width: 65px;
  height: 65px;
  border-radius: 50%; /* 圆形 */
  object-fit: cover; /* 保持比例，确保图片不变形 */
  border: 0 solid #333; /* 圆形边框 */
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5); /* 添加阴影 */
}

.content-container {
  position: absolute;
  top: 150px;
  max-width: 400px;
  width: 100%;
  padding: 10px;
  border-radius: 10px;
  background-color: rgba(255, 255, 255, 0); /* 设置为完全透明 */
  text-align: center;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.content-container.fadeIn {
  opacity: 1; /* 添加类后，透明度变为1 */
}

.content-container.fadeOut {
  opacity: 0; /* 淡出：透明度变为0 */
}

h2 {
  font-size: 24px;
  color: #ddd;
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.15);
}

.footer-text {
  position: absolute;
  bottom: 15px;
  font-size: 16px;
  color: #222;
  padding: 10px;
  border-radius: 12px;
  background-color: rgba(255,255,255,0.65); /* 设置透明背景 */
  text-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.footer-text.fadeIn {
  opacity: 1; /* 添加类后，透明度变为1 */
}

.footer-text.fadeOut {
  opacity: 0; /* 淡出：透明度变为0 */
}

.account-button {
  margin-left: 20px; /* 给按钮加些间距 */
  padding: 6px 12px;
  font-size: 14px;
  font-family: MiSans, PingFang SC, Microsoft YaHei, sans-serif;
  color: #222;
  background-color: rgba(255,255,255,0.7); /* 背景色 */
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
}

.account-button:hover {
  background-color: rgba(47, 122, 216, 0.9); /* 鼠标悬停时的背景色 */
  color: white;
}

</style>
