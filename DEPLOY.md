# 天行建筑智能管理平台 — 部署指南

## 前提条件

- Docker & Docker Compose (推荐)
- 或：.NET 8 SDK + SQL Server 2019+ + Node.js 18+ (手动部署)

---

## 一、Docker 部署（推荐）

### 1. 快速启动

```bash
# 克隆项目后，在项目根目录执行
docker-compose up -d

# 查看日志
docker-compose logs -f
```

### 2. 服务地址

| 服务 | 地址 | 说明 |
|------|------|------|
| 前端 | http://localhost | 浏览器访问 |
| 后端API | http://localhost:5000 | Swagger: http://localhost:5000/swagger |
| 数据库 | localhost:1433 | SQL Server Express |

### 3. 首次使用

数据库会自动初始化，首次访问需注册管理员账户。

---

## 二、手动部署

### 后端部署

```bash
# 1. 安装 .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0

# 2. 修改连接字符串
# Backend/src/TXConstructionManagement/appsettings.json
# → ConnectionStrings.DefaultConnection

# 3. 还原依赖并构建
cd Backend
dotnet restore
dotnet build -c Release

# 4. 数据库迁移
cd src/TXConstructionManagement
dotnet ef migrations add InitialCreate
dotnet ef database update

# 5. 发布并运行
dotnet publish -c Release -o publish
cd publish
dotnet TXConstructionManagement.dll
```

### 前端部署

```bash
# 1. 安装 Node.js 18+
# https://nodejs.org/

# 2. 安装依赖
cd Frontend
npm install

# 3. 修改API地址（如果需要）
# .env.production 中的 VITE_API_BASE_URL

# 4. 构建
npm run build
# 输出在 Frontend/dist/ 目录

# 5. Nginx 配置
# 复制 Frontend/nginx.conf 到 /etc/nginx/conf.d/
# 将 dist/ 目录复制到 /usr/share/nginx/html/
```

---

## 三、IIS 部署（Windows Server）

### 后端

1. 安装 .NET 8 Hosting Bundle
2. 发布后端到本地目录
3. 在 IIS 中创建应用程序池（无托管代码）
4. 创建网站指向发布目录
5. 安装 URL Rewrite 模块（用于前端路由）

### 前端

1. 构建前端：`npm run build`
2. 将 dist/ 目录部署到 IIS 网站根目录
3. 配置 URL Rewrite 规则（SPA路由）：

```xml
<rule name="SPA Routes" stopProcessing="true">
  <match url=".*" />
  <conditions logicalGrouping="MatchAll">
    <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
    <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
    <add input="{REQUEST_URI}" pattern="^/(api|swagger)" negate="true" />
  </conditions>
  <action type="Rewrite" url="/" />
</rule>
```

---

## 四、配置说明

### 数据库连接

编辑 `Backend/src/TXConstructionManagement/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TXConstructionDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "FileStorage": {
    "BaseUrl": "C:/TXConstruction/Documents",
    "MaxFileSizeMB": 100
  },
  "WarningThresholds": {
    "OverBudgetPercent": 5,
    "ClaimDeadlineDays": 28,
    "OverBudgetNoticeDays": 3
  }
}
```

### 文件存储

一期使用本地磁盘存储，切换云存储只需修改 `DocumentService.cs` 中的存储路径。

### 超概预警阈值

默认 5%，可在 `appsettings.json` → `WarningThresholds.OverBudgetPercent` 中修改。

---

## 五、常见问题

### Q: 数据库连接失败？
A: 检查 SQL Server 是否开启 TCP/IP 协议，确认连接字符串中的服务器地址和端口。

### Q: 前端访问 API 404？
A: 检查 Nginx 反向代理配置是否正确，确保 `/api/` 已代理到后端地址。

### Q: 文件上传超过 100MB？
A: 同步修改 `appsettings.json` 的 MaxFileSizeMB 和 Nginx 的 client_max_body_size。

### Q: Swagger 无法访问？
A: 生产环境默认关闭 Swagger，可通过设置 `ASPNETCORE_ENVIRONMENT=Development` 开启。

---

## 六、运维命令

```bash
# Docker
docker-compose ps                     # 查看服务状态
docker-compose logs -f backend        # 查看后端日志
docker-compose restart backend        # 重启后端
docker-compose down                   # 停止所有服务

# 备份数据库
docker exec tx-construction-db /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Password \
  -Q "BACKUP DATABASE TXConstructionDB TO DISK='/var/opt/mssql/backup.bak'"

# 查看实时日志
docker logs -f tx-construction-backend
```
