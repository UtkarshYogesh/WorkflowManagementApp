import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { getStoredUser, restoreSession } from '../services/authApi'
import { ability, updateAbilityFor, type AppAction, type AppSubject } from '../permissions/ability'

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
      { path: '/projects', component: Projects, meta: { permission: { action: 'read', subject: 'Project' } } },
      { path: '/projects/:projectId', component: ProjectDetail, meta: { permission: { action: 'read', subject: 'Project' } } },
      { path: '/projects/:projectId/features/:featureId', component: FeatureDetail, meta: { permission: { action: 'read', subject: 'Feature' } } },
      { path: '/features', component: Features, meta: { permission: { action: 'read', subject: 'Feature' } } },
      { path: '/features/:featureId', component: FeatureDetail, meta: { permission: { action: 'read', subject: 'Feature' } } },
      { path: '/backlogs', component: Backlogs, meta: { permission: { action: 'read', subject: 'Backlog' } } },
      { path: '/backlogs/:backlogId', component: BacklogDetail, meta: { permission: { action: 'read', subject: 'Backlog' } } },
      { path: '/tasks', component: Tasks, meta: { permission: { action: 'read', subject: 'Task' } } },
      { path: '/tasks/:taskId', component: TaskDetail, meta: { permission: { action: 'read', subject: 'Task' } } },
    ]
  },
  
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Route guard for authentication
router.beforeEach(async (to, from, next) => {
  const isUserAuthenticated = await restoreSession()
  updateAbilityFor(getStoredUser())
  const isPublicRoute = to.meta.isPublic === true
  const permission = to.meta.permission as { action: AppAction; subject: AppSubject } | undefined

  if (!isUserAuthenticated && !isPublicRoute) {
    // Not authenticated and trying to access protected route
    next('/login')
  } else if (isUserAuthenticated && isPublicRoute) {
    // Already authenticated and trying to access public route
    next('/')
  } else if (permission && !ability.can(permission.action, permission.subject)) {
    next('/')
  } else {
    // Allow navigation
    next()
  }
})

export default router
