import request from '@/utils/request';

export interface LoginData {
  username: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  username: string;
  realName: string;
  token: string;
  roles: string[];
  departmentIds: number[];
}

export function login(data: LoginData) {
  return request.post('/auth/login', data);
}

export function register(data: { username: string; password: string; realName: string; phone: string; departmentId: number }) {
  return request.post('/auth/register', data);
}