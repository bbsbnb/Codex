<template>
  <div>
    <el-card style="margin-bottom: 20px">
      <el-button type="primary" @click="checkWarnings">触发预警检查</el-button>
    </el-card>
    <el-card>
      <el-table :data="warnings" stripe empty-text="暂无预警">
        <el-table-column label="预警类型" width="150">
          <template #default="{ row }">{{ ['索赔时效','超概预警','超合同预警','材料偏差','计划逾期','资料缺失'][row.type] || row.type }}</template>
        </el-table-column>
        <el-table-column label="级别" width="100">
          <template #default="{ row }">
            <el-tag :type="['info','warning','danger'][row.level]">{{ ['提示','警告','严重'][row.level] }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="message" label="预警内容" min-width="300" />
        <el-table-column prop="triggeredAt" label="触发时间" width="180" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }"><el-tag :type="row.isResolved ? 'success' : 'danger'">{{ row.isResolved ? '已处理' : '未处理' }}</el-tag></template>
        </el-table-column>
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleResolve(row.id)" v-if="!row.isResolved">处理</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const warnings = ref<any[]>([]);

onMounted(loadWarnings);
async function loadWarnings() {
  try { const res = await request.get('/warning/project/1'); if (res.code === 200) warnings.value = res.data || []; } catch {}
}
async function checkWarnings() {
  try { const res = await request.get('/warning/check'); if (res.code === 200) { ElMessage.success('检查完成'); loadWarnings(); } } catch {}
}
async function handleResolve(id: number) {
  try { const res = await request.post('/warning/' + id + '/resolve'); if (res.code === 200) { ElMessage.success('已处理'); loadWarnings(); } } catch {}
}
</script>