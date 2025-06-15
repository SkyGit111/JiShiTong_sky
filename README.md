# CourseProject_SAD_SE

合作软件开发课程相关项目

CI状态： 

[![Node.js Frontend Test](https://github.com/Wangtk311/Jishitong-CourseProject/actions/workflows/node.js.yml/badge.svg)](https://github.com/Wangtk311/Jishitong-CourseProject/actions/workflows/node.js.yml)
[![.NET Backend Test](https://github.com/Wangtk311/Jishitong-CourseProject/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Wangtk311/Jishitong-CourseProject/actions/workflows/dotnet-desktop.yml)

--------

`main` 分支：存储项目最终整体，其内容是从 `main-frontend` `main-backend` `main-docs` 分支组合而来，受到保护规则的保护。

可修改内容：不允许修改，只能从其他分支合并。

`main-frontend` 分支：存储项目前端相关文件，不得修改其中的其他部分，不得与 `main` 分支的项目文档和后端部分内容相重叠或冲突，受到保护规则的保护。

可修改内容：frontend-program, frontend-docs, project-design-docs。

`main-backend` 分支：存储项目后端相关文件，不得修改其中的其他部分，不得与 `main` 分支的项目文档和前端部分内容相重叠或冲突，受到保护规则的保护。

可修改内容：backend-program, backend-docs, project-design-docs。

--------

# 分支命名规则

请以如下格式创建个人或小组开发分支： 

 `(名字)/(前端/后端/文档/小组功能模块)/(功能模块)` 

开头字母为大写，功能模块中的单词间使用连字符。


**举例**：

吴彦祖同学要开发数据库连接的后端功能，则他的分支应该命名为： `Wuyanzu/backend/database-connection` ，基分支为 `main-backend` 。

彭于晏同学和泰森同学要开发前端登录页面，彭于晏同学开发验证码功能，泰森同学开发元素布置功能，则他们两个的小组分支应该命名为： `PYY&TS/frontend/login` ，基分支为 `main-frontend` ，两个人的开发分支应该命名为： `Pengyuyan/frontend/captcha` 和 `Taisen/frontend/element-design` ，基分支为 `PYY&TS/frontend/login` 。这种方式对于小组开发时十分有用，因为两人之间同步代码可以在一个没有保护规则的分支上进行，自由度很高，然而这会导致分支关系较为复杂，考虑到人数较少，可以适当避免这种分支组织模式。

每人创建的开发分支，其基分支为或 `main-frontend` 或 `main-backend` 或小组分支，而**不应该**是 `main` 分支， `main` 分支的下游分支只有 `main-frontend` `main-backend`分支，这三个分支的下游分支才是小组或个人的开发分支。

# CI/CD暂时还没有配置完成，请先不用考虑

# 提交Pull Request时请先与main-frontend或main-backend同步，再提交PR
