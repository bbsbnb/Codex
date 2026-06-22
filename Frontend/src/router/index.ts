import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  { path: '/login', name: 'Login', component: () => import('@/views/Login.vue'), meta: { title: '登录' } },
  {
    path: '/', component: () => import('@/layouts/MainLayout.vue'), redirect: '/dashboard',
    children: [
      { path: 'dashboard', name: 'Dashboard', component: () => import('@/views/Dashboard.vue'), meta: { title: '工作台', icon: 'Odometer' } },
      { path: 'project', name: 'ProjectList', component: () => import('@/views/project/ProjectList.vue'), meta: { title: '项目管理', icon: 'Folder' } },
      { path: 'project/:id', name: 'ProjectDetail', component: () => import('@/views/project/ProjectDetail.vue'), meta: { title: '项目详情', hidden: true } },
      { path: 'document', name: 'DocumentList', component: () => import('@/views/project/DocumentList.vue'), meta: { title: '一次经营资料库', icon: 'Files' } },
      { path: 'plan', name: 'PlanList', component: () => import('@/views/workflow/PlanList.vue'), meta: { title: '月度计划', icon: 'Calendar' } },
      { path: 'workflow', name: 'WorkflowList', component: () => import('@/views/workflow/WorkflowList.vue'), meta: { title: '变更索赔中心', icon: 'Edit' } },
      { path: 'workflow/:id', name: 'FlowDetail', component: () => import('@/views/workflow/FlowDetail.vue'), meta: { title: '流程详情', hidden: true } },
      { path: 'workflow-statistics', name: 'WorkflowStatistics', component: () => import('@/views/workflow/WorkflowStatistics.vue'), meta: { title: '业务台账', icon: 'DataBoard' } },
      { path: 'settlement', name: 'SettlementList', component: () => import('@/views/material/SettlementList.vue'), meta: { title: '结算管理', icon: 'Document' } },
      { path: 'contract-table', name: 'ContractTable', component: () => import('@/views/material/ContractTable.vue'), meta: { title: '建造合同三张表', icon: 'TrendCharts' } },
      { path: 'inspection', name: 'InspectionCenter', component: () => import('@/views/material/InspectionCenter.vue'), meta: { title: '检查监管中心', icon: 'Monitor' } },
      { path: 'payment', name: 'PaymentTracking', component: () => import('@/views/material/PaymentTracking.vue'), meta: { title: '回款管理', icon: 'Coin' } },
      // 阶段五
      { path: 'report', name: 'Report', component: () => import('@/views/workflow/ReportCenter.vue'), meta: { title: '经营分析', icon: 'DataAnalysis' } },
      { path: 'review-meeting', name: 'ReviewMeeting', component: () => import('@/views/workflow/ReviewMeeting.vue'), meta: { title: '复盘会议', icon: 'ChatLineSquare' } },
      { path: 'notification', name: 'NotificationCenter', component: () => import('@/views/system/NotificationCenter.vue'), meta: { title: '消息通知', icon: 'Bell' } },
      { path: 'warning', name: 'Warning', component: () => import('@/views/system/WarningCenter.vue'), meta: { title: '预警中心', icon: 'Warning' } },
    ]
  }
];

const router = createRouter({ history: createWebHistory(), routes });
router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token');
  if (to.name !== 'Login' && !token) { next({ name: 'Login' }); }
  else { if (to.meta.title) document.title = to.meta.title + ' - 天行建筑智能管理平台'; next(); }
});
export default router;