import request from '@/utils/request';

export interface ProjectData {
  projectCode: string;
  projectName: string;
  contractAmount: number;
  startDate: string;
  expectedEndDate?: string;
  address: string;
  clientName: string;
  description: string;
}

export interface ProjectResponse {
  id: number;
  projectCode: string;
  projectName: string;
  contractAmount: number;
  startDate: string;
  expectedEndDate: string | null;
  actualEndDate: string | null;
  status: number;
  address: string;
  clientName: string;
  description: string;
  createdAt: string;
  memberCount: number;
}

export function createProject(data: ProjectData) {
  return request.post('/project', data);
}

export function updateProject(id: number, data: any) {
  return request.put(`/project/${id}`, data);
}

export function deleteProject(id: number) {
  return request.delete(`/project/${id}`);
}

export function getProject(id: number) {
  return request.get(`/project/${id}`);
}

export function getAllProjects() {
  return request.get('/project');
}

export function getProjectsByStatus(status: number) {
  return request.get(`/project/status/${status}`);
}

export function assignMember(projectId: number, userId: number, position: number) {
  return request.post(`/project/${projectId}/member/${userId}?position=${position}`);
}

export function removeMember(projectId: number, userId: number) {
  return request.delete(`/project/${projectId}/member/${userId}`);
}