<template>
  <div>
    <el-card style="margin-bottom: 20px">
      <el-button type="primary" @click="showDialog = true">+ 新建计划</el-button>
    </el-card>
    <el-card>
      <el-table :data="plans" stripe empty-text="暂无计划">
        <el-table-column prop="planName" label="计划名称" min-width="200" />
        <el-table-column label="计划类型" width="120">
          <template #default="{ row }">{{ ['施工进度','材料采购','资金计划','二次经营'][row.planType] || row.planType }}</template>
        </el-table-column>
        <el-table-column prop="responsiblePerson" label="负责人" width="120" />
        <el-table-column prop="dueDate" label="截止日期" width="120" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="['info','warning','success','danger'][row.status]">
              {{ ['待处理','进行中','已完成','已逾期'][row.status] }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleComplete(row.id)" v-if="row.status !== 2">完成</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="showDialog" title="新建月度计划" width="500px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="计划名称"><el-input v-model="form.planName" /></el-form-item>
        <el-form-item label="计划类型"><el-select v-model="form.planType" style="width: 100%">
          <el-option label="施工进度" :value="0" /><el-option label="材料采购" :value="1" />
          <el-option label="资金计划" :value="2" /><el-option label="二次经营" :value="3" />
        </el-select></el-form-item>
        <el-form-item label="内容"><el-input v-model="form.content" type="textarea" :rows="3" /></el-form-item>
        <el-form-item label="负责人"><el-input v-model="form.responsiblePerson" /></el-form-item>
        <el-form-item label="截止日期"><el-date-picker v-model="form.dueDate" type="date" style="width: 100%" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const plans = ref<any[]>([]);
const showDialog = ref(false);
const loading = ref(false);
const form = reactive({ projectId: 1, year: new Date().getFullYear(), month: new Date().getMonth() + 1, planName: '', planType: 0, content: '', responsiblePerson: '', dueDate: '' });

onMounted(async () => {
  try { const res = await request.get('/monthlyplan/project/1'); if (res.code === 200) plans.value = res.data || []; } catch {}
});
async function handleCreate() {
  loading.value = true;
  try {
    const res = await request.post('/monthlyplan', form);
    if (res.code === 200) { ElMessage.success('创建成功'); showDialog.value = false; }
  } finally { loading.value = false; }
}
async function handleComplete(id: number) {
  try { const res = await request.post('/monthlyplan/' + id + '/complete'); if (res.code === 200) ElMessage.success('已标记完成'); } catch {}
}
</script>