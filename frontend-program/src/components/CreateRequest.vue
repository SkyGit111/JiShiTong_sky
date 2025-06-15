<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>创建并发布拼单请求</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 class="page-title">创建并发布拼单请求</h1>
      <p class="page-description">创建一个新的拼单请求并发布出去，供其他用户选择。</p>
      <div class="content-box">
        <el-container style="height: 100%">
          <el-header style="padding: 0; display: flex; align-items: center; background-color: #B3C0D1; font-size: 12px">
            <div style="flex-grow: 1;">
              <el-menu mode="horizontal" v-model="activeIndex" @select="handleSelect" background-color="#B3C0D1" text-color="#fff" active-text-color="#ffd04b" default-active="1">
                <el-menu-item index="1">
                  <template slot="title"><i class="el-icon-money"></i>拼好物</template>
                </el-menu-item>
                <el-menu-item index="2">
                  <template slot="title"><el-icon name="location"></el-icon>拼交通</template>
                </el-menu-item>
                <el-menu-item index="3">
                  <template slot="title"><el-icon name="trophy"></el-icon>拼活动</template>
                </el-menu-item>
              </el-menu>
            </div>
          </el-header>
          <el-main v-loading="loading" element-loading-text="上传中" element-loading-spinner="el-icon-loading" element-loading-background="rgba(255, 255, 255, 0.6)">
            <div v-if="activeIndex === '1'">
              <el-form ref="textForm1" :model="textForm1" label-width="100px">
                <p>拼好物，一起拼团享受更低价格，购物更实惠！</p>
                <el-row :gutter="20">
                  <el-col :span="12">
                    <el-form-item label="拼单标题">
                      <el-input v-model="textForm1.title" placeholder="请输入拼单标题"></el-input>
                    </el-form-item>
                    <el-form-item label="拼单描述">
                      <el-input v-model="textForm1.description" placeholder="请输入拼单描述"></el-input>
                    </el-form-item>
                    <el-form-item label="开放时间">
                      <el-date-picker v-model="textForm1.starttime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="关闭时间">
                      <el-date-picker v-model="textForm1.endtime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="预期人数">
                      <el-input type="number" v-model="textForm1.customValue" :min="1" step="1" @input="checkCustomValue" style="width: 100% !important;" placeholder="请输入人数"></el-input>
                    </el-form-item>
                    <el-form-item label="额外要求">
                      <el-input v-model="textForm1.request" placeholder="请输入额外要求"></el-input>
                    </el-form-item>
                    <el-form-item label="总价分配">
                      <el-input v-model="textForm1.distribute" placeholder="请输入总价分配"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="物品名称">
                      <el-input v-model="textForm1.name" placeholder="请输入物品名称"></el-input>
                    </el-form-item>
                    <el-form-item label="总金额">
                      <el-input type="number" v-model="textForm1.price" :min="0" step="1" @input="checkTotalValue" placeholder="请输入总金额"></el-input>
                    </el-form-item>
                    <el-form-item label="平台或商家">
                      <el-input v-model="textForm1.platform" placeholder="请输入平台或商家"></el-input>
                    </el-form-item>
                    <el-form-item label="购物或收货地">
                      <el-input v-model="textForm1.location" placeholder="请输入购物地或收货地"></el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <!-- 在每个表单的el-button之前添加 -->
                <div class="voice-section" style="margin-bottom: 20px; text-align: center;">
                  <el-button type="success" @click="startVoiceRecognition(activeIndex)" :disabled="isRecording">
                    <i class="el-icon-microphone"></i>
                    {{ isRecording ? '识别中...' : '语音创建拼单' }}
                  </el-button>
                  <div v-if="voiceText" class="voice-result" style="margin-top: 10px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>
                </div>
                <el-button type="primary" @click="CreateOrder" style="margin: 0 auto;">创建拼单</el-button>
              </el-form>
            </div>
            <div v-if="activeIndex === '2'">
              <el-form ref="textForm2" :model="textForm2" label-width="100px">
                <p>拼交通，共享出行，环保又经济，让旅途更轻松！</p>
                <el-row :gutter="20">
                  <el-col :span="12">
                    <el-form-item label="拼单标题">
                      <el-input v-model="textForm2.title" placeholder="请输入拼单标题"></el-input>
                    </el-form-item>
                    <el-form-item label="拼单描述">
                      <el-input v-model="textForm2.description" placeholder="请输入拼单描述"></el-input>
                    </el-form-item>
                    <el-form-item label="开放时间">
                      <el-date-picker v-model="textForm2.starttime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="关闭时间">
                      <el-date-picker v-model="textForm2.endtime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="预期人数">
                      <el-input type="number" v-model="textForm2.customValue" :min="1" step="1" @input="checkCustomValue" style="width:100% !important;" placeholder="请输入人数"></el-input>
                    </el-form-item>
                    <el-form-item label="额外要求">
                      <el-input v-model="textForm2.request" placeholder="请输入额外要求"></el-input>
                    </el-form-item>
                    <el-form-item label="总价分配">
                      <el-input v-model="textForm2.distribute" placeholder="请输入总价分配"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="交通名称">
                      <el-input v-model="textForm2.name" placeholder="请输入交通名称"></el-input>
                    </el-form-item>
                    <el-form-item label="总金额">
                      <el-input type="number" v-model="textForm2.price" :min="0" step="1" @input="checkTotalValue" placeholder="请输入总金额"></el-input>
                    </el-form-item>
                    <el-form-item label="始发地">
                      <el-input v-model="textForm2.location1" placeholder="请输入始发地"></el-input>
                    </el-form-item>
                    <el-form-item label="目的地">
                      <el-input v-model="textForm2.location2" placeholder="请输入目的地"></el-input>
                    </el-form-item>
                    <el-form-item label="预计出发时间">
                      <el-date-picker v-model="textForm2.time" type="datetime" style="width:100% !important;" placeholder="选择时间"></el-date-picker>
                    </el-form-item>
                  </el-col>
                </el-row>
                <!-- 在每个表单的el-button之前添加 -->
                <div class="voice-section" style="margin-bottom: 20px; text-align: center;">
                  <el-button type="success" @click="startVoiceRecognition(activeIndex)" :disabled="isRecording">
                    <i class="el-icon-microphone"></i>
                    {{ isRecording ? '识别中...' : '语音创建拼单' }}
                  </el-button>
                  <div v-if="voiceText" class="voice-result" style="margin-top: 10px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>
                </div>
                <el-button type="primary" @click="CreateOrder">创建拼单</el-button>
              </el-form>
            </div>
            <div v-if="activeIndex === '3'">
              <el-form ref="textForm3" :model="textForm3" label-width="100px">
                <p>拼活动，发现精彩，共享欢乐，让生活更多彩！</p>
                <el-row :gutter="20">
                  <el-col :span="12">
                    <el-form-item label="拼单标题">
                      <el-input v-model="textForm3.title" placeholder="请输入拼单标题"></el-input>
                    </el-form-item>
                    <el-form-item label="拼单描述">
                      <el-input v-model="textForm3.description" placeholder="请输入拼单描述"></el-input>
                    </el-form-item>
                    <el-form-item label="开放时间">
                      <el-date-picker v-model="textForm3.starttime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="关闭时间">
                      <el-date-picker v-model="textForm3.endtime" type="datetime" placeholder="选择时间" style="width:100% !important;"></el-date-picker>
                    </el-form-item>
                    <el-form-item label="预期人数">
                      <el-input type="number" v-model="textForm3.customValue" :min="1" step="1" @input="checkCustomValue" style="width:100% !important;" placeholder="请输入人数"></el-input>
                    </el-form-item>
                    <el-form-item label="额外要求">
                      <el-input v-model="textForm3.request" placeholder="请输入额外要求"></el-input>
                    </el-form-item>
                    <el-form-item label="总价分配">
                      <el-input v-model="textForm3.distribute" placeholder="请输入总价分配"></el-input>
                    </el-form-item>
                  </el-col>
                  <el-col :span="12">
                    <el-form-item label="活动名称">
                      <el-input v-model="textForm3.name" placeholder="请输入活动名称"></el-input>
                    </el-form-item>
                    <el-form-item label="总金额">
                      <el-input type="number" v-model="textForm3.price" :min="0" step="1" @input="checkTotalValue" placeholder="请输入总金额"></el-input>
                    </el-form-item>
                    <el-form-item label="活动地点">
                      <el-input v-model="textForm3.location" placeholder="请输入活动地点"></el-input>
                    </el-form-item>
                    <el-form-item label="预计活动时间">
                      <el-date-picker v-model="textForm3.time" type="datetime" style="width:100% !important;" placeholder="选择时间"></el-date-picker>
                    </el-form-item>
                  </el-col>
                </el-row>
                <!-- 在每个表单的el-button之前添加 -->
                <div class="voice-section" style="margin-bottom: 20px; text-align: center;">
                  <el-button type="success" @click="startVoiceRecognition(activeIndex)" :disabled="isRecording">
                    <i class="el-icon-microphone"></i>
                    {{ isRecording ? '识别中...' : '语音创建拼单' }}
                  </el-button>
                  <div v-if="voiceText" class="voice-result" style="margin-top: 10px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>
                </div>
                <el-button type="primary" @click="CreateOrder">创建拼单</el-button>
              </el-form>
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
import { Message, MessageBox } from 'element-ui'
import store from '../store'
import axios from 'axios'
import serverURL from '../server.config'

