<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <div style="display: flex; align-items: center; gap: 12px">
        <span style="font-weight: 500">{{ analysis.year }}年{{ analysis.month }}月经营分析</span>
        <el-button @click="loadData" :icon="Refresh" size="small">刷新</el-button>
      </div>
    </el-card>

    <!-- 概览卡片 -->
    <el-row :gutter="16" style="margin-bottom: 16px">
      <el-col :span="6" v-for="c in cards" :key="c.label">
        <el-card shadow="hover" style="text-align: center">
          <div :style="{ fontSize: '28px', fontWeight: 'bold', color: c.color }">{{ c.value }}</div>
          <div style="font-size: 12px; color: #64748b; margin-top: 4px">{{ c.label }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <!-- 本月流程类型分布 -->
      <el-col :span="12">
        <el-card style="margin-bottom: 16px">
          <template #header><span style="font-weight: 500">本月流程类型分布</span></template>
          <el-table :data="analysis.flowTypeSummaries" stripe empty-text="暂无数据" size="small">
            <el-table-column prop="typeName" label="流程类型" />
            <el-table-column prop="count" label="数量" width="80" align="center" />
            <el-table-column prop="totalAmount" label="涉及金额" min-width="140">
              <template #default="{ row }">{{ row.totalAmount.toLocaleString() }} 元</template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <!-- 经营数据 -->
      <el-col :span="12">
        <el-card style="margin-bottom: 16px">
          <template #header><span style="font-weight: 500">经营数据汇总</span></template>
          <el-descriptions :column="2" border size="small">
            <el-descriptions-item label="在建项目">{{ analysis.activeProjectCount }} / {{ analysis.projectCount }}</el-descriptions-item>
            <el-descriptions-item label="本月流程数">{{ analysis.monthlyFlowCount }}</el-descriptions-item>
            <el-descriptions-item label="累计结算金额">{{ analysis.totalSettledAmount.toLocaleString() }} 元</el-descriptions-item>
            <el-descriptions-item label="待审批数">{{ analysis.pendingApprovalCount }}</el-descriptions-item>
            <el-descriptions-item label="未处理预警">
              <el-tag :type="analysis.openWarningCount > 0 ? 'danger' : 'success'" size="small">{{ analysis.openWarningCount }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="未闭环检查">
              <el-tag :type="analysis.openInspectionCount > 0 ? 'warning' : 'success'" size="small">{{ analysis.openInspectionCount }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="回款（已到账）">{{ analysis.totalPaymentCollected.toLocaleString() }} 元</el-descriptions-item>
            <el-descriptions-item label="回款（待收款）">
              <span style="color: #f59e0b">{{ analysis.totalPaymentPending.toLocaleString() }} 元</span>
            </el-descriptions-item>
            <el-descriptions-item label="本月复盘会议">{{ analysis.reviewMeetingCount }}</el-descriptions-item>
          </el-descriptions>
        </el-card>
      </el-col>
    </el-row>

    <!-- 快速导航 -->
    <el-card>
      <template #header><span style="font-weight: 500">快速导航</span></template>
      <div style="display: flex; gap: 16px; flex-wrap: wrap">
        <el-button @click="goTo('/workflow')">变更索赔中心</el-button>
        <el-button @click="goTo('/settlement')">结算管理中心</el-button>
        <el-button @click="goTo('/inspection')">检查监管中心</el-button>
        <el-button @click="goTo('/payment')">回款管理</el-button>
        <el-button @click="goTo('/warning')">预警中心</el-button>
        <el-button @click="goTo('/plan')">月度计划</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { Refresh } from '@element-plus/icons-vue';
import request from '@/utils/request';

const router = useRouter();
const now = new Date();
const analysis = reactive({
  year: now.getFullYear(), month: now.getMonth() + 1,
  projectCount: 0, activeProjectCount: 0,
  monthlyFlowCount: 0, totalSettledAmount: 0,
  pendingApprovalCount: 0, openWarningCount: 0,
  openInspectionCount: 0, totalPaymentCollected: 0,
  totalPaymentPending: 0, reviewMeetingCount: 0,
  flowTypeSummaries: [] as any[]
});

const cards = [
  { label: '在建项目', value: 0, color: '#3b82f6' },
  { label: '本月流程', value: 0, color: '#22c55e' },
  { label: '未处理预警', value: 0, color: '#ef4444' },
  { label: '未闭环检查', value: 0, color: '#f59e0b' }
];

onMounted(loadData);
async function loadData() {
  try {
    var res = await request.get('/analysis/monthly');
    if (res.code === 200 && res.data) {
      Object.assign(analysis, res.data);
      cards[0].value = res.data.activeProjectCount;
      cards[1].value = res.data.monthlyFlowCount;
      cards[2].value = res.data.openWarningCount;
      cards[3].value = res.data.openInspectionCount;
    }
  } catch {}
}
function goTo(path: string) { router.push(path); }
</script>