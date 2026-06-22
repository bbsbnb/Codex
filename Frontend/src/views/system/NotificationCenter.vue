<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <span style="font-weight: 500; margin-right: 16px">消息通知</span>
      <el-button size="small" @click="markAllRead" v-if="unreadCount > 0">全部标记已读</el-button>
      <el-button size="small" @click="loadData" :icon="Refresh">刷新</el-button>
      <el-tag v-if="unreadCount > 0" type="danger" style="margin-left: 12px">{{ unreadCount }} 条未读</el-tag>
    </el-card>

    <el-card>
      <el-table :data="notifications" stripe empty-text="暂无通知">
        <el-table-column label="级别" width="80">
          <template #default="{ row }">
            <el-tag :type="row.levelName === '紧急' ? 'danger' : row.levelName === '重要' ? 'warning' : 'info'" size="small">
              {{ { '普通': 'info', '重要': 'warning', '紧急': 'danger' }[row.levelName] || 'info' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="类型" width="80">
          <template #default="{ row }">{{ row.typeName }}</template>
        </el-table-column>
        <el-table-column prop="title" label="标题" min-width="200" />
        <el-table-column prop="content" label="内容" min-width="250" show-overflow-tooltip />
        <el-table-column prop="createdAt" label="时间" width="160" />
        <el-table-column label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.isRead ? 'info' : 'danger'" size="small">{{ row.isRead ? '已读' : '未读' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="80" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="markRead(row.id)" v-if="!row.isRead">标记已读</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Refresh } from '@element-plus/icons-vue';
import request from '@/utils/request';

const notifications = ref<any[]>([]);
const unreadCount = ref(0);

onMounted(loadData);
async function loadData() {
  try {
    var [notifRes, unreadRes] = await Promise.all([
      request.get('/notification/user/0'),
      request.get('/notification/unread/0')
    ]);
    if (notifRes.code === 200) notifications.value = notifRes.data || [];
    if (unreadRes.code === 200) unreadCount.value = unreadRes.data || 0;
  } catch {}
}
async function markRead(id: number) {
  try { var res = await request.post('/notification/' + id + '/read'); if (res.code === 200) loadData(); } catch {}
}
async function markAllRead() {
  try { var res = await request.post('/notification/read-all/0'); if (res.code === 200) { ElMessage.success('已全部标记已读'); loadData(); } } catch {}
}
</script>