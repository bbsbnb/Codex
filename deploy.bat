@echo off
REM 天行建筑智能管理平台 - Docker部署脚本
echo ========================================
echo 天行建筑智能管理平台 - Docker部署
echo ========================================

REM 检查Docker
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [错误] 未安装Docker，请先安装Docker Desktop
    pause
    exit /b 1
)

echo [1/3] 启动所有服务...
docker-compose up -d

echo [2/3] 等待数据库初始化...
timeout /t 15

echo [3/3] 验证服务状态...
docker-compose ps

echo ========================================
echo 部署完成！
echo 前端: http://localhost
echo 后端: http://localhost:5000
echo Swagger: http://localhost:5000/swagger
echo ========================================
pause
