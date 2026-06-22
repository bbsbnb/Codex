import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { LoginResponse } from '@/api/auth';

export const useAuthStore = defineStore('auth', () => {
  const user = ref<LoginResponse | null>(null);
  const token = ref<string>(localStorage.getItem('token') || '');

  function setUser(userData: LoginResponse) {
    user.value = userData;
    token.value = userData.token;
    localStorage.setItem('token', userData.token);
    localStorage.setItem('user', JSON.stringify(userData));
  }

  function logout() {
    user.value = null;
    token.value = '';
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  function loadFromStorage() {
    const saved = localStorage.getItem('user');
    if (saved) {
      try {
        user.value = JSON.parse(saved);
        token.value = user.value?.token || '';
      } catch { localStorage.removeItem('user'); }
    }
  }

  return { user, token, setUser, logout, loadFromStorage };
});