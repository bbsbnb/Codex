<template>
  <div>
    <el-button style="margin-bottom: 16px" @click="$router.back()">返回</el-button>
    <el-card v-if="project" style="margin-bottom: 20px">
      <template #header>
        <span style="font-weight: 500">{{ project.projectName }}（{{ project.projectCode }}）</span>
        <el-tag :type="project.status === 0 ? 'success' : 'warning'" style="margin-left: 12px">
          {{ project.status === 0 ? '在建' : project.status === 1 ? '停工' : '竣工' }}
        </el-tag>
      </template>
      <el-descriptions :column="3" border>
        <el-descriptions-item label="合同金额">{{ project.contractAmount.toLocaleString() }} 元</el-descriptions-item>
        <el-descriptions-item label="业主单位">{{ project.clientName }}</el-descriptions-item>
        <el-descriptions-item label="开工日期">{{ project.startDate }}</el-descriptions-item>
        <el-descriptions-item label="竣工日期">{{ project.expectedEndDate || '未设置' }}</el-descriptions-item>
        <el-descriptions-item label="项目地址" :span="2">{{ project.address }}</el-descriptions-item>
        <el-descriptions-item label="描述" :span="3">{{ project.description || '暂无' }}</el-descriptions-item>
      </el-descriptions>
    </el-card>

    <el-card>
      <template #header><span style="font-weight: 500">项目成员</span></template>
      <el-tag v-for="pos in positionTags" :key="pos.value" style="margin: 4px">{{ pos.label }}</el-tag>
      <div v-if="!project" style="color: #999; padding: 20px; text-align: center">暂无成员</div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { getProject } from '@/api/project';

const route = useRoute();
const project = ref<any>(null);
const positionTags = [
  { label: '项目经理', value: 0 }, { label: '生产经理', value: 3 }, { label: '技术总工', value: 2 },
  { label: '施工员', value: 4 }, { label: '质量员', value: 5 }, { label: '技术员', value: 6 },
  { label: '安全员', value: 7 }, { label: '材料员', value: 8 }, { label: '资料员', value: 9 }
];

onMounted(async () => {
  try {
    const res = await getProject(Number(route.params.id));
    if (res.code === 200) project.value = res.data;
  } catch {}
});
</script>