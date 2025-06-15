<template>
  <div class="home-page">
    <div class="top-overlay"></div>

    <div class="clock">
      <span>{{ currentDate }}</span>
    </div>

    <div v-if="store.state.user.userrole === 'student'" class="title">
      <span>管理个人相关的拼单请求</span>
    </div>
    <div v-if="store.state.user.userrole === 'admin'" class="title">
      <span>管理所有拼单请求</span>
    </div>

    <button @click="logout" class="logout-button">退出</button>
    <button @click="back" class="back-button">返回</button>

    <div class="content" :class="{ fadeIn: showContent, fadeOut: !showContent }">
      <h1 v-if="store.state.user.userrole === 'student'" class="page-title">管理个人相关的拼单请求</h1>
      <h1 v-if="store.state.user.userrole === 'admin'" class="page-title">管理所有拼单请求</h1>
      <p v-if="store.state.user.userrole === 'student'" class="page-description">管理个人加入或发起的拼单，可以退出、修改或删除。</p>
      <p v-if="store.state.user.userrole === 'admin'" class="page-description">管理系统中所有的拼单请求，可以查看、修改或删除。</p>
      <div class="content-box">
        <el-container style="height: 100%">
          <el-header style="padding: 0; display: flex; align-items: center; background-color: #B3C0D1; font-size: 12px">
            <div style="flex-grow: 1;">
              <el-menu mode="horizontal" v-model="activeIndex" @select="handleSelect" background-color="#B3C0D1" text-color="#fff" active-text-color="#ffd04b" default-active="1" >
                <el-menu-item v-if="store.state.user.userrole === 'student'" index="1">
                  <template slot="title"><i class="el-icon-edit"></i>我创建的拼单</template>
                </el-menu-item>
                <el-menu-item v-if="store.state.user.userrole === 'student'" index="2">
                  <template slot="title"><i class="el-icon-plus"></i>我加入的拼单</template>
                </el-menu-item>
                <el-menu-item v-if="store.state.user.userrole === 'admin'" index="1">
                  <template slot="title"><i class="el-icon-edit"></i>所有拼单请求</template>
                </el-menu-item>
              </el-menu>
            </div>
          </el-header>
          <el-main>
              <template>
                <div class="personal-pools">
                  <el-input
                    placeholder="请输入关键词搜索拼单"
                    v-model="searchKeyword"
                    @input="handleSearch"
                    style="margin-bottom: 20px;">
                  </el-input>
                  <div class="table-container">
                  <div v-if="activeIndex === '1'">
                  <el-table  :data="filteredPools" style="width: 100%" >
                    <el-table-column prop="request_id" label="拼单ID" ></el-table-column>
                    <el-table-column prop="title" label="拼单标题" ></el-table-column>
                    <el-table-column prop="request_type" label="拼单类型" ></el-table-column>
                    <el-table-column label="现有人数">
                      <template slot-scope="scope">
                        {{ Math.floor(scope.row.participants.length/11) }}
                      </template>
                    </el-table-column>
                    <el-table-column prop="person_num" label="最大人数"></el-table-column>
                    <el-table-column prop="request_states" label="拼单状态"></el-table-column>
                    <el-table-column label="操作" width="200">
                      <template slot-scope="scope">
                        <el-button type="text" size="small" @click="showDetails(scope.row)">更多信息</el-button>
                        <el-button type="text" size="small" @click="showEditDialog(scope.row)">修改拼单</el-button>
                        <el-button type="text" size="small" @click="confirmDelete(scope.row)">删除拼单</el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                  </div>
                  <div v-if="activeIndex === '2'">
                    <el-table  :data="filteredPools" style="width: 100%" >
                    <el-table-column prop="request_id" label="拼单ID" ></el-table-column>
                    <el-table-column prop="title" label="拼单标题" ></el-table-column>
                    <el-table-column prop="request_type" label="拼单类型" ></el-table-column>
                    <el-table-column label="现有人数">
                      <template slot-scope="scope">
                        {{ Math.floor(scope.row.participants.length/11) }}
                      </template>
                    </el-table-column>
                    <el-table-column prop="person_num" label="最大人数"></el-table-column>
                    <el-table-column prop="request_states" label="拼单状态"></el-table-column>
                    <el-table-column label="操作" width="200">
                      <template slot-scope="scope">
                        <el-button type="text" size="small" @click="showDetails(scope.row)">更多信息</el-button>
                        <el-button type="text" size="small" @click="confirmExit(scope.row)">退出拼单</el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                  </div>
                  </div>
                </div>
              </template>
            <el-dialog :visible.sync="dialogVisibleDetails" title="拼单详情">
                    <div v-if="selectedPool">
                      <div v-if="selectedPool.prime_user_id !== parseInt(store.state.user.userid)">
                        <p><strong>创建用户id:</strong> {{ selectedPool.prime_user_id }}</p>
                        <p><strong>创建用户:</strong> {{ selectedPool.startUsername }}</p>
                      </div>
                      <p><strong>拼单描述:</strong> {{ selectedPool.description }}</p>
                      <p><strong>创建时间:</strong> {{ selectedPool.initiation_time.replace('T', ' ') }}</p>
                      <p><strong>总金额:</strong> {{ selectedPool.total_price }}</p>
                      <p><strong>拼单开始时间:</strong> {{ selectedPool.start_time.replace('T', ' ') }}</p>
                      <p><strong>拼单结束时间:</strong> {{ selectedPool.end_time.replace('T', ' ') }}</p>
                      <div v-if="selectedPool.request_type === 'product'">
                        <p><strong>平台或商家：</strong>{{ selectedPool.specific_info.platform}}</p>
                        <p><strong>购物地或收货地：</strong>{{ selectedPool.specific_info.address }}</p>
                      </div>
                      <div v-if="selectedPool.request_type === 'traffic'">
                        <p><strong>出发地：</strong>{{ selectedPool.specific_info.originPlace }}</p>
                        <p><strong>目的地：</strong>{{ selectedPool.specific_info.destinationPlace}}</p>
                        <p><strong>预计出发时间：</strong>{{ selectedPool.specific_info.trafficTime}}</p>
                      </div>
                      <div v-if="selectedPool.request_type === 'activity'">
                        <p><strong>活动地点：</strong>{{ selectedPool.specific_info.location }}</p>
                        <p><strong>预计活动时间：</strong>{{ selectedPool.specific_info.activityTime}}</p>
                      </div>
                        <p><strong>已加入人数:</strong> {{ Math.floor(selectedPool.participants.length/11) }}</p>
                        <p><strong>额外要求:</strong> {{ selectedPool.extra_requirement }}</p>
                        <p><strong>总价分配:</strong> {{ selectedPool.price_distribution }}</p>
                        <p><strong>拼单状态:</strong> {{ selectedPool.request_states }}</p>
                    </div>
                  </el-dialog>
                  <el-dialog :visible.sync="dialogVisibleEdit" title="修改拼单">
                    <div v-if="selectedPool">
                      <el-form :model="selectedPool" label-width="120px">
                        <el-form-item label="拼单名称">
                          <el-input v-model="selectedPool.title"></el-input>
                        </el-form-item>
                        <el-form-item label="拼单描述">
                          <el-input v-model="selectedPool.description"></el-input>
                        </el-form-item>
                        <el-form-item label="总金额">
                          <el-input v-model="selectedPool.total_price"></el-input>
                        </el-form-item>
                        <el-form-item label="最大人数">
                          <el-input type="number" :min="1" step="1" v-model="selectedPool.person_num"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'product'" label="平台或商家">
                          <el-input v-model="selectedPool.specific_info.platform"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'product'" label="购物地或收货地">
                          <el-input v-model="selectedPool.specific_info.address"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'traffic'" label="出发地">
                          <el-input v-model="selectedPool.specific_info.originPlace"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'traffic'" label="目的地">
                          <el-input v-model="selectedPool.specific_info.destinationPlace"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'traffic'" label="预计出发时间">
                          <el-input v-model="selectedPool.specific_info.trafficTime"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'activity'" label="活动地点">
                          <el-input v-model="selectedPool.specific_info.location"></el-input>
                        </el-form-item>
                        <el-form-item  v-if="selectedPool.request_type === 'activity'" label="预计活动时间">
                          <el-input v-model="selectedPool.specific_info.activityTime"></el-input>
                        </el-form-item>
                        <el-form-item label="额外要求">
                          <el-input v-model="selectedPool.extra_requirement"></el-input>
                        </el-form-item>
                        <el-form-item label="总价分配">
                          <el-input v-model="selectedPool.price_distribution"></el-input>
                        </el-form-item>
                        <!-- 选择参与者 -->
                        <!-- 修改拼单状态 -->
                        <el-form-item label="拼单状态">
                          <el-select v-model="newState" placeholder="请选择拼单状态" style="width:100% !important;">
                            <el-option
                              v-for="item in options"
                              :key="item.value"
                              :label="item.label"
                              :value="item.value">
                            </el-option>
                          </el-select>
                        </el-form-item>
                      </el-form>
                      <span slot="footer" class="dialog-footer">
                      <el-button @click="dialogVisibleEdit = false">取消</el-button>
                      <el-button type="primary" @click="confirmEdit(selectedPool)">确认修改</el-button>
                      </span>
                    </div>
                  </el-dialog>
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
  computed: {
    joiner () {
      return this.getjoiner(this.selectedPool)
    },
    store () {
      return store
    }
  },
  data () {
    return {
      newState: '保持',
      options: [{
        value: '保持',
        label: '保持现状'
      }, {
        value: '更改',
        label: '更改状态'
      }, {
        value: '自动',
        label: '自动判断'
      }],
      value: '',
      currentDate: this.getCurrentDate(), // 初始化时获取当前日期
      showContent: false,
      activeIndex: '1', // 默认激活的菜单项索引
      searchKeyword: '',
      state: '',
      hahah: [{user_id: 100000011, user_name: 'zjz', user_contact: '', user_age: 18, user_introduction: ''}, {user_id: 100000011, user_name: 'zjz', user_contact: '', user_age: 18, user_introduction: ''}],
      createdPools: [
        // 假设的我创建的拼单数据
        {
          id: '1',
          name: '拼单1',
          type: '拼好物',
          title: '',
          price: 100,
          nowpeople: 3,
          peopleLimit: 5,
          state: 0,
          creatorName: store.state.user.username,
          createtime: '2021-06-01 12:00',
          starttime: '2021-06-02 12:00',
          endtime: '2021-06-03 12:00',
          platform: '淘宝',
          location: '上海市',
          request: 'xxx',
          distribute: '均分',
          joinedUsers: [{id: '1', name: '用户1'}, {id: '2', name: '用户2'}, {id: '3', name: '用户3'}],
          isCreator: true
        },
        {
          id: '2',
          name: '拼单2',
          type: '拼交通',
          price: 100,
          nowpeople: 3,
          peopleLimit: 5,
          state: 1,
          creatorName: store.state.user.username,
          createtime: '2021-06-01 12:00',
          starttime: '2021-06-02 12:00',
          endtime: '2021-06-03 12:00',
          location1: '上海市',
          location2: '北京市',
          activetime: '2021-06-02 12:00',
          request: 'xxx',
          distribute: '均分',
          joinedUsers: [{id: '1', name: '用户1'}, {id: '2', name: '用户2'}, {id: '3', name: '用户3'}],
          isCreator: true
        },
        {
          id: '3',
          name: '拼单3',
          type: '拼活动',
          price: 100,
          nowpeople: 3,
          peopleLimit: 5,
          state: 1,
          creatorName: store.state.user.username,
          createtime: '2021-06-01 12:00',
          starttime: '2021-06-02 12:00',
          endtime: '2021-06-03 12:00',
          location: '上海市',
          activetime: '2021-06-02 12:00',
          request: 'xxx',
          distribute: '均分',
          joinedUsers: [{id: '1', name: '用户1'}, {id: '2', name: '用户2'}, {id: '3', name: '用户3'}],
          isCreator: true
        },
        {
          id: '4',
          name: '拼单4',
          type: '拼yy',
          state: 0,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        },
        {
          id: '5',
          name: '拼单5',
          type: '拼好物',
          state: 1,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        },
        {
          id: '1',
          name: '拼单1',
          type: '拼好物',
          state: 0,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        },
        {
          id: '2',
          name: '拼单2',
          type: '拼交通',
          state: 1,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        },
        {
          id: '3',
          name: '拼单3',
          type: '拼活动',
          state: 0,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        },
        {
          id: '4',
          name: '拼单4',
          type: '拼yy',
          state: 0,
          joinedUsers: [{id: '1', name: '用户1'}],
          creatorName: '我',
          isCreator: true
        }

        // 更多我创建的拼单...
      ],
      joinedPools: [
        // 假设的我加入的拼单数据
        {
          id: '5',
          name: '拼单5',
          type: '拼好物',
          state: 1,
          joinedUsers: [{id: '2', name: '用户2'}],
          creatorName: '用户A',
          isCreator: false
        }
        // 更多我加入的拼单...
      ],
      filteredPools: [], // 当前显示的拼单列表
      filteredCreatedPools: [],
      filteredJoinedPools: [],
      filteredCreatedId: [],
      filteredJoinedId: [],
      test1: [],
      test2: [],
      temp1: [],
      temp2: [],
      prime_user_id: [],
      dialogVisibleDetails: false,
      dialogVisibleEdit: false,
      selectedPool: null, // 当前选中的拼单
      selectedUserToRemove: null // 用于存储要删除的参与者 ID
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
        this.filteredPools = this.filteredCreatedPools
      } else if (this.activeIndex === '2') {
        this.filteredPools = this.filteredJoinedPools
      }
      for (let i = 0; i < this.filteredPools.length; i++) {
        if (this.filteredPools[i].request_type === 'product') {
          this.filteredPools[i].request_type = '拼好物'
        } else if (this.filteredPools[i].request_type === 'traffic') {
          this.filteredPools[i].request_type = '拼交通'
        } else if (this.filteredPools[i].request_type === 'activity') {
          this.filteredPools[i].request_type = '拼活动'
        }
      }
      this.searchKeyword = '' // 重置搜索关键词
      this.handleSearch() // 重新搜索以更新filteredPools
    },
    handleSearch () {
      let currentPool
      if (this.activeIndex === '1') {
        currentPool = this.filteredCreatedPools
      } else if (this.activeIndex === '2') {
        currentPool = this.filteredJoinedPools
      }
      if (this.searchKeyword) {
        this.filteredPools = currentPool.filter(pool => {
          return (pool.title.includes(this.searchKeyword))
        })
      } else {
        this.filteredPools = currentPool
      }
    },
    showDetails (pool) {
      this.selectedPool = pool
      this.dialogVisibleDetails = true
    },
    // 显示修改拼单的弹窗
    showEditDialog (pool) {
      this.selectedPool = pool
      this.dialogVisibleEdit = true
    },
    async getjoiner (pool) {
      let joiner = []
      for (let i = 0; i < JSON.parse(pool.participants).length; i++) {
        try {
          const usernameResponse1 = await axios.get(serverURL + `/v1/user/getinformation?userId=${JSON.parse(pool.participants)[i]}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (usernameResponse1.status === 200) {
            let user = {
              user_id: usernameResponse1.data.user_id,
              user_name: usernameResponse1.data.user_name,
              user_contact: usernameResponse1.data.user_contact,
              user_age: usernameResponse1.data.user_age,
              user_introduction: usernameResponse1.data.user_introduction
            }
            joiner.push(user) // 将新对象推入 joiner 数组
          }
        } catch (error) {
          this.$message.error('失败')
        }
      }
      console.log('Joiner:', joiner)
      return joiner
    },
    // 退出拼单
    async confirmExit (pool) {
      try {
        const exitResponse = await axios.post(serverURL + `/v1/groupOrder/quit?userId=${store.state.user.userid}&requestId=${pool.request_id}`, {}, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (exitResponse.status === 200) {
          this.$message.success('退出拼单成功')
          setTimeout(() => {
            this.$router.go(0)
          }, 300)
        }
      } catch (error) {
        this.$message.error('退出拼单失败')
      }
    },
    // 删除拼单
    confirmDelete (pool) {
      MessageBox.confirm('您确定要删除该拼单吗?', '提示:', {
        confirmButtonText: '是的',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(async () => {
        try {
          const deleteResponse = await axios.delete(serverURL + `/v1/groupOrder/delete?requestId=${pool.request_id}`, {
            headers: {
              Authorization: 'Bearer ' + this.$store.state.user.token
            }
          })
          if (deleteResponse.status === 200) {
            this.$message.success('拼单已删除')
            setTimeout(() => {
              this.$router.go(0)
            }, 300)
          }
        } catch (error) {
          this.$message.error('删除拼单失败')
        }
      })
    },
    // 修改拼单参与者
    removeParticipant () {
      if (this.selectedUserToRemove) {
        this.selectedPool.joinedUsers = this.selectedPool.joinedUsers.filter(user => user.id !== this.selectedUserToRemove)
        this.selectedUserToRemove = null // 重置选中的参与者
        this.$message.success('参与者已删除')
      }
    },
    async confirmEdit (pool) {
      if (this.newState === '更改') {
        if (pool.request_states === '满员' || pool.request_states === '关闭') {
          pool.request_states = 'open'
        } else {
          pool.request_states = 'close'
        }
      } else if (this.newState === '自动') {
        pool.request_states = 'normal'
      }
      const data = {
        'modifyRequest': {
          'groupOrder': {
            'requestId': pool.request_id,
            'primeUserId': pool.prime_user_id,
            'startTime': pool.start_time,
            'endTime': pool.end_time,
            'requestType': pool.request_type,
            'initiationTime': pool.initiation_time,
            'title': pool.title,
            'personNum': parseInt(pool.person_num),
            'description': pool.description,
            'priceDistribution': pool.price_distribution,
            'extraRequirement': pool.extra_requirement
          },
          'project': {
            'projectName': pool.project_name,
            'totalPrice': parseInt(pool.total_price),
            'requestType': pool.request_type
          }
        }
      }
      if (pool.request_type === '拼交通') {
        data.modifyRequest.groupOrder.requestType = 'traffic'
        data.modifyRequest.project.requestType = 'traffic'
      }
      if (pool.request_type === '拼好物') {
        data.modifyRequest.groupOrder.requestType = 'product'
        data.modifyRequest.project.requestType = 'product'
      }
      if (pool.request_type === '拼活动') {
        data.modifyRequest.groupOrder.requestType = 'activity'
        data.modifyRequest.project.requestType = 'activity'
      }
      if (pool.request_states === 'close' || pool.request_states === 'normal' || pool.request_states === 'open') {
        data.modifyRequest.groupOrder.requestStates = pool.request_states
      }
      if (data.modifyRequest.project.requestType === 'product') {
        data.modifyRequest.project.specificInfo = {
          'platform': pool.specific_info.platform,
          'address': pool.specific_info.address
        }
      } else
      if (data.modifyRequest.project.requestType === 'traffic') {
        data.modifyRequest.project.specificInfo = {
          'originPlace': pool.specific_info.originPlace,
          'destinationPlace': pool.specific_info.destinationPlace,
          'trafficTime': pool.specific_info.trafficTime
        }
      } else if (data.modifyRequest.project.requestType === 'activity') {
        data.modifyRequest.project.specificInfo = {
          'location': pool.specific_info.location,
          'activityTime': pool.specific_info.activityTime
        }
      }
      try {
        const modifyResponse = await axios.post(serverURL + '/v1/groupOrder/modify', data, {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + store.state.user.token
          }
        })
        if (modifyResponse.status === 200) {
          this.$message.success('拼单信息已更新')
          setTimeout(() => {
            this.$router.go(0)
          }, 300)
          this.dialogVisibleEdit = false
        }
      } catch (error) {
        this.$message.error('更新拼单信息失败')
      }
    }
  },
  async created () {
    // 初始化时显示我加入的拼单
    try {
      const myParticipateResponse = await axios.get(serverURL + `/v1/groupOrder/getMyParticipate?userId=${store.state.user.userid}`, {
        headers: {
          'Authorization': 'Bearer ' + store.state.user.token
        }
      })
      if (myParticipateResponse.status === 200) {
        this.filteredJoinedId = myParticipateResponse.data.data
      }
    } catch (error) {
      Message({
        type: 'error',
        message: '获取加入拼单失败，请重试'
      })
    }
    for (let i = 0; i < this.filteredJoinedId.length; i++) {
      this.test2[i] = this.filteredJoinedId[i].request_id
    }
    try {
      const groupOrderResponse1 = await axios.post(serverURL + '/v1/groupOrder/getmultipleinformation', this.test2, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + this.$store.state.user.token
        }
      })
      if (groupOrderResponse1.status === 200) {
        this.temp2 = groupOrderResponse1.data.data
        for (let i = 0; i < this.temp2.length; i++) {
          this.filteredJoinedPools[i] = this.temp2[i][0]
          this.prime_user_id[i] = this.temp2[i][0].prime_user_id
        }
      }
    } catch (error) {
      this.$message.error('获取拼单失败')
    }
    for (let i = 0; i < this.prime_user_id.length; i++) {
      try {
        const usernameResponse1 = await axios.get(serverURL + `/v1/user/getinformation?userId=${this.prime_user_id[i]}`, {
          headers: {
            Authorization: 'Bearer ' + this.$store.state.user.token
          }
        })
        if (usernameResponse1.status === 200) {
          this.filteredJoinedPools[i].startUsername = usernameResponse1.data.user_name
        }
      } catch (error) {}
    }
    // 初始化时显示我创建的拼单
    try {
      const myCreateResponse = await axios.get(serverURL + `/v1/groupOrder/getMyCreate?userId=${store.state.user.userid}`, {
        headers: {
          'Authorization': 'Bearer ' + store.state.user.token
        }
      })
      if (myCreateResponse.status === 200) {
        this.filteredCreatedId = myCreateResponse.data.data
      }
    } catch (error) {
      Message({
        type: 'error',
        message: '获取拼单号失败，请重试'
      })
    }
    for (let i = 0; i < this.filteredCreatedId.length; i++) {
      this.test1[i] = this.filteredCreatedId[i].request_id
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
          this.filteredCreatedPools[i] = this.temp1[i][0]
        }
      }
    } catch (error) {
      this.$message.error('获取拼单失败')
    }
    this.filteredPools = this.filteredCreatedPools
    for (let i = 0; i < this.filteredPools.length; i++) {
      if (this.filteredPools[i].request_type === 'product') {
        this.filteredPools[i].request_type = '拼好物'
      } else if (this.filteredPools[i].request_type === 'traffic') {
        this.filteredPools[i].request_type = '拼交通'
      } else if (this.filteredPools[i].request_type === 'activity') {
        this.filteredPools[i].request_type = '拼活动'
      }
    }
    this.$forceUpdate()
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
