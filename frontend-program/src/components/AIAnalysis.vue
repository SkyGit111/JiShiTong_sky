<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>AI智能分析与推荐</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 class="page-title">AI智能分析与推荐</h1>
      <p class="page-description">使用我们高效的AI工具，分析并获取满足您需求的拼单。</p>
      <div class="content-box" v-loading="loading" element-loading-text="思考中请稍候" element-loading-spinner="el-icon-loading" element-loading-background="rgba(255, 255, 255, 0.6)">
        <el-container style="height: 100%">
          <el-header>
            <p><strong>AI智能分析与推荐</strong></p>
          </el-header>
          <el-main>
        <!-- AI 对话历史 -->
        <div class="dialog-chathistory">
          <el-scrollbar style="height: 100%;">
            <div v-for="(msg, index) in messages" :key="index" :class="['message', msg.isUser ? 'user' : 'ai']">
              <div v-bind:class="msg.isUser? 'message-container1' : 'message-container2'">
                <img v-if="!msg.isUser" class="avatar" src="@/assets/defaultavatar.jpg" alt="AI Avatar"  />
                  <div class="message-content">{{ msg.text }}</div>
                  <img v-if="msg.isUser" class="avatar" :src="userAvatar" alt="User Avatar" />
              </div>
            </div>
          </el-scrollbar>
        </div>
        </el-main>
        <el-footer>
        <!-- 输入框和发送按钮 -->
          <el-input
            v-model="userMessage"
            placeholder="请输入您的问题..."
            class="input-box"
            @keyup.enter="sendMessage">
            <template #append>
              <el-button type="primary" @click="sendMessage" icon="el-icon-send">发送</el-button>
            </template>
          </el-input>
        </el-footer>
        </el-container>
      </div>
    </div>

    <div class="background">
      <img src="@/assets/background.jpg" alt="background" />
    </div>
  </div>
</template>

<script>
import {Message, MessageBox} from 'element-ui'
import serverURL from '../server.config'
import axios from 'axios'
import store from '../store'

