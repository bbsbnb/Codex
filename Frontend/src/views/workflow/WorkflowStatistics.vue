<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <template #header><span style="font-weight: 500">变更索赔台账</span></template>
      <el-table :data="flowData" stripe empty-text="暂无数据">
        <el-table-column label="流程类型" width="120">
          <template #default="{ row }">{{ flowTypeName(row.flowType) }}</template>
        </el-table-column>
        <el-table-column label="总数" width="80" align="center" prop="total" />
        <el-table-column label="草稿" width="80" align="center" prop="draft" />
        <el-table-column label="待审批" width="80" align="center" prop="pending" />
        <el-table-column label="已通过" width="80" align="center" prop="approved" />
        <el-table-column label="已驳回" width="80" align="center" prop="rejected" />
        <el-table-column label="已归档" width="80" align="center" prop="archived" />
        <el-table-column label="涉及总金额" min-width="150">
          <template #default="{ row }">{{ row.totalAmount.toLocaleString() }} 元</template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-card>
      <template #header><span style="font-weight: 500">按项目汇总</span></template>
      <el-table :data="projectSummary" stripe empty-text="暂无项目">
        <el-table-column prop="projectName" label="项目名称" />
        <el-table-column label="工联单" width="80" align="center" />
        <el-table-column label="认质认价" width="80" align="center" />
        <el-table-column label="签证" width="60" align="center" />
        <el-table-column label="索赔" width="60" align="center" />
        <el-table-column label="设计变更" width="80" align="center" />
        <el-table-column label="合计" width="80" align="center" />
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import request from '@/utils/request';

const flowData = ref<any[]>([]);
const projectSummary = ref<any[]>([]);

function flowTypeName(v: number) {
  return ['工联单','认质认价','签证','索赔','设计变更'][v] || v;
}

onMounted(async () => {
  try {
    var res = await request.get('/workflow-statistics/project/1');
    if (res.code === 200) { flowData.value = res.data.groups || []; }
  } catch {}

  try {
    var pj = await request.get('/project');
    if (pj.code === 200) projectSummary.value = (pj.data || []).map(function(p: any) {
      return { projectName: p.projectName };
    });
  } catch {}
});
</script>