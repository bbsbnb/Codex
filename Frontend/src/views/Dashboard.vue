<template>
  <div>
    <el-row :gutter="20">
      <el-col :span="6" v-for="stat in stats" :key="stat.label">
        <el-card shadow="hover" style="margin-bottom: 20px">
          <div style="text-align: center">
            <div :style="{ fontSize: '32px', color: stat.color, fontWeight: 'bold' }">{{ stat.value }}</div>
            <div style="color: #64748b; margin-top: 8px; font-size: 14px">{{ stat.label }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card style="margin-bottom: 20px">
      <template #header><span style="font-weight: 500">待办事项</span></template>
      <el-table :data="pendingFlows" style="width: 100%" empty-text="暂无待办事项">
        <el-table-column prop="flowNo" label="编号" width="180" />
        <el-table-column label="类型">
          <template #default="{ row }">{{ flowTypeLabels[row.flowType] || row.flowType }}</template>
        </el-table-column>
        <el-table-column prop="title" label="标题" />
        <el-table-column prop="initiatedAt" label="发起时间" width="180" />
      </el-table>
    </el-card>

    <el-card>
      <template #header><span style="font-weight: 500">在建项目</span></template>
      <el-table :data="activeProjects" style="width: 100%" empty-text="暂无在建项目">
        <el-table-column prop="projectCode" label="编号" width="120" />
        <el-table-column prop="projectName" label="项目名称" />
        <el-table-column prop="contractAmount" label="合同金额" width="150">
          <template #default="{ row }">{{ row.contractAmount.toLocaleString() }} 元</template>
        </el-table-column>
        <el-table-column prop="memountCount" label="项目成员" width="100" />
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button type="primary" link @click="goProject(row.id)">查看</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { getAllProjects } from '@/api/project';
import request from '@/utils/request';

const router = useRouter();
const stats = ref([
  { label: '在建项目', value: 0, color: '#3b82f6' },
  { label: '本月计划', value: 0, color: '#22c55e' },
  { label: '待审批流程', value: 0, color: '#f59e0b' },
  { label: '未处理预警', value: 0, color: '#ef4444' }
]);
const activeProjects = ref<any[]>([]);
const pendingFlows = ref<any[]>([]);
const flowTypeLabels: Record<number, string> = { 0: '工联单', 1: '认质认价', 2: '签证', 3: '索赔', 4: '设计变更', 5: '月验工计价' };

onMounted(async () => {
  try {
    const res = await getAllProjects();
    if (res.code === 200) {
      activeProjects.value = res.data || [];
      stats.value[0].value = (res.data || []).filter((p: any) => p.status === 0).length;
    }
  } catch {}
});
function goProject(id: number) { router.push('/project/' + id); }
</script>