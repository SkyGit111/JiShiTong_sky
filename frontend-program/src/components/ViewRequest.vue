<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div class="title">
      <span>查看开放的拼单请求</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 class="page-title">查看开放的拼单请求</h1>
      <p class="page-description">探索系统中开放的拼单，并加入您意向的拼单请求。</p>
      <div class="content-box">
        <el-container style="height: 100%">
          <el-header style="padding: 0; display: flex; align-items: center; background-color: #B3C0D1; font-size: 12px ">
            <div style="flex-grow: 1;">
              <el-menu mode="horizontal" v-model="activeIndex" @select="handleSelect" background-color="#B3C0D1" text-color="#fff" active-text-color="#ffd04b" default-active="1" >
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
          <el-main v-loading="loading" element-loading-text="处理中" element-loading-spinner="el-icon-loading" element-loading-background="rgba(255, 255, 255, 0.6)">
            <div v-if="activeIndex === '1'">
              <template>
                <div class="open-pools">
                  <el-row :gutter="20">
                    <el-col :span="4">
                      <el-select v-model="searchType" placeholder="请选择" style="width: 100%;">
                        <el-option label="按发起用户搜索" value="creator"></el-option>
                        <el-option label="按拼单标题搜索" value="title"></el-option>
                      </el-select>
                    </el-col>
                    <el-col :span="16">
                      <el-input
                        placeholder="请输入关键词搜索拼单"
                        v-model="searchKeyword"
                        @input="handleSearch"
                        style="margin-bottom: 20px;">
                      </el-input>
                    </el-col>
                    <el-col :span="4">
                      <el-button type="success" @click="startVoiceRecognition" :disabled="isRecording" style="width: 100%;">
                        <i class="el-icon-microphone"></i>
                        {{ isRecording ? '识别中...' : '语音搜索' }}
                      </el-button>
                    </el-col>
                  </el-row>

                  <div v-if="voiceText" class="voice-result" style="margin-bottom: 20px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>

                  <!-- 拼单列表 -->
                <div class="table-container">
                  <el-table :data="filteredPools">
                    <el-table-column prop="title" label="拼单标题"></el-table-column>
                    <el-table-column prop="startUsername" label="发起用户"></el-table-column>
                    <el-table-column prop="total_price" label="总金额"></el-table-column>
                    <el-table-column prop="end_time" label="拼单结束时间"></el-table-column>
                    <el-table-column label="现有人数">
                      <template slot-scope="scope">
                        {{ Math.floor(scope.row.participants.length/11) }}
                      </template>
                    </el-table-column>
                    <el-table-column prop="person_num" label="最大人数"></el-table-column>
                    <el-table-column label="操作">
                      <template slot-scope="scope">
                        <el-button type="text" size="small" @click="showDetails(scope.row)">更多信息</el-button>
                        <el-button type="text" size="small" v-if="Math.floor(scope.row.participants.length/11) < scope.row.person_num" @click="applyJoin(scope.row)">加入拼单</el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                </div>
                  <!-- 拼单详情对话框 -->
                  <el-dialog :visible.sync="dialogVisible" title="拼单详情">
                    <div v-if="selectedPool">
                      <p><strong>拼单id: </strong> {{ selectedPool.request_id }}</p>
                      <p><strong>发起用户id: </strong> {{ selectedPool.prime_user_id }}</p>
                      <p><strong>创建时间：</strong> {{ selectedPool.initiation_time.replace('T', ' ') }}</p>
                      <p><strong>拼单开始时间：</strong> {{ selectedPool.start_time.replace('T', ' ') }}</p>
                      <p><strong>平台或商家:</strong> {{ selectedPool.specific_info.platform }}</p>
                      <p><strong>购物地或收货地:</strong> {{ selectedPool.specific_info.address }}</p>
                      <p><strong>额外要求:</strong> {{ selectedPool.extra_requirement }}</p>
                      <p><strong>总价分配:</strong> {{ selectedPool.price_distribution }}</p>
                    </div>
                  </el-dialog>
                </div>
              </template>
            </div>
            <div v-if="activeIndex === '2'">
              <template>
                <div class="open-pools">
                  <el-row :gutter="20">
                    <el-col :span="4">
                      <el-select v-model="searchType" placeholder="请选择" style="width: 100%;">
                        <el-option label="按发起用户搜索" value="creator"></el-option>
                        <el-option label="按拼单标题搜索" value="title"></el-option>
                      </el-select>
                    </el-col>
                    <el-col :span="16">
                      <el-input
                        placeholder="请输入关键词搜索拼单"
                        v-model="searchKeyword"
                        @input="handleSearch"
                        style="margin-bottom: 20px;">
                      </el-input>
                    </el-col>
                    <el-col :span="4">
                      <el-button type="success" @click="startVoiceRecognition" :disabled="isRecording" style="width: 100%;">
                        <i class="el-icon-microphone"></i>
                        {{ isRecording ? '识别中...' : '语音搜索' }}
                      </el-button>
                    </el-col>
                  </el-row>

                  <div v-if="voiceText" class="voice-result" style="margin-bottom: 20px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>

                  <!-- 拼单列表 -->
                <div class="table-container">
                  <el-table :data="filteredPools">
                    <el-table-column prop="title" label="拼单标题"></el-table-column>
                    <el-table-column prop="startUsername" label="发起用户"></el-table-column>
                    <el-table-column prop="total_price" label="总金额"></el-table-column>
                    <el-table-column prop="end_time" label="拼单结束时间" width="170px"></el-table-column>
                    <el-table-column label="现有人数">
                      <template slot-scope="scope">
                        {{ Math.floor(scope.row.participants.length/11) }}
                      </template>
                    </el-table-column>
                    <el-table-column prop="person_num" label="最大人数"></el-table-column>
                    <el-table-column prop="specific_info.trafficTime" label="预计出发时间" min-width="100"></el-table-column>
                    <el-table-column prop="specific_info.originPlace" label="出发地"></el-table-column>
                    <el-table-column prop="specific_info.destinationPlace" label="目的地"></el-table-column>
                    <el-table-column label="操作" min-width="100">
                      <template slot-scope="scope">
                        <el-button type="text" size="small" @click="showDetails(scope.row)">更多信息</el-button>
                        <el-button type="text" size="small"  v-if="Math.floor(scope.row.participants.length/11) < scope.row.person_num" @click="applyJoin(scope.row)">加入拼单</el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                </div>
                  <!-- 拼单详情对话框 -->
                  <el-dialog :visible.sync="dialogVisible" title="拼单详情">
                    <div v-if="selectedPool">
                      <p><strong>拼单id: </strong> {{ selectedPool.request_id }}</p>
                      <p><strong>发起用户id:</strong> {{ selectedPool.prime_user_id }}</p>
                      <p><strong>创建时间：</strong> {{ selectedPool.initiation_time.replace('T', ' ') }}</p>
                      <p><strong>拼单开始时间:</strong> {{ selectedPool.start_time.replace('T', ' ') }}</p>
                      <p><strong>额外要求:</strong> {{ selectedPool.extra_requirement }}</p>
                      <p><strong>总价分配:</strong> {{ selectedPool.price_distribution }}</p>
                    </div>
                  </el-dialog>
                </div>
              </template>
            </div>
            <div v-if="activeIndex === '3'">
              <template>
                <div class="open-pools">
                  <!-- 搜索框 -->
                  <el-row :gutter="20">
                    <el-col :span="4">
                      <el-select v-model="searchType" placeholder="请选择" style="width: 100%;">
                        <el-option label="按发起用户搜索" value="creator"></el-option>
                        <el-option label="按拼单标题搜索" value="title"></el-option>
                      </el-select>
                    </el-col>
                    <el-col :span="16">
                      <el-input
                        placeholder="请输入关键词搜索拼单"
                        v-model="searchKeyword"
                        @input="handleSearch"
                        style="margin-bottom: 20px;">
                      </el-input>
                    </el-col>
                    <el-col :span="4">
                      <el-button type="success" @click="startVoiceRecognition" :disabled="isRecording" style="width: 100%;">
                        <i class="el-icon-microphone"></i>
                        {{ isRecording ? '识别中...' : '语音搜索' }}
                      </el-button>
                    </el-col>
                  </el-row>

                  <div v-if="voiceText" class="voice-result" style="margin-bottom: 20px; padding: 10px; background: #f0f0f0; border-radius: 5px;">
                    <strong>识别内容：</strong>{{ voiceText }}
                  </div>

                  <!-- 拼单列表 -->
                  <div class="table-container">
                  <el-table :data="filteredPools">
                    <el-table-column prop="title" label="拼单标题"></el-table-column>
                    <el-table-column prop="startUsername" label="发起用户"></el-table-column>
                    <el-table-column prop="total_price" label="总金额"></el-table-column>
                    <el-table-column prop="end_time" label="拼单结束时间"></el-table-column>
                    <el-table-column label="现有人数">
                      <template slot-scope="scope">
                        {{ Math.floor(scope.row.participants.length/11) }}
                      </template>
                    </el-table-column>
                    <el-table-column prop="person_num" label="最大人数"></el-table-column>
                    <el-table-column prop="specific_info.activityTime" label="预计活动时间" min-width="100"></el-table-column>
                    <el-table-column label="操作">
                      <template slot-scope="scope">
                        <el-button type="text" size="small" @click="showDetails(scope.row)">更多信息</el-button>
                        <el-button type="text" size="small"  v-if="Math.floor(scope.row.participants.length/11) < scope.row.person_num" @click="applyJoin(scope.row)">加入拼单</el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                  </div>
                  <!-- 拼单详情对话框 -->
                  <el-dialog :visible.sync="dialogVisible" title="拼单详情">
                    <div v-if="selectedPool">
                      <p><strong>拼单id: </strong> {{ selectedPool.request_id }}</p>
                      <p><strong>发起用户id:</strong> {{ selectedPool.prime_user_id }}</p>
                      <p><strong>创建时间：</strong> {{ selectedPool.initiation_time.replace('T', ' ') }}</p>
                      <p><strong>拼单开始时间:</strong> {{ selectedPool.start_time.replace('T', ' ') }}</p>
                      <p><strong>额外要求:</strong> {{ selectedPool.extra_requirement }}</p>
                      <p><strong>总价分配:</strong> {{ selectedPool.price_distribution }}</p>
                      <p><strong>活动地点：</strong> {{ selectedPool.specific_info.location }}</p>
                    </div>
                  </el-dialog>
                </div>
              </template>
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
      searchKeyword: '',
      activeIndex: '1', // 默认激活的菜单项索引
      pool: {
        1: [
          // 假设的拼单数据
          {
            id: '1',
            createTime: '2021-01-01 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户A',
            type: '拼好物',
            title: '食品拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '2',
            createTime: '2021-01-02 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户B',
            type: '拼好物',
            title: '衣服拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '1',
            createTime: '2021-01-01 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户A',
            type: '拼好物',
            title: '食品拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '2',
            createTime: '2021-01-02 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户B',
            type: '拼好物',
            title: '衣服拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '1',
            createTime: '2021-01-01 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户A',
            type: '拼好物',
            title: '食品拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '2',
            createTime: '2021-01-02 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户B',
            type: '拼好物',
            title: '衣服拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '1',
            createTime: '2021-01-01 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户A',
            type: '拼好物',
            title: '食品拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '2',
            createTime: '2021-01-02 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户B',
            type: '拼好物',
            title: '衣服拼单',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          }
          // 更多拼单...
        ],
        2: [
          // 假设的拼单数据
          {
            id: '1',
            createTime: '2021-01-01 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户A',
            type: '拼交通',
            title: '交通拼单1',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          },
          {
            id: '2',
            createTime: '2021-01-02 10:00',
            starttime: '2024-01-01 10:00',
            endtime: '2024-01-01 10:00',
            activeTime: '2024-01-01 10:00',
            creator: '用户B',
            type: '拼交通',
            title: '交通拼单2',
            nowPeople: 3,
            maxPeople: 5,
            price: 500,
            request: '无额外要求',
            distribute: '均分'
          }
          // 更多拼单...
        ],
        3: []
      },
      prime_user_id: [],
      temp1: [],
      temp2: [],
      temp3: [],
      test1: [],
      test2: [],
      test3: [],
      productPool: [],
      trafficPool: [],
      activityPool: [],
      productId: [],
      activityId: [],
      trafficId: [],
      selectedPool: null,
      searchType: 'title', // 设置默认搜索类型为title
      dialogVisible: false,
      isRecording: false,
      voiceText: '',
      recognition: null,
      loading: false,
      isProcessingAI: false,
      filteredPools: [] // 确保初始化filteredPools
    }
  },
  created() {
    this.loadData();
  },
  mounted () {
    // 每秒更新一次时钟
    setInterval(() => {
      this.currentDate = this.getCurrentDate()
    }, 1000)
    // 页面加载后延迟显示内容
    this.showContent = true
    // 初始化语音识别
    this.initSpeechRecognition()
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
    handleSelect (key) {
      // 菜单项被选中时的处理函数
      this.activeIndex = key
      if (this.activeIndex === '1') {
        this.filteredPools = this.productPool
      } else if (this.activeIndex === '2') {
        this.filteredPools = this.trafficPool
      } else if (this.activeIndex === '3') {
        this.filteredPools = this.activityPool
      }
      this.searchKeyword = '' // 重置搜索关键词
      this.handleSearch() // 重新搜索以更新filteredPools
    },
    handleSearch () {
      let currentPool
      if (this.activeIndex === '1') {
        currentPool = this.productPool
      } else if (this.activeIndex === '2') {
        currentPool = this.trafficPool
      } else {
        currentPool = this.activityPool
      }
      if (this.searchKeyword) {
        this.filteredPools = currentPool.filter(pool => {
          return (this.searchType === 'creator' && pool.startUsername.includes(this.searchKeyword)) ||
            (this.searchType === 'title' && pool.title.includes(this.searchKeyword))
        })
      } else {
        this.filteredPools = currentPool
      }
    },
    showDetails (pool) {
      this.selectedPool = pool
      this.dialogVisible = true
    },
    async applyJoin (pool) {
      // 发送申请加入的信息
      for (let i = 0; i < Math.floor(pool.participants.length) / 11; i++) {
        if (pool.participants.slice(1 + 11 * i, 10 + 11 * i) === store.state.user.userid) {
          this.$message.error('您已经加入过该拼单')
          return
        }
      }
      try {
        const joinResponse = await axios.post(serverURL + `/v1/groupOrder/participate?userId=${store.state.user.userid}&requestId=${pool.request_id}`, {}, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (joinResponse.status === 200) {
          this.$message.success('成功加入拼单')
          setTimeout(() => {
            this.$router.go(0)
          }, 300)
        }
      } catch (error) {
        this.$message.error('加入拼单失败')
      }
    },
    startVoiceRecognition() {
      if (!this.recognition) {
        this.initSpeechRecognition();
      }

      this.isRecording = true;
      this.voiceText = '';
      this.recognition.start();
    },

    // 初始化语音识别
    initSpeechRecognition() {
      if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        this.recognition = new SpeechRecognition();
        this.recognition.continuous = false;
        this.recognition.interimResults = false;
        this.recognition.lang = 'zh-CN';

        this.recognition.onstart = () => {
          this.isRecording = true;
          this.voiceText = '';
        };

        this.recognition.onresult = (event) => {
          const transcript = event.results[0][0].transcript;
          this.voiceText = transcript;
          this.processVoiceWithAI(transcript);
        };

        this.recognition.onerror = (event) => {
          this.isRecording = false;
          this.$message.error('语音识别出错：' + event.error);
        };

        this.recognition.onend = () => {
          this.isRecording = false;
        };
      } else {
        this.$message.error('您的浏览器不支持语音识别功能');
      }
    },

    // 调用通义千问API处理语音内容
    async processVoiceWithAI(voiceText) {
      this.isProcessingAI = true;
      this.loading = true;

      try {
        const prompt = this.buildPrompt(voiceText);
        const result = await this.callTongyiAPI(prompt);
        this.applySearchResult(result);
        this.$message.success('AI识别完成，搜索结果已更新');
      } catch (error) {
        this.$message.error('AI处理失败：' + error.message);
      } finally {
        this.isProcessingAI = false;
        this.loading = false;
      }
    },

    // 构建给大模型的prompt
    buildPrompt(voiceText) {
      return `请分析以下语音内容，提取出拼单搜索信息并返回JSON格式数据。语音内容："${voiceText}"

搜索类型包括：
- searchType: 搜索类型，可选值为"creator"（按发起用户搜索）或"title"（按拼单标题搜索）
- keyword: 搜索关键词

请严格按照以下JSON格式返回，不要包含任何其他内容：
{
  "searchType": "",
  "keyword": ""
}

注意：
1. 如果语音中没有明确提到搜索类型，默认使用"title"（按拼单标题搜索）
2. 如果语音中没有明确搜索关键词，则返回空字符串
3. 只返回JSON，不要有任何解释文字`;
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
        });

        if (!response.ok) {
          const errorText = await response.text();
          throw new Error(`API调用失败: ${response.status} - ${errorText}`);
        }

        const data = await response.json();
        console.log('API返回原始数据:', data);

        // 标准方式验证数据结构
        if (!data || !data.choices || !data.choices.length ||
          !data.choices[0] || !data.choices[0].message ||
          !data.choices[0].message.content) {
          throw new Error('API返回格式不正确，无法获取内容');
        }

        // 获取API返回的内容文本
        const content = data.choices[0].message.content;
        console.log('API返回文本内容:', content);

        // 确保jsonStr有初始值
        let jsonStr = content;

        try {
          // 尝试提取Markdown代码块
          const codeBlockMatch = content.match(/```(?:json)?\s*([\s\S]*?)\s*```/);
          if (codeBlockMatch && codeBlockMatch[1]) {
            jsonStr = codeBlockMatch[1].trim();
            console.log('从Markdown提取JSON:', jsonStr);
          } else {
            console.log('无Markdown代码块，尝试直接解析文本');
          }

          // 解析前验证jsonStr不为空
          if (!jsonStr || jsonStr.trim() === '') {
            throw new Error('提取的JSON字符串为空');
          }

          // 解析JSON
          const parsedJson = JSON.parse(jsonStr);
          return parsedJson;
        } catch (e) {
          console.error('JSON解析错误:', e);
          throw new Error('无法解析AI返回的JSON内容');
        }
      } catch (error) {
        console.error('API调用异常:', error);
        throw error;
      }
    },

    // 应用搜索结果
    applySearchResult(result) {
      console.log('应用搜索结果:', result);

      if (typeof result !== 'object' || result === null) {
        console.error('AI结果不是有效的对象:', result);
        this.$message.error('AI返回结果格式错误，无法应用搜索条件。');
        return;
      }

      // 设置搜索类型（如果有）
      if (result.searchType && (result.searchType === 'creator' || result.searchType === 'title')) {
        this.searchType = result.searchType;
      } else if (result.searchType) {
        // 如果值不是有效的选项，尝试智能匹配
        if (result.searchType.includes('用户') || result.searchType.includes('发起')) {
          this.searchType = 'creator';
        } else {
          this.searchType = 'title'; // 默认为按标题搜索
        }
      }

      // 设置搜索关键词（如果有）
      if (result.keyword) {
        this.searchKeyword = result.keyword;
      }

      // 执行搜索
      this.handleSearch();
    },
    async loadData() {
      // 好物初始化
      try {
        const productIdResponse = await axios.get(serverURL + `/v1/groupOrder/getOpenProduct?userId=${store.state.user.userid}`, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (productIdResponse.status === 200) {
          this.productId = productIdResponse.data.data
        }
      } catch (error) {
        this.$message.error('获取好物拼单号失败')
      }
      for (let i = 0; i < this.productId.length; i++) {
        this.test1[i] = this.productId[i].request_id
      }
      try {
        const groupOrderResponse1 = await axios.post(serverURL + '/v1/groupOrder/getmultipleinformation', this.test1, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + this.$store.state.user.token
          }
        })
        if (groupOrderResponse1.status === 200) {
          this.temp1 = groupOrderResponse1.data.data
          for (let i = 0; i < this.temp1.length; i++) {
            this.temp1[i][0].start_time = this.temp1[i][0].start_time.replace('T', ' ')
            this.temp1[i][0].end_time = this.temp1[i][0].end_time.replace('T', ' ')
            this.productPool[i] = this.temp1[i][0]
            this.prime_user_id[i] = this.temp1[i][0].prime_user_id
          }
        }
      } catch (error) {
        this.$message.error('获取拼单标题失败')
      }
      for (let i = 0; i < this.prime_user_id.length; i++) {
        try {
          const usernameResponse1 = await axios.get(serverURL + `/v1/user/getinformation?userId=${this.prime_user_id[i]}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (usernameResponse1.status === 200) {
            this.productPool[i].startUsername = usernameResponse1.data.user_name
          }
        } catch (error) {
          this.$message.error('获取发起用户失败')
        }
      }
      // 交通初始化
      try {
        const trafficIdResponse = await axios.get(serverURL + `/v1/groupOrder/getOpenTraffic?userId=${store.state.user.userid}`, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (trafficIdResponse.status === 200) {
          this.trafficId = trafficIdResponse.data.data
        }
      } catch (error) {
        this.$message.error('获取交通拼单号失败')
      }
      for (let i = 0; i < this.trafficId.length; i++) {
        this.test2[i] = this.trafficId[i].request_id
      }
      try {
        const groupOrderResponse2 = await axios.post(serverURL + `/v1/groupOrder/getmultipleinformation`, this.test2, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + this.$store.state.user.token
          }
        })
        if (groupOrderResponse2.status === 200) {
          this.temp2 = groupOrderResponse2.data.data
          for (let i = 0; i < this.temp1.length; i++) {
            this.temp2[i][0].start_time = this.temp2[i][0].start_time.replace('T', ' ')
            this.temp2[i][0].end_time = this.temp2[i][0].end_time.replace('T', ' ')
            this.temp2[i][0].specific_info.trafficTime = this.temp2[i][0].specific_info.trafficTime.replace('T', ' ')
            this.trafficPool[i] = this.temp2[i][0]
            this.prime_user_id[i] = this.temp2[i][0].prime_user_id
          }
        }
      } catch (error) {}
      for (let i = 0; i < this.prime_user_id.length; i++) {
        try {
          const usernameResponse1 = await axios.get(serverURL + `/v1/user/getinformation?userId=${this.prime_user_id[i]}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (usernameResponse1.status === 200) {
            this.trafficPool[i].startUsername = usernameResponse1.data.user_name
          }
        } catch (error) {}
      }
      // 活动初始化
      try {
        const activityIdResponse = await axios.get(serverURL + `/v1/groupOrder/getOpenActivity?userId=${store.state.user.userid}`, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (activityIdResponse.status === 200) {
          this.activityId = activityIdResponse.data.data
        }
      } catch (error) {
        this.$message.error('获取活动拼单号失败')
      }
      for (let i = 0; i < this.activityId.length; i++) {
        this.test3[i] = this.activityId[i].request_id
      }
      try {
        const groupOrderResponse3 = await axios.post(serverURL + `/v1/groupOrder/getmultipleinformation`, this.test3, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + this.$store.state.user.token
          }
        })
        if (groupOrderResponse3.status === 200) {
          this.temp3 = groupOrderResponse3.data.data
          for (let i = 0; i < this.temp3.length; i++) {
            this.temp3[i][0].start_time = this.temp3[i][0].start_time.replace('T', ' ')
            this.temp3[i][0].end_time = this.temp3[i][0].end_time.replace('T', ' ')
            this.temp3[i][0].specific_info.activityTime = this.temp3[i][0].specific_info.activityTime.replace('T', ' ')
            this.activityPool[i] = this.temp3[i][0]
            this.prime_user_id[i] = this.temp3[i][0].prime_user_id
          }
        }
      } catch (error) {}
      for (let i = 0; i < this.prime_user_id.length; i++) {
        try {
          const usernameResponse1 = await axios.get(serverURL + `/v1/user/getinformation?userId=${this.prime_user_id[i]}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (usernameResponse1.status === 200) {
            this.activityPool[i].startUsername = usernameResponse1.data.user_name
          }
        } catch (error) {}
      }
      this.filteredPools = this.productPool
      this.$forceUpdate()
    }
  }
}
</script>

<style scoped>
.table-container {
  overflow-y: auto;
  max-height: 400px;
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
</style>