export default {
  data () {
    return {
      OpenAI: require('openai'),
      client: '',
      chathistory: [],
      currentDate: this.getCurrentDate(), // 初始化时获取当前日期
      showContent: false,
      userMessage: '',
      messages: [],
      loading: false,
      userAvatar: require('@/assets/defaultavatar.jpg')
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
    back () {
      // 设置内容为隐藏（渐渐消失）
      this.showContent = false

      // 等待动画完成后再跳转页面
      setTimeout(() => {
        this.$router.push({ name: 'Home' })
      }, 300) // 0.3秒后跳转，与动画时间保持一致
    },
    async sendMessage () {
      if (this.userMessage.trim() !== '') {
        this.messages.push({text: this.userMessage, isUser: true})
        this.chathistory.push(
          {'role': 'user', 'content': this.userMessage}
        )
        console.log(this.chathistory)
        this.loading = true
        const response = await axios.post('https://api.moonshot.cn/v1/chat/completions', {
          'model': 'moonshot-v1-8k',
          'messages': this.chathistory,
          'temperature': 0.3
        }, {
          headers: {
            'Authorization': 'Bearer ' + 'sk-iDnT1hkxxcXGMB4q8kF1qazqDDqAbd7Bs6vqreUPkKZVRog9'
          }
        })
        if (response.data.choices[0].message.content) {
          this.loading = false
          this.chathistory.push(
            {'role': 'system', 'content': response.data.choices[0].message.content}
          )
          this.messages.push({text: response.data.choices[0].message.content, isUser: false})
        }
        this.loading = false
        this.userMessage = ''
      }
    }
  },
  async created () {
    this.loading = true
    this.chathistory = []
    try {
      const userResponse = await axios.get(serverURL + `/v1/user/getinformation?userId=${store.state.user.userid}`, {
        headers: {
          'Authorization': 'Bearer ' + store.state.user.token
        }
      })
      if (userResponse.data.code === 200) {
        this.userAvatar = userResponse.data.icon
      }
    } catch (error) {
      Message({
        type: 'error',
        message: '初始化用户信息失败'
      })
    }
    let requestList = []
    let reqId = []
    let reqSeq = []
    try {
      const reqResponse = await axios.get(serverURL + `/v1/groupOrder/getall`, {
        headers: {
          Authorization: 'Bearer ' + this.$store.state.user.token
        }
      })
      if (reqResponse.status === 200) {
        reqId = reqResponse.data.data
      }
    } catch (error) {
      this.$message.error('获取拼单号失败')
    }
    for (let i = 0; i < reqId.length; i++) {
      reqSeq[i] = reqId[i].request_id
    }
    try {
      const groupOrderResponse = await axios.post(serverURL + '/v1/groupOrder/getmultipleinformation', reqSeq, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + this.$store.state.user.token
        }
      })
      if (groupOrderResponse.status === 200) {
        const temp1 = groupOrderResponse.data.data
        for (let i = 0; i < temp1.length; i++) {
          requestList[i] = temp1[i][0]
          console.log(requestList[i])
        }
        console.log(requestList.toString())
      }
    } catch (error) {
      this.$message.error('获取拼单失败')
    }
    var reqs
    for (let i = 0; i < requestList.length; i++) {
      reqs = reqs + ' ' + JSON.stringify(requestList[i], null, 2)
    }
    this.chathistory = [
      {
        'role': 'system',
        'content': '你是济事通拼单系统管家，你只擅长中文对话。你会为用户提供安全，有帮助，准确的回答。同时，你会拒绝一切涉及恐怖主义，种族歧视，黄色暴力等问题的回答。注意，你将只回答与拼单相关的问题，不会关注用户提出的其他问题。注意你每次回复我的信息不允许超过70个字。'
      },
      {
        'role': 'user',
        'content': '注意，下面是系统中所有的拼单信息：' + reqs
      },
      {
        'role': 'system',
        'content': '好的，我已经收到了你的拼单信息。'
      },
      {
        'role': 'user',
        'content': '好的，我的拼单内容已经发给你了。请注意，后续的所有对话请不要说你正在读取信息或正在思考，请在思考后直接给出你的回答。接下来，我们将开始正式的问答。你的下一句回答应该是：您好，我是济事通拼单系统管家，有什么可以帮助您的吗？'
      }
    ]
    const response = await axios.post('https://api.moonshot.cn/v1/chat/completions', {
      'model': 'moonshot-v1-8k',
      'messages': this.chathistory,
      'temperature': 0.3
    }, {
      headers: {
        'Authorization': 'Bearer ' + 'sk-iDnT1hkxxcXGMB4q8kF1qazqDDqAbd7Bs6vqreUPkKZVRog9'
      }
    })
    if (response.data.choices[0].message.content) {
      this.loading = false
      this.chathistory.push(
        {'role': 'system', 'content': response.data.choices[0].message.content}
      )
      this.messages.push({text: response.data.choices[0].message.content, isUser: false})
    }
    this.loading = false
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

.back-button {
  position: absolute;
  top: 6px;
  left: 70px;
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

.back-button:hover {
  background-color: rgba(26, 160, 3, 0.85); /* 鼠标悬停时背景色 */
  color: white;
}

.home-page {
  display: flex;
  height: 98vh;
  width: 99vw;
  justify-content: center;
  align-items: center;
  position: relative;
}

.content {
  position: absolute;
  top: 65px;
  width: calc(99vw - 40px);
  height: calc(98vh - 85px);
  background-color: rgba(255, 255, 255, 0.3); /* 半透明背景色 */
  border-radius: 10px;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.3);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.content.fadeIn {
  opacity: 1; /* 添加类后，透明度变为1 */
}

.content.fadeOut {
  opacity: 0; /* 淡出：透明度变为0 */
}

.page-title {
  text-align: left;
  font-size: 26px;
  font-weight: bold;
  margin-top: 25px;
  margin-left: 20px;
  margin-bottom: 0;
  color: #2b2b2b;
}

.page-description {
  text-align: left;
  font-size: 15px;
  margin-top: 10px;
  margin-left: 20px;
  color: #2b2b2b;
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
.content-box {
  width: calc(99vw - 120px);
  height: calc(98vh - 250px);
  padding: 20px;
  margin-top: 10px;
  margin-left: 20px;

  background-color: rgba(255, 255, 255, 0.7); /* 半透明背景色 */
  border-radius: 7px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
.dialog-chathistory {
  flex-grow: 1;
  display: flex;
  justify-content: flex-end;
  flex-direction: column;
  overflow-y: auto;
  margin-bottom: 20px;
}
.message {
  margin: 10px 0;
  max-width: 80%;
  padding: 10px;
  border-radius: 12px;
  font-size: 14px;
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  padding-bottom: 20px;
}

.user .message-content {
  background-color: #9fc8e8;
  box-align: flex-end;
  padding: 8px 15px;
  border-radius: 10px;
  word-wrap: break-word;
}

.ai .message-content {
  background-color: #f1f1f1;
  box-align: flex-start;
  padding: 8px 15px;
  border-radius: 10px;
  word-wrap: break-word;
}

.input-box {
  width: 80%;
  margin-right: 10px;
}
.message-container1 {
  position: absolute;
  right: 8%;
  display: flex;
  align-items: flex-end;
  justify-content: flex-start;
  margin-left: auto;
  gap: 10px;
}
.message-container2 {
  position: absolute;
  left: 8%;
  display: flex;
  align-items: flex-end;
  justify-content: flex-end;
  margin-right: auto;
  gap: 10px;
}
.avatar {
  width: 35px;
  height: 35px;
  border-radius: 50%;
  object-fit: cover;
}
</style>
