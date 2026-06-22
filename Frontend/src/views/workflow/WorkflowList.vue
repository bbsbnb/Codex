<template>
  <div>
    <!-- 统计卡片 -->
    <el-row :gutter="16" style="margin-bottom: 16px">
      <el-col :span="4" v-for="s in flowStats" :key="s.type">
        <el-card shadow="hover" style="text-align: center; cursor: pointer"
          :style="activeTab === s.type ? 'border-color: #3b82f6; border-width: 2px' : ''"
          @click="switchTab(s.type)">
          <div style="font-size: 24px; font-weight: bold; color: #3b82f6">{{ s.count }}</div>
          <div style="font-size: 12px; color: #64748b">{{ s.label }}</div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 操作栏 -->
    <el-card style="margin-bottom: 16px">
      <div style="display: flex; align-items: center; justify-content: space-between">
        <div>
          <el-button type="primary" @click="showCreate = true">+ 新建流程</el-button>
          <el-button @click="loadData" :icon="Refresh">刷新</el-button>
        </div>
        <el-tag type="info">
          {{ activeTab === -1 ? '全部流程' : flowTypeName(activeTab) }}
        </el-tag>
      </div>
    </el-card>

    <!-- 流程列表 + 审批链预览 -->
    <el-row :gutter="16">
      <el-col :span="16">
        <el-card>
          <el-table :data="flows" stripe empty-text="暂无流程" @row-click="showDetail=true; selectedFlow=$event">
            <el-table-column prop="flowNo" label="编号" width="140" />
            <el-table-column prop="title" label="标题" min-width="160" />
            <el-table-column label="金额" width="130">
              <template #default="{ row }">{{ row.amount ? row.amount.toLocaleString() + ' 元' : '-' }}</template>
            </el-table-column>
            <el-table-column label="状态" width="90">
              <template #default="{ row }">
                <el-tag :type="statusType(row.status)" size="small">{{ statusLabel(row.status) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="initiatedAt" label="发起时间" width="160" />
          </el-table>
          <el-pagination v-if="flows.length > 0" background layout="total,prev,pager,next"
            :total="flows.length" :page-size="20" style="margin-top: 12px; justify-content: center" />
        </el-card>
      </el-col>

      <!-- 右侧审批链预览 -->
      <el-col :span="8">
        <el-card v-if="selectedFlow" style="margin-bottom: 12px">
          <template #header>
            <span style="font-weight: 500">审批进度 - {{ selectedFlow.flowNo }}</span>
          </template>
          <div v-if="workflowNodes.length > 0">
            <div v-for="(node, idx) in workflowNodes" :key="idx" style="display: flex; align-items: center; margin-bottom: 8px">
              <div :style="{
                width: '24px', height: '24px', borderRadius: '50%',
                background: node.nodeIndex > selectedFlow.currentNodeId ? '#22c55e' : node.nodeIndex === selectedFlow.currentNodeId ? '#f59e0b' : '#e5e7eb',
                color: node.nodeIndex === selectedFlow.currentNodeId ? '#fff' : node.nodeIndex > selectedFlow.currentNodeId ? '#fff' : '#999',
                display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '12px', marginRight: '8px', flexShrink: 0
              }">{{ idx + 1 }}</div>
              <div style="flex: 1">
                <div style="font-size: 13px; font-weight: 500">{{ node.role }}</div>
                <div style="font-size: 11px; color: #94a3b8">{{ node.description }}</div>
              </div>
            </div>
          </div>
          <div v-else style="color: #999; text-align: center; padding: 12px">暂无审批节点</div>
        </el-card>

        <el-card v-if="selectedFlow">
          <template #header><span style="font-weight: 500">审批操作</span></template>
          <el-input v-model="approvalComment" type="textarea" :rows="3" placeholder="请输入审批意见" style="margin-bottom: 12px" />
          <div style="display: flex; gap: 8px">
            <el-button type="success" @click="handleApprove" :disabled="selectedFlow.status !== 0 && selectedFlow.status !== 1">通过</el-button>
            <el-button type="danger" @click="handleReject" :disabled="selectedFlow.status !== 0 && selectedFlow.status !== 1">驳回</el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 新建流程弹窗 -->
    <el-dialog v-model="showCreate" title="新建流程" width="520px">
      <el-form :model="createForm" label-width="100px">
        <el-form-item label="流程类型"><el-select v-model="createForm.flowType" style="width: 100%">
          <el-option v-for="f in flowTypes" :key="f.value" :label="f.label" :value="f.value" />
        </el-select></el-form-item>
        <el-form-item label="标题"><el-input v-model="createForm.title" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="createForm.description" type="textarea" :rows="3" /></el-form-item>
        <el-form-item label="涉及金额"><el-input-number v-model="createForm.amount" :min="0" style="width: 100%" /></el-form-item>

        <template v-if="createForm.flowType === 0">
          <el-form-item label="联系单位"><el-input v-model="createForm.contactParty" /></el-form-item>
        </template>
        <template v-if="createForm.flowType === 1">
          <el-form-item label="材料名称"><el-input v-model="createForm.materialName" /></el-form-item>
          <el-form-item label="规格"><el-input v-model="createForm.materialSpec" /></el-form-item>
          <el-form-item label="品牌"><el-input v-model="createForm.brand" /></el-form-item>
        </template>
        <template v-if="createForm.flowType === 3">
          <el-form-item label="索赔发生日"><el-date-picker v-model="createForm.claimOccurrenceDate" type="date" style="width: 100%" /></el-form-item>
        </template>
        <template v-if="createForm.flowType === 4">
          <el-form-item label="变更通知单号"><el-input v-model="createForm.changeNoticeNo" /></el-form-item>
        </template>
      </el-form>
      <template #footer>
        <el-button @click="showCreate = false">取消</el-button>
        <el-button type="primary" @click="handleCreate" :loading="loading">提交</el-button>
      </template>
    </el-dialog>

    <!-- 流程详情抽屉 -->
    <el-drawer v-model="showDetail" :title="selectedFlow?.flowNo + ' - ' + selectedFlow?.title" size="600px">
      <template v-if="selectedFlow">
        <el-descriptions :column="2" border size="small">
          <el-descriptions-item label="流程编号">{{ selectedFlow.flowNo }}</el-descriptions-item>
          <el-descriptions-item label="流程类型">{{ flowTypeName(selectedFlow.flowType) }}</el-descriptions-item>
          <el-descriptions-item label="金额">{{ selectedFlow.amount?.toLocaleString() + ' 元' || '-' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="statusType(selectedFlow.status)" size="small">{{ statusLabel(selectedFlow.status) }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="发起时间">{{ selectedFlow.initiatedAt }}</el-descriptions-item>
          <el-descriptions-item label="完成时间">{{ selectedFlow.completedAt || '-' }}</el-descriptions-item>
          <el-descriptions-item label="描述" :span="2">{{ selectedFlow.description || '暂无' }}</el-descriptions-item>
        </el-descriptions>

        <div style="margin-top: 20px; font-weight: 500; margin-bottom: 12px">审批记录</div>
        <el-timeline>
          <el-timeline-item v-for="a in selectedFlow.approvals" :key="a.id" :timestamp="a.approvedAt">
            <div style="font-weight: 500">{{ a.approverName }}</div>
            <div><el-tag :type="a.action === 1 ? 'success' : a.action === 2 ? 'danger' : 'info'" size="small">
              {{ ['待处理','已通过','已驳回','已退回'][a.action] }}
            </el-tag></div>
            <div style="color: #64748b; margin-top: 4px">{{ a.comment }}</div>
          </el-timeline-item>
        </el-timeline>
      </template>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import { Refresh } from '@element-plus/icons-vue';
import request from '@/utils/request';

const flows = ref<any[]>([]);
const allFlows = ref<any[]>([]);
const selectedFlow = ref<any>(null);
const showCreate = ref(false);
const showDetail = ref(false);
const loading = ref(false);
const activeTab = ref(-1);
const approvalComment = ref('');

const flowTypes = [
  { label: '工联单', value: 0 }, { label: '认质认价', value: 1 }, { label: '签证', value: 2 },
  { label: '索赔', value: 3 }, { label: '设计变更', value: 4 }
];

const flowStats = computed(() => {
  var tabs = [{ type: -1, label: '全部', count: allFlows.value.length }];
  flowTypes.forEach(function(ft) {
    tabs.push({ type: ft.value, label: ft.label, count: allFlows.value.filter(function(f) { return f.flowType === ft.value; }).length });
  });
  return tabs;
});

const workflowNodes = computed(() => {
  if (!selectedFlow.value) return [];
  return [
    { nodeIndex: 3, role: '施工员', description: '编制草签单' },
    { nodeIndex: 2, role: '生产经理', description: '审核' },
    { nodeIndex: 1, role: '项目经理', description: '审批' },
    { nodeIndex: 0, role: '公司领导', description: '终审归档' }
  ];
});

const createForm = reactive({
  projectId: 1, flowType: 0, title: '', description: '', amount: 0,
  contactParty: '', materialName: '', materialSpec: '', brand: '',
  claimOccurrenceDate: '', changeNoticeNo: ''
});

onMounted(loadData);

function switchTab(type: number) { activeTab.value = type; filterFlows(); }

function filterFlows() {
  if (activeTab.value === -1) flows.value = [...allFlows.value];
  else flows.value = allFlows.value.filter(function(f) { return f.flowType === activeTab.value; });
}

function flowTypeName(v: number) { var ft = flowTypes.find(function(f) { return f.value === v; }); return ft ? ft.label : v; }
function statusLabel(s: number) { return ['草稿','待审批','已通过','已驳回','已归档','已取消'][s] || s; }
function statusType(s: number) { return ['info','warning','success','danger','info','info'][s] || 'info'; }

async function loadData() {
  try {
    var res = await request.get('/businessflow/project/1');
    if (res.code === 200) { allFlows.value = res.data || []; filterFlows(); }
  } catch {}
}

async function handleCreate() {
  if (!createForm.title) { ElMessage.warning('请输入标题'); return; }
  loading.value = true;
  try {
    var res = await request.post('/businessflow', { projectId: 1, flowType: createForm.flowType, title: createForm.title, description: createForm.description, amount: createForm.amount });
    if (res.code === 200) { ElMessage.success('创建成功'); showCreate.value = false; loadData(); }
  } finally { loading.value = false; }
}

async function handleApprove() {
  if (!selectedFlow.value) return;
  try {
    var res = await request.post('/businessflow/submit', { flowId: selectedFlow.value.id, action: 1, comment: approvalComment.value || '审批通过' });
    if (res.code === 200) { ElMessage.success('审批完成'); approvalComment.value = ''; loadData(); showDetail.value = false; }
  } catch {}
}

async function handleReject() {
  if (!selectedFlow.value) return;
  try {
    var res = await request.post('/businessflow/submit', { flowId: selectedFlow.value.id, action: 2, comment: approvalComment.value || '驳回' });
    if (res.code === 200) { ElMessage.success('已驳回'); approvalComment.value = ''; loadData(); showDetail.value = false; }
  } catch {}
}
</script>