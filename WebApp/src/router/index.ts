import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Projects from '../views/Projects.vue'
import ProjectDetail from '../views/ProjectDetail.vue'
import FeatureDetail from '../views/FeatureDetail.vue'
import BacklogDetail from '../views/BacklogDetail.vue'

const routes = [
  { path: '/', component: Dashboard },
  { path: '/projects', component: Projects },
  { path: '/projects/:projectId', component: ProjectDetail },
  { path: '/projects/:projectId/features/:featureId', component: FeatureDetail },
  { path: '/projects/:projectId/features/:featureId/backlogs/:backlogId', component: BacklogDetail },
]

export default createRouter({
  history: createWebHistory(),
  routes
})