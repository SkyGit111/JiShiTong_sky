<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>注册账号</span>
    </div>

    <button type="reset" @click="logout" class="logout-button">退出</button>
    <button type="back" @click="back" class="back-button">返回</button>

    <div class="background">
      <img src="@/assets/background.jpg" alt="background" />
    </div>

    <div class="registerform">
      <el-form :model="ruleForm" status-icon :rules="rules" ref="ruleForm" label-width="auto" class="demo-ruleForm">
        <el-form-item label="用户名" prop="name">
          <el-input type="name" v-model="ruleForm.name" autocomplete="off"></el-input>
        </el-form-item>
        <el-form-item label="密码" prop="pass">
          <el-input type="password" v-model="ruleForm.pass" autocomplete="off"></el-input>
        </el-form-item>
        <el-form-item label="确认密码" prop="checkPass">
          <el-input type="password" v-model="ruleForm.checkPass" autocomplete="off"></el-input>
        </el-form-item>
        <el-form-item label="身份">
          <el-radio-group v-model="ruleForm.role">
            <el-radio label="student" value="student">学生账户</el-radio>
            <el-radio label="admin" value="admin">管理员账户</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item v-if="ruleForm.role === 'admin'" label="注册密码" prop="adminPassword">
          <el-input type="password" v-model="ruleForm.adminPassword" autocomplete="off"></el-input>
        </el-form-item>
        <div class="notice">您的个人详细信息可以在以后修改</div>
        <el-button type="primary" @click="submitForm('ruleForm')">注册新账号</el-button>
      </el-form>
    </div>
  </div>
</template>

<script>
import {Message, MessageBox} from 'element-ui'
import axios from 'axios'
import serverURL from '../server.config'
import router from '../router'

export default {
  data () {
    let checkName = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入用户名'))
      } else {
        callback()
      }
    }
    let validatePass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入密码'))
      } else {
        if (this.ruleForm.checkPass !== '') {
          this.$refs.ruleForm.validateField('checkPass')
        }
        callback()
      }
    }
    let validatePass2 = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.ruleForm.pass) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    let validateAdminPassword = (rule, value, callback) => {
      if (this.ruleForm.role === 'admin' && value === null) {
        callback(new Error('请输入注册密码'))
      } else {
        callback()
      }
    }
    return {
      ruleForm: {
        pass: '',
        checkPass: '',
        name: '',
        role: 'student', // 身份字段
        adminPassword: null // 管理员注册密码
      },
      rules: {
        pass: [
          { validator: validatePass, trigger: 'blur' }
        ],
        checkPass: [
          { validator: validatePass2, trigger: 'blur' }
        ],
        name: [
          { validator: checkName, trigger: 'blur' }
        ],
        adminPassword: [
          { validator: validateAdminPassword, trigger: 'blur' }
        ]
      },
      currentDate: this.getCurrentDate()
    }
  },
  mounted () {
    // 每秒更新一次时钟
    setInterval(() => {
      this.currentDate = this.getCurrentDate()
    }, 1000)
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
      this.$router.push({ name: 'Login' })
    },
    submitForm (formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          try {
            Message({
              type: 'info',
              message: '注册中，请稍候',
              duration: 2000
            })
            if (this.ruleForm.adminPassword === null) {
              const response = await axios.post(serverURL + `/v1/user/register`, {
                userName: this.ruleForm.name,
                userPassword: this.ruleForm.pass,
                userRole: this.ruleForm.role
              }, {
                headers: {
                  'Content-Type': 'application/json'
                }
              })
              if (response.status === 200) {
                MessageBox.alert('您已注册成功!您的账户为' + (this.ruleForm.role === 'student' ? '学生账户,' : '管理员账户,') + `账号为${response.data.user_id},请牢记您的账号和密码!`, '提示:', {
                  confirmButtonText: '好的',
                  callback: () => {
                    Message({
                      type: 'info',
                      message: '请您继续登录',
                      duration: 2000
                    })
                    router.push('/login')
                  }
                })
              }
            } else {
              const response = await axios.post(serverURL + `/v1/user/register?org_pass=${this.ruleForm.adminPassword}`, {
                userName: this.ruleForm.name,
                userPassword: this.ruleForm.pass,
                userRole: this.ruleForm.role
              })
              if (response.status === 200) {
                MessageBox.alert('您已注册成功!您的账户为' + (this.ruleForm.role === 'student' ? '学生账户,' : '管理员账户,') + `账号为${response.data.user_id},请牢记您的账号和密码!`, '提示:', {
                  confirmButtonText: '好的',
                  callback: () => {
                    Message({
                      type: 'info',
                      message: '请您继续登录',
                      duration: 2000
                    })
                    router.push('/login')
                  }
                })
              }
            }
          } catch (error) {
            if (error.response.status === 400) {
              MessageBox.alert('抱歉，您的注册密码错误，请重试或联系您所属组织的管理员进行确认。', '提示:', {
                confirmButtonText: '好的',
                type: 'error'
              })
            } else {
              Message(
                {
                  message: '服务器错误，请等待修复后再试，或联系管理员',
                  type: 'error'
                }
              )
            }
          }
        } else {
          return false
        }
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

.registerform {
  position: absolute;
  width: 25vw;
  height: auto;
  background-color: rgba(230, 245, 255, 0.75); /* 半透明背景色 */
  border-radius: 10px;
  box-shadow: 0 0 15px rgba(0, 0, 0, 0.2);
  padding: 60px;
}

.notice {
  position: relative;
  top: 0;
  margin-bottom:20px;
  right: 0;
  font-size: 14px;
  color: rgba(21, 21, 21, 0.65);
}
</style>
