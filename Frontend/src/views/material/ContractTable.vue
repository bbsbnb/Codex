<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <div style="display: flex; align-items: center; gap: 12px">
        <el-select v-model="projectId" placeholder="选择项目" style="width: 200px" @change="loadTable">
          <el-option v-for="p in projects" :key="p.id" :label="p.projectName" :value="p.id" />
        </el-select>
        <el-date-picker v-model="period" type="month" value-format="YYYY-MM" placeholder="选择月份" @change="loadTable" />
        <el-button type="primary" @click="generateTable" :loading="genLoading">生成/刷新</el-button>
      </div>
    </el-card>

    <el-row :gutter="16">
      <!-- 预计总收入调整表 -->
      <el-col :span="8">
        <el-card>
          <template #header><span style="font-weight: 500">预计总收入调整表</span></template>
          <el-descriptions v-if="table" :column="1" border size="small">
            <el-descriptions-item label="原始合同收入">{{ formatMoney(table.originalTotalIncome) }}</el-descriptions-item>
            <el-descriptions-item label="签证调增">{{ formatMoney(table.visaAdjustment) }}</el-descriptions-item>
            <el-descriptions-item label="索赔调增">{{ formatMoney(table.claimAdjustment) }}</el-descriptions-item>
            <el-descriptions-item label="设计变更调增">{{ formatMoney(table.designChangeAdjustment) }}</el-descriptions-item>
            <el-descriptions-item label="调整后总收入">
              <span style="color: #22c55e; font-weight: bold">{{ formatMoney(table.adjustedTotalIncome) }}</span>
            </el-descriptions-item>
          </el-descriptions>
          <div v-else style="text-align: center; color: #999; padding: 20px">选择项目并生成数据</div>
        </el-card>
      </el-col>

      <!-- 预计总成本调整表 -->
      <el-col :span="8">
        <el-card>
          <template #header><span style="font-weight: 500">预计总成本调整表</span></template>
          <el-descriptions v-if="table" :column="1" border size="small">
            <el-descriptions-item label="原始预算成本">{{ formatMoney(table.originalTotalCost) }}</el-descriptions-item>
            <el-descriptions-item label="材料结算成本">{{ formatMoney(table.materialCostActual) }}</el-descriptions-item>
            <el-descriptions-item label="调整后总成本">
              <span :style="{ color: table.adjustedTotalCost > table.originalTotalCost ? '#ef4444' : '#22c55e', fontWeight: 'bold' }">
                {{ formatMoney(table.adjustedTotalCost) }}
              </span>
            </el-descriptions-item>
          </el-descriptions>
          <div v-else style="text-align: center; color: #999; padding: 20px">选择项目并生成数据</div>
        </el-card>
      </el-col>

      <!-- 利润动态调整表 -->
      <el-col :span="8">
        <el-card>
          <template #header><span style="font-weight: 500">利润动态调整表</span></template>
          <el-descriptions v-if="table" :column="1" border size="small">
            <el-descriptions-item label="原始利润">{{ formatMoney(table.originalProfit) }}</el-descriptions-item>
            <el-descriptions-item label="当前利润">
              <span :style="{ color: table.currentProfit >= 0 ? '#22c55e' : '#ef4444', fontWeight: 'bold' }">
                {{ formatMoney(table.currentProfit) }}
              </span>
            </el-descriptions-item>
            <el-descriptions-item label="利润变化率">
              <el-tag :type="table.profitChangeRate < 0 ? 'danger' : 'success'">
                {{ table.profitChangeRate >= 0 ? '+' : '' }}{{ table.profitChangeRate }}%
              </el-tag>
            </el-descriptions-item>
          </el-descriptions>
          <div v-else style="text-align: center; color: #999; padding: 20px">选择项目并生成数据</div>
        </el-card>
      </el-col>
    </el-row>

    <el-card style="margin-top: 16px" v-if="table">
      <template #header><span style="font-weight: 500">数据来源汇总</span></template>
      <div style="color: #64748b; font-size: 13px">
        <p>· 签证调增金额 = 所有已归档签证单的调增金额汇总</p>
        <p>· 索赔调增金额 = 所有已归档索赔单的调增金额汇总</p>
        <p>· 设计变更调增金额 = 所有已归档设计变更单的调增金额汇总</p>
        <p>· 材料结算成本 = 材料月结算单的总金额汇总</p>
        <p>· 数据更新时间：{{ table.updatedAt }}</p>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const projects = ref<any[]>([]);
const projectId = ref(0);
const period = ref('');
const table = ref<any>(null);
const genLoading = ref(false);

onMounted(async () => {
  try {
    var res = await request.get('/project');
    if (res.code === 200) {
      projects.value = res.data || [];
      if (projects.value.length) { projectId.value = projects.value[0].id; loadTable(); }
    }
  } catch {}
});

function formatMoney(v: number) {
  return (v || 0).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' 元';
}

async function loadTable() {
  if (!projectId.value || !period.value) return;
  var [year, month] = period.value.split('-');
  try {
    var res = await request.get('/settlement/contract-table', { params: { projectId: projectId.value, year: parseInt(year), month: parseInt(month) } });
    if (res.code === 200) table.value = res.data;
  } catch { table.value = null; }
}

async function generateTable() {
  if (!projectId.value || !period.value) { ElMessage.warning('请选择项目和月份'); return; }
  genLoading.value = true;
  var [year, month] = period.value.split('-');
  try {
    var res = await request.post('/settlement/contract-table/generate', null, { params: { projectId: projectId.value, year: parseInt(year), month: parseInt(month) } });
    if (res.code === 200) { ElMessage.success('生成成功'); table.value = res.data; }
  } finally { genLoading.value = false; }
}
</script>