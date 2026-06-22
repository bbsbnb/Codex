<template>
  <div>
    <el-card style="margin-bottom: 20px">
      <el-upload
        :action="uploadUrl"
        :headers="uploadHeaders"
        :data="uploadData"
        :on-success="handleUploadSuccess"
        :on-error="() => ElMessage.error('上传失败')"
        :show-file-list="false"
        :limit="1"
      >
        <el-button type="primary">上传资料</el-button>
        <template #tip><div style="color: #999; font-size: 12px; margin-top: 8px">支持 PDF/Word/Excel/图片，最大 100MB</div></template>
      </el-upload>
    </el-card>

    <el-card>
      <el-table :data="documents" stripe empty-text="暂无资料">
        <el-table-column prop="fileName" label="文件名" min-width="250" />
        <el-table-column label="分类" width="150">
          <template #default="{ row }">{{ categoryLabels[row.category] || row.category }}</template>
        </el-table-column>
        <el-table-column prop="fileSize" label="大小" width="100">
          <template #default="{ row }">{{ (row.fileSize / 1024).toFixed(1) }} KB</template>
        </el-table-column>
        <el-table-column prop="uploadDate" label="上传时间" width="180" />
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="handleDelete(row.id)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { getAllProjects } from '@/api/project';
import request from '@/utils/request';
import { getDocumentsByProject } from '@/api/document';

const documents = ref<any[]>([]);
const projects = ref<any[]>([]);
const selectedProject = ref<number>(0);
const categoryLabels = ['施工合同', '招投标文件', '建造合同', '商务策划', '一次经营交底', '施工组织设计'];
const uploadUrl = '/api/document/upload';
const uploadHeaders = { Authorization: 'Bearer ' + localStorage.getItem('token') };
const uploadData = { ProjectId: 1, Category: 0, Remark: '' };

onMounted(async () => {
  try {
    const res = await getAllProjects();
    if (res.code === 200) { projects.value = res.data || []; if (projects.value.length) selectedProject.value = projects.value[0].id; }
  } catch {}
});
function handleUploadSuccess() { ElMessage.success('上传成功'); }
function handleDelete(id: number) { request.delete('/document/' + id).then(() => ElMessage.success('已删除')); }
</script>