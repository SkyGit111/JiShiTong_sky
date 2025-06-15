<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>查看个人相关拼单请求的消息通知</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 class="page-title">查看个人相关拼单请求的消息通知</h1>
      <p class="page-description">获得系统中有关您的拼单的最新动态。</p>
      <div class="content-box">
        <el-container style="height: 100%; width: 100%; overflow: auto">
          <el-main>
          <div class="pool-messages">
            <!-- 消息列表 -->
            <el-table :data="sortedMessages" stripe>
              <el-table-column prop="message_id" label="消息id" ></el-table-column>
              <el-table-column prop="sender_id" label="发送人" ></el-table-column>
              <el-table-column prop="message_time" label="发送时间" ></el-table-column>
              <el-table-column prop="request_id" label="拼单号" ></el-table-column>
              <el-table-column prop="poolName" label="拼单标题" ></el-table-column>
              <el-table-column prop="content" label="消息内容"></el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                    <el-button type="text" size="small" @click="deleteMessage(scope.row)">删除</el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
          </el-main>
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
import store from '../store'
import axios from 'axios'
import serverURL from '../server.config'

export default {
  data () {
    return {
      currentDate: this.getCurrentDate(), // 初始化时获取当前日期
      showContent: false,
      messages: [],
      sortedMessages: [],
      dialogVisible: false,
      selectedMessage: null
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
    sortMessages () {
      // 按发送时间排序消息
      for (let i = 0; i < this.messages.length; i++) {
        this.messages[i].message_time = this.messages[i].message_time.replace('T', ' ')
      }
      this.sortedMessages = [...this.messages].sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
    },
    deleteMessage (message) {
      MessageBox.confirm('您确定要删除这条消息吗?', '提示:', {
        confirmButtonText: '是的',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(async () => {
        try {
          const deleteResponse = await axios.delete(serverURL + `/v1/message/delete?messageId=${message.message_id}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (deleteResponse.status === 200) {
            this.$message.success('删除成功')
            setTimeout(() => {
              this.$router.go(0)
            }, 300)
            this.sortMessages()
          }
        } catch (error) {
          this.$message.error('删除失败')
        }
      })
    }
  },
  async created () {
    try {
      const messageResponse = await axios.get(serverURL + `/v1/message/getInformation?userId=${store.state.user.userid}`, {
        headers: {
          Authorization: 'Bearer ' + this.$store.state.user.token
        }
      })
      if (messageResponse.status === 200) {
        this.messages = messageResponse.data.messages
      }
    } catch (error) {
      this.$message.error('获取拼单信息失败')
    }
    for (let i = 0; i < this.messages.length; i++) {
      try {
        const groupOrderResponse = await axios.get(serverURL + `/v1/groupOrder/getinformation?requestId=${this.messages[i].request_id}`, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (groupOrderResponse.status === 200) {
          this.messages[i].message_time = this.messages[i].message_time.replace('T', ' ')
          this.messages[i].poolName = groupOrderResponse.data.data[0].title
        }
      } catch (error) {
        this.$message.error('获取拼单标题失败')
      }
    }
    this.sortMessages()
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
</style>
