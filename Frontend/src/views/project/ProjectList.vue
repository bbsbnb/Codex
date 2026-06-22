<template>
  <div>
    <div style="display: flex; justify-content: space-between; margin-bottom: 20px">
      <el-button type="primary" @click="showDialog = true">+ 新建项目</el-button>
    </div>
    <el-card>
      <el-table :data="projects" stripe style="width: 100%">
        <el-table-column prop="projectCode" label="项目编号" width="120" />
        <el-table-column prop="projectName" label="项目名称" min-width="200" />
        <el-table-column prop="clientName" label="业主单位" width="150" />
        <el-table-column prop="contractAmount" label="合同金额" width="150">
          <template #default="{ row }">{{ row.contractAmount.toLocaleString() }} 元</template>
        </el-table-column>
        <el-table-column prop="startDate" label="开工日期" width="120" />
        <el-table-column prop="memberCount" label="成员" width="60" align="center" />
        <el-table-column label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'success' : row.status === 1 ? 'warning' : 'info'">
              {{ statusLabels[row.status] }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="goDetail(row.id)">查看</el-button>
            <el-button type="danger" link @click="handleDelete(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="showDialog" title="新建项目" width="600px" :close-on-click-modal="false">
      <el-form :model="form" :rules="rules" ref="formRef" label-width="100px">
        <el-form-item label="项目编号" prop="projectCode"><el-input v-model="form.projectCode" /></el-form-item>
        <el-form-item label="项目名称" prop="projectName"><el-input v-model="form.projectName" /></el-form-item>
        <el-form-item label="合同金额"><el-input-number v-model="form.contractAmount" :min="0" style="width: 100%" /></el-form-item>
        <el-form-item label="业主单位"><el-input v-model="form.clientName" /></el-form-item>
        <el-form-item label="开工日期"><el-date-picker v-model="form.startDate" type="date" style="width: 100%" /></el-form-item>
        <el-form-item label="竣工日期"><el-date-picker v-model="form.expectedEndDate" type="date" style="width: 100%" /></el-form-item>
        <el-form-item label="项目地址"><el-input v-model="form.address" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" :rows="3" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { getAllProjects, createProject, deleteProject } from '@/api/project';

const router = useRouter();
const projects = ref<any[]>([]);
const showDialog = ref(false);
const loading = ref(false);
const formRef = ref();
const statusLabels = ['在建', '停工', '竣工'];

const form = reactive({
  projectCode: '', projectName: '', contractAmount: 0, clientName: '',
  startDate: '', expectedEndDate: '', address: '', description: ''
});
const rules = {
  projectCode: [{ required: true, message: '请输入项目编号', trigger: 'blur' }],
  projectName: [{ required: true, message: '请输入项目名称', trigger: 'blur' }]
};

onMounted(loadProjects);
async function loadProjects() {
  try {
    const res = await getAllProjects();
    if (res.code === 200) projects.value = res.data || [];
  } catch {}
}
async function handleCreate() {
  const valid = await formRef.value.validate().catch(() => false);
  if (!valid) return;
  loading.value = true;
  try {
    const res = await createProject(form);
    if (res.code === 200) { ElMessage.success('创建成功'); showDialog.value = false; loadProjects(); }
    else ElMessage.error(res.message || '创建失败');
  } catch {} finally { loading.value = false; }
}
async function handleDelete(id: number) {
  try {
    await ElMessageBox.confirm('确定删除该项目吗?', '提示');
    const res = await deleteProject(id);
    if (res.code === 200) { ElMessage.success('已删除'); loadProjects(); }
  } catch {}
}
function goDetail(id: number) { router.push('/project/' + id); }
</script>