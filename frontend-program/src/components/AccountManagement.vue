<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>个人账号管理</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 class="page-title">个人账号管理</h1>
      <p class="page-description">检视或管理您的济事通账号的具体信息。</p>
      <div class="content-box">
        <div class="user-info-container">
          <div style="width: 40%; display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center;">
            <div class="avatar-box" @click="uploadAvatar" v-loading="loading" element-loading-text="上传中" element-loading-spinner="el-icon-loading" element-loading-background="rgba(0, 0, 0, 0.8)">
              <img :src="userForm.avatarUrl" alt="用户头像" class="avatar" />
              <input type="file" ref="fileInput" @change="handleFileUpload" style="display: none;" />
            </div>
            <h4>点击上传新头像</h4>
          </div>
          <div style="width: 60%; margin-top: 5vh; margin-bottom: 3vh; margin-right: 5vh">
        <el-container style="width: 100%; height: 100%; overflow: auto">

          <el-main>
                <el-form ref="userForm" :model="userForm" label-width="100px" style="align-items: center">
                  <el-form-item label="用户名">
                    <el-input v-model="userForm.username" placeholder="输入新用户名"></el-input>
                  </el-form-item>
                  <el-form-item label="修改密码">
                    <el-input type="password" v-model="userForm.newPassword" autocomplete="off" placeholder="(未修改)"></el-input>
                  </el-form-item>
                  <el-form-item label="确认新密码">
                    <el-input type="password" v-model="userForm.confirmPassword" autocomplete="off" placeholder="(未修改)"></el-input>
                  </el-form-item>
                  <el-form-item v-if="userrole === 'student'" label="年龄">
                    <el-input type="number" v-model="userForm.age" :min="1" @input="checkAge" placeholder="(暂未填写)"></el-input>
                  </el-form-item>
                  <el-form-item v-if="userrole === 'student'" label="联系方式">
                    <el-input v-model="userForm.contact" placeholder="(暂未填写)"></el-input>
                  </el-form-item>
                  <el-form-item v-if="userrole === 'student'" label="个人介绍" style="max-height: 15vh">
                    <el-input v-model="userForm.signature" placeholder="(暂未填写)"></el-input>
                  </el-form-item>
                  <el-form-item style="margin-bottom: 0 !important;">
                    <div class="action-buttons">
                      <el-button type="primary" @click="confirmModify">更新信息</el-button>
                      <el-button type="danger" @click="deleteAccount">注销账号</el-button>
                    </div>
                  </el-form-item>
                </el-form>
          </el-main>
        </el-container>
          </div>
        </div>
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
      userrole: store.state.user.userrole,
      currentDate: this.getCurrentDate(),
      showContent: false,
      dialogVisibleDelete: false,
      userForm: {
        username: '',
        newPassword: '',
        confirmPassword: '',
        signature: '',
        contact: '',
        age: '',
        avatarUrl: ''
      },
      loading: false
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
    checkAge (value) {
      if (value < 1) {
        this.userForm.age = 1
      }
    },
    async confirmModify () {
      if (!this.checkForm()) {
        this.loading = false
        return
      }
      const data = { userId: parseInt(store.state.user.userid) }
      if (this.userForm.newPassword !== '' && this.userForm.newPassword !== null) {
        data.userPassword = this.userForm.newPassword
      }
      if (this.userForm.username !== '' && this.userForm.username !== null) {
        data.userName = this.userForm.username
      }
      if (this.userForm.signature !== '' && this.userForm.signature !== null) {
        data.userIntroduction = this.userForm.signature
      }
      if (this.userForm.contact !== '' && this.userForm.contact !== null) {
        data.userContact = this.userForm.contact
      }
      if (this.userForm.age !== '' && this.userForm.age !== null) {
        data.userAge = parseInt(this.userForm.age)
      }
      if (this.userForm.avatarUrl !== '' && this.userForm.avatarUrl !== null) {
        data.icon = this.userForm.avatarUrl
      }

      try {
        const changeResponse = await axios.post(serverURL + '/v1/user/modify', data, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + store.state.user.token
          }
        })
        if (changeResponse.status === 200) {
          this.$message.success('个人信息更新成功')
          this.store.state.username = this.userForm.username
        }
      } catch (error) {
        if (error.response.status === 400) {
          Message({
            type: 'error',
            message: '修改失败，请重试'
          })
        }
      }
    },
    checkForm () {
      if (this.userForm.newPassword !== this.userForm.confirmPassword) {
        MessageBox('新密码和确认密码不一致', '提示:', {
          confirmButtonText: '好的',
          type: 'error'
        })
        return false
      }
      if (this.userForm.username === '' || this.userForm.username === null) {
        MessageBox('用户名不能为空', '提示:', {
          confirmButtonText: '好的',
          type: 'error'
        })
        return false
      }
      return true
    },
    deleteAccount () {
      MessageBox.confirm('您确定要注销账号吗?该操作不可撤销,请慎重操作!', '提示:', {
        confirmButtonText: '是的',
        cancelButtonText: '取消',
        type: 'confirm'
      }).then(async () => {
        try {
          const deleteResponse = await axios.delete(serverURL + `/v1/user/delete?userId=${store.state.user.userid}`, {
            headers: {
              'Authorization': 'Bearer ' + store.state.user.token
            }
          })
          if (deleteResponse.status === 200) {
            Message({
              type: 'success',
              message: '您的账号已成功注销'
            })
            this.$store.commit('LOGOUT')
            this.$router.push({ name: 'Login' })
          }
        } catch (error) {
          Message({
            type: 'error',
            message: '注销失败，请联系管理员'
          })
        }
      }).catch(() => {
        Message({
          type: 'info',
          message: '已取消注销'
        })
      })
    },
    uploadAvatar () {
      this.$refs.fileInput.click()
    },
    async handleFileUpload (event) {
      this.loading = true
      const file = event.target.files[0]
      if (file) {
        // 检查文件类型是否为图片
        const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/jpg']
        if (!allowedTypes.includes(file.type)) {
          Message({
            type: 'error',
            message: '请上传有效的头像图片文件(jpeg/png/gif/jpg)'
          })
          this.loading = false
          return
        }

        const APIKEY = 'fdea0e9f73cf3c2a4fbb5e1abb67abba'
        try {
          const formData = new FormData()
          formData.append('image', file)
          const imgResponse = await axios.post(`https://api.imgbb.com/1/upload?key=${APIKEY}&name=${store.state.user.userid}`, formData)
          if (imgResponse.status === 200) {
            Message({
              type: 'success',
              message: '上传头像成功'
            })
            this.userForm.avatarUrl = imgResponse.data.data.url
            this.$forceUpdate()
          }
        } catch (error) {
          Message({
            type: 'error',
            message: '上传头像失败，请重试'
          })
        }
        this.loading = false
      }
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
        this.userForm.username = userinfoResponse.data.user_name
        this.userForm.signature = userinfoResponse.data.user_introduction
        this.userForm.contact = userinfoResponse.data.user_contact
        this.userForm.age = userinfoResponse.data.user_age
        this.userForm.avatarUrl = userinfoResponse.data.icon
        this.userForm = { ...this.userForm }
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
.user-info-container {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center; /* 垂直居中 */
  justify-content: flex-start; /* 水平排列 */
}

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

.action-buttons {
  display: flex; /*按钮布局*/
  justify-content: center;
}

.avatar-box {
  width: 38vh;
  height: 38vh;
  border-radius: 50%;
  overflow: hidden;
  cursor: pointer;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.3);
}

.avatar {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>
