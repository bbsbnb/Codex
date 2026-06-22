<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <el-radio-group v-model="activeType" @change="loadData">
        <el-radio-button :value="-1">全部</el-radio-button>
        <el-radio-button v-for="t in types" :key="t.value" :value="t.value">{{ t.label }}</el-radio-button>
      </el-radio-group>
      <el-button type="primary" style="float: right" @click="showCreate = true">+ 新建检查记录</el-button>
    </el-card>

    <el-card>
      <el-table :data="filteredData" stripe empty-text="暂无检查记录">
        <el-table-column label="检查类型" width="130">
          <template #default="{ row }">{{ typeName(row.type) }}</template>
        </el-table-column>
        <el-table-column prop="issueDescription" label="问题描述" min-width="200" show-overflow-tooltip />
        <el-table-column prop="issueCategory" label="问题类别" width="120" />
        <el-table-column prop="rectificationRequirement" label="整改要求" width="150" show-overflow-tooltip />
        <el-table-column prop="rectificationDeadline" label="整改期限" width="120" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="['info','warning','success','danger'][row.status]" size="small">{{ ['待处理','整改中','已闭环','已逾期'][row.status] }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="inspectionDate" label="检查日期" width="160" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="showEdit(row)">处理</el-button>
            <el-button type="danger" link @click="handleDelete(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- 新建检查 -->
    <el-dialog v-model="showCreate" :title="'新建' + (activeType >= 0 ? typeName(activeType) : '') + '检查'" width="550px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="检查类型"><el-select v-model="form.type" style="width: 100%">
          <el-option v-for="t in types" :key="t.value" :label="t.label" :value="t.value" />
        </el-select></el-form-item>
        <el-form-item label="问题描述"><el-input v-model="form.issueDescription" type="textarea" :rows="3" /></el-form-item>
        <el-form-item label="问题类别"><el-input v-model="form.issueCategory" placeholder="如: 材料质量/施工隐患/进度滞后等" /></el-form-item>
        <el-form-item label="整改要求"><el-input v-model="form.rectificationRequirement" /></el-form-item>
        <el-form-item label="整改期限"><el-date-picker v-model="form.rectificationDeadline" type="date" style="width: 100%" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">提交</el-button>
      </template>
    </el-dialog>

    <!-- 处理弹窗 -->
    <el-dialog v-model="showEditDialog" title="处理检查记录" width="500px">
      <el-form :model="editForm" label-width="100px">
        <el-form-item label="问题描述">{{ editForm.issueDescription }}</el-form-item>
        <el-form-item label="处理结果"><el-input v-model="editForm.result" type="textarea" :rows="3" /></el-form-item>
        <el-form-item label="状态"><el-select v-model="editForm.status" style="width: 100%">
          <el-option label="整改中" :value="1" />
          <el-option label="已闭环" :value="2" />
        </el-select></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showEditDialog = false">取消</el-button>
        <el-button type="primary" @click="handleUpdate" :loading="loading">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const records = ref<any[]>([]);
const activeType = ref(-1);
const showCreate = ref(false);
const showEditDialog = ref(false);
const loading = ref(false);

const types = [
  { label: '质量管理', value: 0 }, { label: '安全管理', value: 1 },
  { label: '进度管理', value: 2 }, { label: '文明施工', value: 3 },
  { label: '合同管理', value: 4 }, { label: '材料管理', value: 5 },
  { label: '对下招标', value: 6 }, { label: '资料管理', value: 7 }
];

const form = reactive({ projectId: 1, type: 0, issueDescription: '', issueCategory: '', rectificationRequirement: '', rectificationDeadline: '' });
const editForm = reactive({ id: 0, result: '', status: 1, issueDescription: '' });

const filteredData = computed(() => {
  if (activeType.value === -1) return records.value;
  return records.value.filter(function(r: any) { return r.type === activeType.value; });
});

function typeName(v: number) { var t = types.find(function(ti) { return ti.value === v; }); return t ? t.label : v; }

onMounted(loadData);
async function loadData() {
  try {
    var res = await request.get('/inspection/project/1');
    if (res.code === 200) records.value = res.data || [];
  } catch {}
}

async function handleCreate() {
  if (!form.issueDescription) { ElMessage.warning('请描述问题'); return; }
  loading.value = true;
  try {
    var res = await request.post('/inspection', form);
    if (res.code === 200) { ElMessage.success('创建成功'); showCreate.value = false; loadData(); }
  } finally { loading.value = false; }
}

function showEdit(row: any) {
  editForm.id = row.id; editForm.result = row.result || ''; editForm.status = row.status; editForm.issueDescription = row.issueDescription;
  showEditDialog.value = true;
}

async function handleUpdate() {
  loading.value = true;
  try {
    var res = await request.put('/inspection/' + editForm.id, { result: editForm.result, status: editForm.status });
    if (res.code === 200) { ElMessage.success('保存成功'); showEditDialog.value = false; loadData(); }
  } finally { loading.value = false; }
}

async function handleDelete(id: number) {
  try {
    var res = await request.delete('/inspection/' + id);
    if (res.code === 200) { ElMessage.success('已删除'); loadData(); }
  } catch {}
}
</script>