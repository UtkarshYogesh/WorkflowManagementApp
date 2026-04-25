import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Projects from '../views/Projects.vue'
import ProjectDetail from '../views/ProjectDetail.vue'
import Features from '../views/Features.vue'
import FeatureDetail from '../views/FeatureDetail.vue'
import Backlogs from '../views/Backlogs.vue'
import BacklogDetail from '../views/BacklogDetail.vue'
import Tasks from '../views/Tasks.vue'
import TaskDetail from '../views/TaskDetail.vue'

const routes = [
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

export default createRouter({
  history: createWebHistory(),
  routes
})