export default {
  data () {
    return {
      activeIndex: '1', // 默认激活的菜单项索引
      currentDate: this.getCurrentDate(), // 初始化时获取当前日期
      showContent: false,
      loading: false,
      textForm1: {
        title: '',
        description: '',
        platform: '',
        starttime: '',
        endtime: '',
        customValue: '1',
        name: '',
        location: '',
        price: '0',
        request: '',
        distribute: ''
      }, // 用于存储选择器的值  用于存储输入框的值
      textForm2: {
        title: '',
        description: '',
        request: '',
        customValue: '1',
        name: '',
        location1: '',
        location2: '',
        time: '',
        price: '0',
        starttime: '',
        endtime: '',
        distribute: ''
      },
      textForm3: {
        title: '',
        description: '',
        request: '',
        customValue: '1',
        name: '',
        location: '',
        time: '',
        price: '0',
        starttime: '',
        endtime: '',
        distribute: ''
      },
      // ... 现有数据
      isRecording: false,
      voiceText: '',
      recognition: null,
      isProcessingAI: false
    }
  },
  mounted () {
    // 每秒更新一次时钟
    setInterval(() => {
      this.currentDate = this.getCurrentDate()
    }, 1000)
    // 页面加载后延迟显示内容
    this.showContent = true
    this.initSpeechRecognition()
  },
  methods: {
    // 初始化语音识别
    initSpeechRecognition () {
      if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition
        this.recognition = new SpeechRecognition()
        this.recognition.continuous = false
        this.recognition.interimResults = false
        this.recognition.lang = 'zh-CN'

        this.recognition.onstart = () => {
          this.isRecording = true
          this.voiceText = ''
        }

        this.recognition.onresult = (event) => {
          const transcript = event.results[0][0].transcript
          this.voiceText = transcript
          this.processVoiceWithAI(transcript)
        }

        this.recognition.onerror = (event) => {
          this.isRecording = false
          this.$message.error('语音识别出错：' + event.error)
        }

        this.recognition.onend = () => {
          this.isRecording = false
        }
      } else {
        this.$message.error('您的浏览器不支持语音识别功能')
      }
    },

    // 开始语音识别
    startVoiceRecognition (formType) {
      if (!this.recognition) {
        this.initSpeechRecognition()
      }
      this.currentFormType = formType // 记录当前是哪个表单
      this.recognition.start()
    },

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
        this.$router.push({name: 'Login'})
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
        this.$router.push({name: 'Home'})
      }, 300) // 0.3秒后跳转，与动画时间保持一致
    },

    handleSelect (key, keyPath) {
      // 菜单项被选中时的处理函数
      this.activeIndex = key
    },

    checkCustomValue(customValue) {
      if (customValue < 1) {
        this.textForm1.customValue = 1
        this.textForm2.customValue = 1
        this.textForm3.customValue = 1
        // 刷新前端页面
        this.$forceUpdate()
      }
    },

    checkTotalValue (customValue) {
      if (customValue < 0) {
        this.textForm1.customValue = 0
        this.textForm2.customValue = 0
        this.textForm3.customValue = 0
        // 刷新前端页面
        this.$forceUpdate()
      }
    },

    getCreatetime () {
      const year = new Date().getFullYear()
      const now = new Date()
      const month = String(now.getMonth() + 1).padStart(2, '0')
      const day = String(now.getDate()).padStart(2, '0')
      const hours = String(now.getHours()).padStart(2, '0')
      const minutes = String(now.getMinutes()).padStart(2, '0')
      const seconds = String(now.getSeconds()).padStart(2, '0')
      return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
    },

    transISOTimetoLocalString (time) {
      const date = new Date(time)
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      const hours = String(date.getHours()).padStart(2, '0')
      const minutes = String(date.getMinutes()).padStart(2, '0')
      const seconds = String(date.getSeconds()).padStart(2, '0')
      return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
    },

    async CreateOrder() {
      this.loading = true
      if (!this.checkForm()) {
        this.loading = false
        return
      }
      const data = {}
      const now = this.getCreatetime()
      let startTime
      if (this.activeIndex === '1') {
        if (new Date(this.textForm1.starttime) < new Date(now)) {
          startTime = now
        } else {
          startTime = this.transISOTimetoLocalString(this.textForm1.starttime)
        }
        const requestJson = {
          groupOrder: {
            primeUserId: parseInt(store.state.user.userid),
            requestType: 'product',
            startTime: startTime,
            endTime: this.transISOTimetoLocalString(this.textForm1.endtime),
            title: this.textForm1.title,
            initiationTime: now,
            personNum: parseInt(this.textForm1.customValue),
            extraRequirement: this.textForm1.request,
            description: this.textForm1.description,
            priceDistribution: this.textForm1.distribute
          },
          project: {
            projectName: this.textForm1.name,
            totalPrice: parseInt(this.textForm1.price),
            requestType: 'product',
            specificInfo: {
              platform: this.textForm1.platform,
              address: this.textForm1.location
            }
          }
        }
        data.requestJson = requestJson
      } else if (this.activeIndex === '2') {
        if (new Date(this.textForm2.starttime) < new Date(now)) {
          startTime = now
        } else {
          startTime = this.transISOTimetoLocalString(this.textForm2.starttime)
        }
        const requestJson = {
          groupOrder: {
            primeUserId: parseInt(store.state.user.userid),
            requestType: 'traffic',
            startTime: startTime,
            endTime: this.transISOTimetoLocalString(this.textForm2.endtime),
            title: this.textForm2.title,
            initiationTime: now,
            personNum: parseInt(this.textForm2.customValue),
            extraRequirement: this.textForm2.request,
            description: this.textForm2.description,
            priceDistribution: this.textForm2.distribute
          },
          project: {
            projectName: this.textForm2.name,
            totalPrice: parseInt(this.textForm2.price),
            requestType: 'traffic',
            specificInfo: {
              originPlace: this.textForm2.location1,
              destinationPlace: this.textForm2.location2,
              trafficTime: this.transISOTimetoLocalString(this.textForm2.time)
            }
          }
        }
        data.requestJson = requestJson
      } else if (this.activeIndex === '3') {
        if (new Date(this.textForm3.starttime) < new Date(now)) {
          startTime = now
        } else {
          startTime = this.transISOTimetoLocalString(this.textForm3.starttime)
        }
        const requestJson = {
          groupOrder: {
            primeUserId: parseInt(store.state.user.userid),
            requestType: 'activity',
            startTime: startTime,
            endTime: this.transISOTimetoLocalString(this.textForm3.endtime),
            title: this.textForm3.title,
            initiationTime: now,
            personNum: parseInt(this.textForm3.customValue),
            extraRequirement: this.textForm3.request,
            description: this.textForm3.description,
            priceDistribution: this.textForm3.distribute
          },
          project: {
            projectName: this.textForm3.name,
            totalPrice: parseInt(this.textForm3.price),
            requestType: 'activity',
            specificInfo: {
              location: this.textForm3.location,
              activityTime: this.transISOTimetoLocalString(this.textForm3.time)
            }
          }
        }
        data.requestJson = requestJson
      }

      await new Promise(resolve => setTimeout(resolve, 1000))

      try {
        const createResponse = await axios.post(serverURL + '/v1/groupOrder/create', data, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + store.state.user.token
          }
        })
        if (createResponse.status === 200) {
          this.showContent = false
          // 等待动画完成后再跳转页面
          setTimeout(() => {
            this.$router.push({name: 'Home'})
          }, 300)
          this.$message.success('拼单创建成功')
        }
        this.loading = false
      } catch (error) {
        Message({
          type: 'error',
          message: '创建失败，请重试'
        })
        this.loading = false
      }
    },

    checkForm() {
      // 根据 activeIndex 的值决定检查哪个表单
      let form
      switch (this.activeIndex) {
        case '1':
          form = this.textForm1
          break
        case '2':
          form = this.textForm2
          break
        case '3':
          form = this.textForm3
          break
        default:
          return false
      }
      // 检查必填项是否为空
      for (let key in form) {
        if (form[key] === '' && key !== 'request') {
          Message({
            type: 'error',
            message: '请完整填写表格'
          })
          return false
        }
      }
      if (new Date(form.endtime) - new Date(form.starttime) < 3600000 || new Date(form.endtime) - new Date(this.getCreatetime()) < 3600000) {
        Message({
          type: 'error',
          message: '拼单关闭时间距离创建时间或开放时间至少应为1小时'
        })
        return false
      }
      if (new Date(form['time']) < new Date(form['endTime'])) {
        Message({
          type: 'error',
          message: '拼单项目进行时间应在拼单结束时间之前'
        })
        return false
      }
      return true
    },

    // 调用通义千问API处理语音内容
    async processVoiceWithAI(voiceText) {
      this.isProcessingAI = true
      this.loading = true

      try {
        const prompt = this.buildPrompt(voiceText, this.activeIndex)
        const result = await this.callTongyiAPI(prompt)
        this.fillFormWithAIResult(result)
        this.$message.success('AI识别完成，请检查并确认信息')
      } catch (error) {
        this.$message.error('AI处理失败：' + error.message)
      } finally {
        this.isProcessingAI = false
        this.loading = false
      }
    },

    // 构建给大模型的prompt
    buildPrompt(voiceText, formType) {
      const basePrompt = `请分析以下语音内容，提取出拼单信息并返回JSON格式数据。语音内容："${voiceText}"`

      let fieldsDescription = ''
      let jsonTemplate = ''

      if (formType === '1') { // 拼好物
        fieldsDescription = `
拼好物表单包含以下字段：
- title: 拼单标题
- description: 拼单描述
- name: 物品名称
- price: 总金额（数字）
- platform: 平台或商家
- location: 购物或收货地
- customValue: 预期人数（数字，默认1）
- request: 额外要求（可为空）
- distribute: 总价分配
- starttime: 开放时间（ISO格式，如果未提及则设为空字符串）
- endtime: 关闭时间（ISO格式，如果未提及则设为空字符串）`

        jsonTemplate = `{
  "title": "",
  "description": "",
  "name": "",
  "price": "",
  "platform": "",
  "location": "",
  "customValue": "1",
  "request": "",
  "distribute": "",
  "starttime": "",
  "endtime": ""
}`
      } else if (formType === '2') { // 拼交通
        fieldsDescription = `
拼交通表单包含以下字段：
- title: 拼单标题
- description: 拼单描述
- name: 交通名称
- price: 总金额（数字）
- location1: 始发地
- location2: 目的地
- customValue: 预期人数（数字，默认1）
- request: 额外要求（可为空）
- distribute: 总价分配
- starttime: 开放时间（ISO格式）
- endtime: 关闭时间（ISO格式）
- time: 预计出发时间（ISO格式）`

        jsonTemplate = `{
  "title": "",
  "description": "",
  "name": "",
  "price": "",
  "location1": "",
  "location2": "",
  "customValue": "1",
  "request": "",
  "distribute": "",
  "starttime": "",
  "endtime": "",
  "time": ""
}`
      } else { // 拼活动
        fieldsDescription = `
拼活动表单包含以下字段：
- title: 拼单标题
- description: 拼单描述
- name: 活动名称
- price: 总金额（数字）
- location: 活动地点
- customValue: 预期人数（数字，默认1）
- request: 额外要求（可为空）
- distribute: 总价分配
- starttime: 开放时间（ISO格式）
- endtime: 关闭时间（ISO格式）
- time: 预计活动时间（ISO格式）`

        jsonTemplate = `{
  "title": "",
  "description": "",
  "name": "",
  "price": "",
  "location": "",
  "customValue": "1",
  "request": "",
  "distribute": "",
  "starttime": "",
  "endtime": "",
  "time": ""
}`
      }

      return `${basePrompt}

${fieldsDescription}

请严格按照以下JSON格式返回，不要包含任何其他内容：
${jsonTemplate}

注意：
1. 如果语音中没有明确提到某个字段，请设为空字符串
2. 时间格式请使用ISO格式，如果语音中提到"明天下午3点"这样的相对时间，请转换为具体的ISO时间格式
3. 数字字段请返回字符串格式的数字
4. 只返回JSON，不要有任何解释文字`
    },

    // 调用通义千问API
    async callTongyiAPI(prompt) {
      try {
        const response = await fetch('http://localhost:3000/api/tongyi', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify({ prompt })
        })

        if (!response.ok) {
          const errorText = await response.text()
          throw new Error(`API调用失败: ${response.status} - ${errorText}`)
        }

        const data = await response.json()
        console.log('API返回原始数据:', data)

        // 标准方式验证数据结构
        if (!data || !data.choices || !data.choices.length ||
          !data.choices[0] || !data.choices[0].message ||
          !data.choices[0].message.content) {
          throw new Error('API返回格式不正确，无法获取内容')
        }

        // 获取API返回的内容文本
        const content = data.choices[0].message.content
        console.log('API返回文本内容:', content)

        // 确保jsonStr有初始值
        let jsonStr = content

        try {
          // 尝试提取Markdown代码块
          const codeBlockMatch = content.match(/```(?:json)?\s*([\s\S]*?)\s*```/)
          if (codeBlockMatch && codeBlockMatch[1]) {
            jsonStr = codeBlockMatch[1].trim()
            console.log('从Markdown提取JSON:', jsonStr)
          } else {
            console.log('无Markdown代码块，尝试直接解析文本')
          }

          // 解析前验证jsonStr不为空
          if (!jsonStr || jsonStr.trim() === '') {
            throw new Error('提取的JSON字符串为空')
          }

          // 解析JSON
          const parsedJson = JSON.parse(jsonStr)
          return parsedJson
        } catch (e) {
          console.error('JSON解析错误:', e)
          throw new Error('无法解析AI返回的JSON内容')
        }
      } catch (error) {
        console.error('API调用异常:', error)
        throw error
      }
    },

    // 将AI识别结果填入表单
    fillFormWithAIResult(result) {
      const formKey = `textForm${this.activeIndex}`
      const currentForm = this[formKey]

      console.log('[LOG] fillFormWithAIResult: 接收到的AI结果:', result)

      if (typeof result !== 'object' || result === null) {
        console.error('[LOG] fillFormWithAIResult: AI结果不是有效的对象:', result)
        this.$message.error('AI返回结果格式错误，无法填充表单。')
        return
      }

      Object.keys(result).forEach(key => {
        if (currentForm.hasOwnProperty(key)) {
          // 确保赋的值不是undefined，如果是，则赋空字符串或按需处理
          currentForm[key] = result[key] !== undefined ? result[key] : ''
          console.log(`[LOG] fillFormWithAIResult: 表单 ${formKey} 字段 ${key} 更新为:`, currentForm[key])
        } else {
          console.warn(`[LOG] fillFormWithAIResult: AI结果中的字段 "${key}" 在表单 ${formKey} 中不存在。`)
        }
      })
      this.$forceUpdate() // 强制更新视图
    }
  }
}
</script>

<style scoped>
.el-header {
  background-color: #B3C0D1;
  color: #333;
  line-height: 60px;
  padding: 0;
}

.el-menu-item {
  border: none;
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

.voice-section {
  border: 2px dashed #ddd;
  padding: 15px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.voice-result {
  font-size: 14px;
  color: #666;
  word-wrap: break-word;
}

.el-icon-microphone {
  margin-right: 5px;
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
  background-color: rgba(255, 255, 255, 0.5); /* 默认背景色 */
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
  background-color: rgba(255, 255, 255, 0.5); /* 默认背景色 */
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
