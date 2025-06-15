const express = require('express')
const cors = require('cors')
const axios = require('axios')

const app = express()
const port = 3000

// 硬编码API密钥（生产环境应改用环境变量）
const API_KEY = 'sk-0fe92bf297734396b9b2e66f4dd42169'

// 启用CORS和JSON解析
app.use(cors())
app.use(express.json())

// 通义千问API代理端点
app.post('/api/tongyi', async (req, res) => {
    try {
        console.log('收到代理请求')
        console.log('请求prompt摘要:', req.body.prompt.substring(0, 100) + '...')
        console.log('使用API密钥前5位:', API_KEY.substring(0, 5) + '...')

        // 向模型明确指示返回纯JSON，减少Markdown格式
        const messages = [
            {
                role: "system",
                content: "请直接返回纯JSON格式数据，不要使用Markdown代码块，不要包含额外解释文字"
            },
            {
                role: "user",
                content: req.body.prompt
            }
        ]

        const response = await axios({
            method: 'post',
            url: 'https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${API_KEY}`
            },
            data: {
                model: 'qwen-turbo',
                messages: messages,
                temperature: 0.3,
                response_format: { type: 'json_object' } // 明确要求JSON格式
            }
        })

        console.log('API返回状态:', response.status)
        console.log('响应摘要:',
            response.data.choices && response.data.choices[0].message ?
                response.data.choices[0].message.content.substring(0, 100) + '...' :
                '无内容')

        res.json(response.data)
    } catch (error) {
        console.error('代理错误详情:')
        if (error.response) {
            console.error('状态码:', error.response.status)
            console.error('响应头:', JSON.stringify(error.response.headers))
            console.error('响应体:', JSON.stringify(error.response.data))
        } else {
            console.error('请求错误:', error.message)
        }

        res.status(error.response?.status || 500).json({
            error: '通义千问API调用失败',
            details: error.response?.data || error.message
        })
    }
})

// 测试端点
app.get('/', (req, res) => {
    res.send('代理服务器运行正常')
})

app.listen(port, () => {
    console.log(`代理服务器运行在 http://localhost:${port}`)
})