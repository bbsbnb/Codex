<template>
  <div class="login-container">
    <div class="login-card">
      <h2 style="text-align: center; margin-bottom: 30px; color: #1e293b">天行建筑智能管理平台</h2>
      <el-form :model="form" :rules="rules" ref="formRef" label-width="0">
        <el-form-item prop="username">
          <el-input v-model="form.username" placeholder="用户名" size="large" prefix-icon="User" />
        </el-form-item>
        <el-form-item prop="password">
          <el-input v-model="form.password" type="password" placeholder="密码" size="large" prefix-icon="Lock" show-password />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" size="large" style="width: 100%" @click="handleLogin" :loading="loading">登 录</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { login } from '@/api/auth';
import { useAuthStore } from '@/store/modules/auth';

const router = useRouter();
const userStore = useAuthStore();
const formRef = ref();
const loading = ref(false);
const form = reactive({ username: '', password: '' });
const rules = {
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
};

async function handleLogin() {
  const valid = await formRef.value.validate().catch(() => false);
  if (!valid) return;
  loading.value = true;
  try {
    const res = await login(form);
    if (res.code === 200 && res.data) {
      userStore.setUser(res.data);
      ElMessage.success('登录成功');
      router.push('/dashboard');
    } else {
      ElMessage.error(res.message || '登录失败');
    }
  } catch { ElMessage.error('登录失败，请检查网络'); }
  finally { loading.value = false; }
}
</script>

<style scoped>
.login-container {
  height: 100vh; display: flex; align-items: center; justify-content: center;
  background: linear-gradient(135deg, #1e293b 0%, #334155 100%);
}
.login-card {
  width: 400px; padding: 40px; background: #fff; border-radius: 12px; box-shadow: 0 20px 60px rgba(0,0,0,0.3);
}
</style>