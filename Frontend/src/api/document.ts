import request from '@/utils/request';

export function uploadDocument(file: File, projectId: number, category: number, remark: string) {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('ProjectId', String(projectId));
  formData.append('Category', String(category));
  formData.append('Remark', remark);
  return request.post('/document/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function getDocumentsByProject(projectId: number) {
  return request.get(`/document/project/${projectId}`);
}

export function deleteDocument(id: number) {
  return request.delete(`/document/${id}`);
}

export function searchDocuments(keyword: string, category?: number) {
  const params: any = { keyword };
  if (category !== undefined) params.category = category;
  return request.get('/document/search', { params });
}