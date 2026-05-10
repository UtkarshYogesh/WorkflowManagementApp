import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { isAuthenticated } from '../services/authApi'

// View Imports
import Dashboard from '../views/Dashboard.vue'
import Projects from '../views/Projects.vue'
import ProjectDetail from '../views/ProjectDetail.vue'
import Features from '../views/Features.vue'
import FeatureDetail from '../views/FeatureDetail.vue'
import Backlogs from '../views/Backlogs.vue'
import BacklogDetail from '../views/BacklogDetail.vue'
import Tasks from '../views/Tasks.vue'
import TaskDetail from '../views/TaskDetail.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import HomePage from '@/pages/HomePage.vue'

const publicRoutes = ['/login', '/register']

const routes: RouteRecordRaw[] = [
  { path: '/login', component: Login, meta: { isPublic: true } },
  { path: '/register', component: Register, meta: { isPublic: true } },
  {
    path: '/',
    component: HomePage,
    children: [
      { path: '/', component: Dashboard },
      { path: '/projects', component: Projects },
      { path: '/projects/:projectId', component: ProjectDetail },
      { path: '/projects/:projectId/features/:featureId', component: FeatureDetail },
      { path: '/features', component: Features },
      { path: '/features/:featureId', component: FeatureDetail },
      { path: '/backlogs', component: Backlogs },
      { path: '/backlogs/:backlogId', component: BacklogDetail },
      { path: '/tasks', component: Tasks },
      { path: '/tasks/:taskId', component: TaskDetail },
    ]
  },
  
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Route guard for authentication
router.beforeEach((to, from, next) => {
  const isUserAuthenticated = isAuthenticated()
  const isPublicRoute = to.meta.isPublic === true

  if (!isUserAuthenticated && !isPublicRoute) {
    // Not authenticated and trying to access protected route
    next('/login')
  } else if (isUserAuthenticated && isPublicRoute) {
    // Already authenticated and trying to access public route
    next('/')
  } else {
    // Allow navigation
    next()
  }
})

export default router