<template>
  <div>
    <el-button style="margin-bottom: 16px" @click="$router.back()">返回</el-button>

    <el-row :gutter="16">
      <el-col :span="16">
        <el-card style="margin-bottom: 16px">
          <template #header>
            <span style="font-weight: 500">{{ flow?.flowNo }} - {{ flow?.title }}</span>
            <el-tag :type="flow ? statusType(flow.status) : 'info'" style="margin-left: 12px">{{ flow ? statusLabel(flow.status) : '' }}</el-tag>
          </template>
          <el-descriptions :column="2" border v-if="flow">
            <el-descriptions-item label="流程类型">{{ flow.flowTypeName }}</el-descriptions-item>
            <el-descriptions-item label="状态">{{ flow.statusName }}</el-descriptions-item>
            <el-descriptions-item label="涉及金额">{{ flow.amount?.toLocaleString() + ' 元' || '-' }}</el-descriptions-item>
            <el-descriptions-item label="发起时间">{{ flow.initiatedAt }}</el-descriptions-item>
            <el-descriptions-item label="完成时间">{{ flow.completedAt || '-' }}</el-descriptions-item>
            <el-descriptions-item label="描述" :span="2">{{ flow.description || '暂无' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card>
          <template #header><span style="font-weight: 500">审批记录</span></template>
          <div v-if="flow?.approvals?.length">
            <div v-for="a in flow.approvals" :key="a.id" style="padding: 12px 0; border-bottom: 1px solid #f1f5f9">
              <div style="display: flex; justify-content: space-between">
                <span style="font-weight: 500">{{ a.approverName }}</span>
                <el-tag :type="a.action === 1 ? 'success' : a.action === 2 ? 'danger' : 'info'" size="small">
                  {{ ['待处理','已通过','已驳回','已退回'][a.action] }}
                </el-tag>
              </div>
              <div style="color: #64748b; font-size: 13px; margin-top: 4px">{{ a.comment }}</div>
              <div style="color: #94a3b8; font-size: 12px; margin-top: 2px">{{ a.approvedAt }}</div>
            </div>
          </div>
          <div v-else style="color: #999; text-align: center; padding: 20px">暂无审批记录</div>
        </el-card>
      </el-col>

      <el-col :span="8">
        <el-card>
          <template #header><span style="font-weight: 500">审批流程</span></template>
          <div v-if="flow?.workflowNodes?.length">
            <div v-for="(node, idx) in flow.workflowNodes" :key="idx" style="margin-bottom: 8px">
              <div :style="{
                padding: '8px 12px', borderRadius: '6px',
                background: node.nodeIndex > flow.currentNodeId ? '#f0fdf4' : node.nodeIndex === flow.currentNodeId ? '#fef3c7' : '#f8fafc',
                border: node.nodeIndex === flow.currentNodeId ? '1px solid #f59e0b' : '1px solid #e5e7eb'
              }">
                <div style="display: flex; justify-content: space-between">
                  <span style="font-weight: 500; font-size: 13px">{{ node.role }}</span>
                  <el-tag v-if="node.nodeIndex > flow.currentNodeId" type="success" size="small">已通过</el-tag>
                  <el-tag v-else-if="node.nodeIndex === flow.currentNodeId" type="warning" size="small">进行中</el-tag>
                  <el-tag v-else type="info" size="small">待处理</el-tag>
                </div>
                <div style="font-size: 12px; color: #64748b; margin-top: 2px">{{ node.description }}</div>
              </div>
            </div>
          </div>
          <div v-else style="color: #999; text-align: center; padding: 20px">暂无流程节点</div>
        </el-card>

        <el-card style="margin-top: 16px" v-if="flow && (flow.status === 0 || flow.status === 1)">
          <template #header><span style="font-weight: 500">审批操作</span></template>
          <el-input v-model="comment" type="textarea" :rows="3" placeholder="请输入审批意见" style="margin-bottom: 12px" />
          <div style="display: flex; gap: 8px">
            <el-button type="success" @click="handleAction(1)" style="flex: 1">通过</el-button>
            <el-button type="danger" @click="handleAction(2)" style="flex: 1">驳回</el-button>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import request from '@/utils/request';

const route = useRoute();
const flow = ref<any>(null);
const comment = ref('');

function statusLabel(s: number) { return ['草稿','待审批','已通过','已驳回','已归档','已取消'][s] || s; }
function statusType(s: number) { return ['info','warning','success','danger','info','info'][s] || 'info'; }

onMounted(async () => {
  try {
    var res = await request.get('/businessflow/' + route.params.id);
    if (res.code === 200) flow.value = res.data;
  } catch {}
});

async function handleAction(action: number) {
  if (!comment.value) { ElMessage.warning('请填写审批意见'); return; }
  try {
    var res = await request.post('/businessflow/submit', { flowId: flow.value.id, action: action, comment: comment.value });
    if (res.code === 200) { ElMessage.success('操作成功'); location.reload(); }
  } catch {}
}
</script>