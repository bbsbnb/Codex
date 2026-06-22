<template>
  <el-container style="height: 100vh">
    <el-aside width="240px" style="background: #1e293b; overflow-y: auto">
      <div class="logo-area"><h2 style="color: #fff; text-align: center; padding: 20px 0; margin: 0; font-size: 18px">天行建筑管理平台</h2></div>
      <el-menu :default-active="route.path" background-color="#1e293b" text-color="#cbd5e1" active-text-color="#3b82f6" router>
        <el-menu-item index="/dashboard"><el-icon><Odometer /></el-icon><span>工作台</span></el-menu-item>
        <el-menu-item index="/project"><el-icon><Folder /></el-icon><span>项目管理</span></el-menu-item>
        <el-menu-item index="/document"><el-icon><Files /></el-icon><span>一次经营资料库</span></el-menu-item>
        <el-menu-item index="/plan"><el-icon><Calendar /></el-icon><span>月度计划</span></el-menu-item>
        <el-sub-menu index="/workflow"><template #title><el-icon><Edit /></el-icon><span>变更索赔中心</span></template>
          <el-menu-item index="/workflow">流程管理</el-menu-item>
          <el-menu-item index="/workflow-statistics">业务台账</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/settlement"><template #title><el-icon><Document /></el-icon><span>结算管理中心</span></template>
          <el-menu-item index="/settlement">结算管理</el-menu-item>
          <el-menu-item index="/contract-table">建造合同三张表</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/inspection"><template #title><el-icon><Monitor /></el-icon><span>检查监管中心</span></template>
          <el-menu-item index="/inspection">检查记录</el-menu-item>
          <el-menu-item index="/payment">回款管理</el-menu-item>
        </el-sub-menu>
        <el-menu-item index="/report"><el-icon><DataAnalysis /></el-icon><span>经营分析</span></el-menu-item>
        <el-menu-item index="/review-meeting"><el-icon><ChatLineSquare /></el-icon><span>复盘会议</span></el-menu-item>
        <el-menu-item index="/notification"><el-icon><Bell /></el-icon><span>消息通知</span></el-menu-item>
        <el-menu-item index="/warning"><el-icon><Warning /></el-icon><span>预警中心</span></el-menu-item>
      </el-menu>
    </el-aside>
    <el-container>
      <el-header style="background: #fff; border-bottom: 1px solid #e5e7eb; display: flex; align-items: center; justify-content: space-between; padding: 0 20px">
        <span style="font-size: 16px; font-weight: 500">{{ route.meta.title }}</span>
        <div style="display: flex; align-items: center; gap: 16px">
          <el-badge :value="unreadCount" :hidden="unreadCount === 0" style="cursor: pointer" @click="router.push('/notification')">
            <el-icon :size="20"><Bell /></el-icon>
          </el-badge>
          <span>{{ userStore.user?.realName || '未登录' }}</span>
          <el-button type="danger" size="small" @click="handleLogout">退出</el-button>
        </div>
      </el-header>
      <el-main style="background: #f1f5f9; padding: 20px; overflow-y: auto"><router-view /></el-main>
    </el-container>
  </el-container>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/store/modules/auth';
import request from '@/utils/request';
const route = useRoute(); const router = useRouter(); const userStore = useAuthStore();
const unreadCount = ref(0);
userStore.loadFromStorage();
onMounted(async () => {
  try { var res = await request.get('/notification/unread/0'); if (res.code === 200) unreadCount.value = res.data || 0; } catch {}
});
function handleLogout() { userStore.logout(); router.push('/login'); }
</script>