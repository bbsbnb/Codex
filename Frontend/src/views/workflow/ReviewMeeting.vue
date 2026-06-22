<template>
  <div>
    <el-card style="margin-bottom: 16px">
      <el-button type="primary" @click="showCreate = true">+ 发起复盘会议</el-button>
      <el-button @click="loadData" :icon="Refresh">刷新</el-button>
    </el-card>

    <el-card>
      <el-table :data="meetings" stripe empty-text="暂无复盘会议">
        <el-table-column prop="meetingTitle" label="会议标题" min-width="200" />
        <el-table-column prop="meetingDate" label="会议日期" width="160" />
        <el-table-column prop="participants" label="参会人员" width="150" show-overflow-tooltip />
        <el-table-column prop="responsiblePerson" label="责任人" width="100" />
        <el-table-column prop="deadline" label="完成期限" width="120" />
        <el-table-column label="状态" width="90">
          <template #default="{ row }">
            <el-tag :type="['info','success','info'][row.status]" size="small">{{ ['草稿','已完成','已归档'][row.status] }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="showDetail(row)">详情</el-button>
            <el-button type="success" link @click="handleComplete(row.id)" v-if="row.status === 0">完成</el-button>
            <el-button type="warning" link @click="handleArchive(row.id)" v-if="row.status === 1">归档</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="showCreate" title="发起复盘会议" width="600px">
      <el-form :model="form" label-width="100px">
        <el-form-item label="会议标题"><el-input v-model="form.meetingTitle" /></el-form-item>
        <el-form-item label="参会人员"><el-input v-model="form.participants" placeholder="输入参会人员姓名" /></el-form-item>
        <el-form-item label="会议内容"><el-input v-model="form.content" type="textarea" :rows="3" placeholder="议题收集：各业务线汇总问题" /></el-form-item>
        <el-form-item label="会议结论"><el-input v-model="form.conclusions" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="行动事项"><el-input v-model="form.actionItems" type="textarea" :rows="2" /></el-form-item>
        <el-form-item label="责任人"><el-input v-model="form.responsiblePerson" /></el-form-item>
        <el-form-item label="完成期限"><el-date-picker v-model="form.deadline" type="date" style="width: 100%" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">提交</el-button>
      </template>
    </el-dialog>

    <el-drawer v-model="showDetailPanel" :title="detail?.meetingTitle" size="500px">
      <template v-if="detail">
        <el-descriptions :column="1" border size="small">
          <el-descriptions-item label="会议日期">{{ detail.meetingDate }}</el-descriptions-item>
          <el-descriptions-item label="参会人员">{{ detail.participants }}</el-descriptions-item>
          <el-descriptions-item label="会议内容">{{ detail.content }}</el-descriptions-item>
          <el-descriptions-item label="会议结论">{{ detail.conclusions }}</el-descriptions-item>
          <el-descriptions-item label="行动事项">{{ detail.actionItems }}</el-descriptions-item>
          <el-descriptions-item label="责任人">{{ detail.responsiblePerson }}</el-descriptions-item>
          <el-descriptions-item label="完成期限">{{ detail.deadline || '-' }}</el-descriptions-item>
          <el-descriptions-item label="状态">{{ detail.statusName }}</el-descriptions-item>
        </el-descriptions>
      </template>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Refresh } from '@element-plus/icons-vue';
import request from '@/utils/request';

const meetings = ref<any[]>([]);
const showCreate = ref(false);
const showDetailPanel = ref(false);
const loading = ref(false);
const detail = ref<any>(null);
const form = reactive({ projectId: 1, meetingTitle: '', participants: '', content: '', conclusions: '', actionItems: '', responsiblePerson: '', deadline: '' });

onMounted(loadData);
async function loadData() {
  try { var res = await request.get('/review-meeting/project/1'); if (res.code === 200) meetings.value = res.data || []; } catch {}
}
async function handleCreate() {
  if (!form.meetingTitle) { ElMessage.warning('请输入会议标题'); return; }
  loading.value = true;
  try { var res = await request.post('/review-meeting', form); if (res.code === 200) { ElMessage.success('创建成功'); showCreate.value = false; loadData(); } } finally { loading.value = false; }
}
async function handleComplete(id: number) { try { var res = await request.post('/review-meeting/' + id + '/complete'); if (res.code === 200) { ElMessage.success('已完成'); loadData(); } } catch {} }
async function handleArchive(id: number) { try { var res = await request.post('/review-meeting/' + id + '/archive'); if (res.code === 200) { ElMessage.success('已归档'); loadData(); } } catch {} }
function showDetail(row: any) { detail.value = row; showDetailPanel.value = true; }
</script>