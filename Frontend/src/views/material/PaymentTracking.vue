<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <el-button type="primary" @click="showCreate = true">+ 新建回款计划</el-button>
    </el-card>

    <el-row :gutter="16" style="margin-bottom: 16px">
      <el-col :span="6" v-for="s in summaryCards" :key="s.label">
        <el-card style="text-align: center">
          <div style="font-size: 24px; font-weight: bold; color: #3b82f6">{{ s.count }}</div>
          <div style="font-size: 12px; color: #64748b">{{ s.label }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-card>
      <el-table :data="payments" stripe empty-text="暂无回款记录">
        <el-table-column prop="paymentNo" label="编号" width="160" />
        <el-table-column prop="amount" label="金额" width="140">
          <template #default="{ row }">{{ row.amount.toLocaleString() }} 元</template>
        </el-table-column>
        <el-table-column prop="paymentDate" label="计划回款日" width="120" />
        <el-table-column prop="remark" label="备注" min-width="160" />
        <el-table-column prop="responsiblePerson" label="责任人" width="100" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="['info','success','warning','danger'][row.status]" size="small">
              {{ ['待收款','已到账','部分到账','已逾期'][row.status] }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="140" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="updateStatus(row.id, 1)" v-if="row.status === 0">标记到账</el-button>
            <el-button type="warning" link @click="updateStatus(row.id, 2)" v-if="row.status === 0">部分到账</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="showCreate" title="新建回款计划" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="回款金额"><el-input-number v-model="form.amount" :min="0" :precision="2" style="width: 100%" /></el-form-item>
        <el-form-item label="计划回款日"><el-date-picker v-model="form.paymentDate" type="date" style="width: 100%" /></el-form-item>
        <el-form-item label="责任人"><el-input v-model="form.remark" placeholder="输入责任人姓名" /></el-form-item>
        <el-form-item label="备注"><el-input v-model="form.remark" type="textarea" :rows="2" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const payments = ref<any[]>([]);
const showCreate = ref(false);
const loading = ref(false);
const form = reactive({ projectId: 1, amount: 0, paymentDate: '', responsibleUserId: 0, remark: '' });

const summaryCards = computed(() => [
  { label: '总笔数', count: payments.value.length },
  { label: '已到账', count: payments.value.filter(function(p: any) { return p.status === 1; }).length },
  { label: '待收款', count: payments.value.filter(function(p: any) { return p.status === 0; }).length },
  { label: '已逾期', count: payments.value.filter(function(p: any) { return p.status === 3; }).length }
]);

onMounted(loadData);
async function loadData() {
  try { var res = await request.get('/payment/project/1'); if (res.code === 200) payments.value = res.data || []; } catch {}
}

async function handleCreate() {
  if (form.amount <= 0) { ElMessage.warning('请输入金额'); return; }
  loading.value = true;
  try {
    var res = await request.post('/payment', form);
    if (res.code === 200) { ElMessage.success('创建成功'); showCreate.value = false; loadData(); }
  } finally { loading.value = false; }
}

async function updateStatus(id: number, status: number) {
  try {
    var res = await request.put('/payment/' + id, { status: status });
    if (res.code === 200) { ElMessage.success('已更新'); loadData(); }
  } catch {}
}
</script>