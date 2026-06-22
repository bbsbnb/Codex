<template>
  <div>
    <!-- 结算类型选择 -->
    <el-card style="margin-bottom: 16px">
      <el-radio-group v-model="activeType" @change="loadData">
        <el-radio-button :value="-1">全部</el-radio-button>
        <el-radio-button :value="0">月验工计价</el-radio-button>
        <el-radio-button :value="1">材料月结算</el-radio-button>
        <el-radio-button :value="2">月消耗量核定</el-radio-button>
      </el-radio-group>
      <el-button type="primary" style="float: right" @click="showCreate = true">+ 新建结算单</el-button>
    </el-card>

    <!-- 列表 -->
    <el-card>
      <el-table :data="filteredData" stripe empty-text="暂无结算数据">
        <el-table-column prop="flowNo" label="编号" width="160" />
        <el-table-column prop="title" label="标题" min-width="180" />
        <el-table-column label="类型" width="120">
          <template #default="{ row }">{{ ['月验工计价','材料月结算','月消耗量核定'][row.settlementType] || row.settlementType }}</template>
        </el-table-column>
        <el-table-column label="金额" width="140">
          <template #default="{ row }">{{ row.amount?.toLocaleString() || '-' }} 元</template>
        </el-table-column>
        <el-table-column label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="['info','warning','success','danger','info'][row.status]" size="small">
              {{ ['草稿','待审批','已通过','已驳回','已归档'][row.status] }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="initiatedAt" label="发起时间" width="160" />
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="showDetail(row)">查看</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 创建结算单 -->
    <el-dialog v-model="showCreate" :title="'新建' + typeNames[activeType === -1 ? 0 : activeType]" width="560px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="结算类型"><el-select v-model="form.settlementType" style="width: 100%">
          <el-option label="月验工计价" :value="0" />
          <el-option label="材料月结算" :value="1" />
          <el-option label="月消耗量核定" :value="2" />
        </el-select></el-form-item>
        <el-form-item label="标题"><el-input v-model="form.title" /></el-form-item>

        <template v-if="form.settlementType === 0">
          <el-form-item label="验工月份"><el-date-picker v-model="form.settlementMonth" type="month" value-format="YYYY-MM" style="width: 100%" /></el-form-item>
          <el-form-item label="完成工程量"><el-input-number v-model="form.completedQuantity" :min="0" style="width: 100%" /></el-form-item>
          <el-form-item label="单位"><el-input v-model="form.quantityUnit" placeholder="m3/t/个" style="width: 100%" /></el-form-item>
          <el-form-item label="报审金额"><el-input-number v-model="form.submittedAmount" :min="0" :precision="2" style="width: 100%" /></el-form-item>
        </template>

        <template v-if="form.settlementType === 1">
          <el-form-item label="材料名称"><el-input v-model="form.materialName" /></el-form-item>
          <el-form-item label="规格型号"><el-input v-model="form.materialSpec" /></el-form-item>
          <el-form-item label="结算数量"><el-input-number v-model="form.settlementQuantity" :min="0" style="width: 100%" /></el-form-item>
          <el-form-item label="单价"><el-input-number v-model="form.unitPrice" :min="0" :precision="2" style="width: 100%" /></el-form-item>
          <el-form-item label="分包单位"><el-input v-model="form.subcontractor" /></el-form-item>
        </template>

        <template v-if="form.settlementType === 2">
          <el-form-item label="计划消耗量"><el-input-number v-model="form.plannedQuantity" :min="0" style="width: 100%" /></el-form-item>
          <el-form-item label="实际消耗量"><el-input-number v-model="form.actualQuantity" :min="0" style="width: 100%" /></el-form-item>
          <el-form-item label="偏差原因"><el-input v-model="form.deviationReason" type="textarea" :rows="2" /></el-form-item>
        </template>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">提交</el-button>
      </template>
    </el-dialog>

    <!-- 结算单详情弹窗 -->
    <el-drawer v-model="showDetailPanel" :title="detail?.flowNo" size="500px">
      <template v-if="detail">
        <el-descriptions :column="2" border size="small">
          <el-descriptions-item label="编号">{{ detail.flowNo }}</el-descriptions-item>
          <el-descriptions-item label="类型">{{ detail.settlementTypeName }}</el-descriptions-item>
          <el-descriptions-item label="状态"><el-tag :type="['info','warning','success','danger','info'][detail.status]" size="small">{{ detail.statusName }}</el-tag></el-descriptions-item>
          <el-descriptions-item label="金额">{{ detail.amount?.toLocaleString() + ' 元' || '-' }}</el-descriptions-item>
          <el-descriptions-item label="发起时间">{{ detail.initiatedAt }}</el-descriptions-item>
        </el-descriptions>

        <div style="margin-top: 20px; font-weight: 500; margin-bottom: 12px">详细数据</div>
        <div v-if="detail.settlementType === 0">
          <el-descriptions :column="2" border size="small">
            <el-descriptions-item label="验工月份">{{ detail.settlementMonth || '-' }}</el-descriptions-item>
            <el-descriptions-item label="完成量">{{ detail.completedQuantity ?? '-' }} {{ detail.quantityUnit || '' }}</el-descriptions-item>
          </el-descriptions>
        </div>
        <div v-if="detail.settlementType === 1">
          <el-descriptions :column="2" border size="small">
            <el-descriptions-item label="材料名称">{{ detail.materialName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="规格">{{ detail.materialSpec || '-' }}</el-descriptions-item>
            <el-descriptions-item label="分包单位">{{ detail.subcontractor || '-' }}</el-descriptions-item>
          </el-descriptions>
        </div>
        <div v-if="detail.settlementType === 2">
          <el-descriptions :column="2" border size="small">
            <el-descriptions-item label="计划消耗">{{ detail.plannedQuantity ?? '-' }}</el-descriptions-item>
            <el-descriptions-item label="实际消耗">{{ detail.actualQuantity ?? '-' }}</el-descriptions-item>
            <el-descriptions-item label="偏差率">
              <el-tag :type="detail.deviationRate > 5 ? 'danger' : 'success'" v-if="detail.deviationRate != null">
                {{ detail.deviationRate }}%
              </el-tag>
            </el-descriptions-item>
          </el-descriptions>
        </div>
      </template>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const settlements = ref<any[]>([]);
const activeType = ref(-1);
const showCreate = ref(false);
const showDetailPanel = ref(false);
const loading = ref(false);
const detail = ref<any>(null);

const typeNames = ['月验工计价', '材料月结算', '月消耗量核定'];

const form = reactive({
  projectId: 1, settlementType: 0, title: '',
  settlementMonth: '', completedQuantity: 0, quantityUnit: '', submittedAmount: 0,
  materialName: '', materialSpec: '', settlementQuantity: 0, unitPrice: 0, subcontractor: '',
  plannedQuantity: 0, actualQuantity: 0, deviationReason: ''
});

const filteredData = computed(() => {
  if (activeType.value === -1) return settlements.value;
  return settlements.value.filter(function(s: any) { return s.settlementType === activeType.value; });
});

onMounted(loadData);

async function loadData() {
  try {
    var res = await request.get('/settlement/project/1');
    if (res.code === 200) settlements.value = res.data || [];
  } catch {}
}

async function handleCreate() {
  loading.value = true;
  try {
    var res = await request.post('/settlement/create', form);
    if (res.code === 200) { ElMessage.success('创建成功'); showCreate.value = false; loadData(); }
    else ElMessage.error(res.message || '创建失败');
  } finally { loading.value = false; }
}

function showDetail(row: any) { detail.value = row; showDetailPanel.value = true; }
</script>