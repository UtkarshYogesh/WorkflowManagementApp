<template>
  <section class="page dashboard-page">
    <div class="page-header">
      <div>
        <p class="eyebrow">Overview</p>
        <h1>Dashboard</h1>
        <p class="subtitle">A quick view of active work across projects, features, backlog, and tasks.</p>
      </div>
      <div class="page-actions">
        <router-link class="button primary" to="/projects">New project</router-link>
        <router-link class="button secondary" to="/tasks">View tasks</router-link>
      </div>
    </div>

    <div class="stats-grid">
      <article class="stat-card">
        <span>Projects</span>
        <strong>{{ projects?.length ?? 0 }}</strong>
      </article>
      <article class="stat-card">
        <span>Features</span>
        <strong>{{ features?.length ?? 0 }}</strong>
      </article>
      <article class="stat-card">
        <span>Backlog items</span>
        <strong>{{ backlogs?.length ?? 0 }}</strong>
      </article>
      <article class="stat-card">
        <span>Tasks</span>
        <strong>{{ tasks?.length ?? 0 }}</strong>
      </article>
    </div>

    <div class="dashboard-grid">
      <section class="panel work-panel">
        <div class="section-title">
          <h2>Task status</h2>
          <p>Current task distribution.</p>
        </div>
        <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
        <div v-else-if="!tasks?.length" class="empty-state">No tasks created yet.</div>
        <div v-else class="status-list">
          <div v-for="item in taskStatusSummary" :key="item.status" class="status-row">
            <span>{{ item.status }}</span>
            <div class="status-meter">
              <span :style="{ width: `${item.percent}%` }"></span>
            </div>
            <strong>{{ item.count }}</strong>
          </div>
        </div>
      </section>

      <section class="panel">
        <div class="section-title">
          <h2>Recent projects</h2>
          <p>Open a project to plan features and backlog items.</p>
        </div>
        <div v-if="isProjectsLoading" class="empty-state">Loading projects...</div>
        <div v-else-if="!projects?.length" class="empty-state">No projects found.</div>
        <div v-else class="project-list">
          <router-link
            v-for="project in recentProjects"
            :key="project.projectId"
            class="project-row"
            :to="`/projects/${project.projectId}`"
          >
            <span>
              <strong>{{ project.name }}</strong>
              <small>{{ project.description || 'No description' }}</small>
            </span>
            <small>{{ formatDate(project.createdAt) }}</small>
          </router-link>
        </div>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useProjects } from '../composables/useProjects'
import { useFeatures } from '../composables/useFeatures'
import { useBacklogs } from '../composables/useBacklogs'
import { useTasks } from '../composables/useTasks'

const { data: projects, isLoading: isProjectsLoading } = useProjects()
const { data: features } = useFeatures()
const { data: backlogs } = useBacklogs()
const { data: tasks, isLoading: isTasksLoading } = useTasks()

const recentProjects = computed(() => {
  return [...(projects.value || [])]
    .sort((a: any, b: any) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5)
})

const taskStatusSummary = computed(() => {
  const list = tasks.value || []
  const counts = list.reduce((acc: Record<string, number>, task: any) => {
    const status = task.status || 'Todo'
    acc[status] = (acc[status] || 0) + 1
    return acc
  }, {})

  return Object.entries(counts as Record<string, number>).map(([status, count]) => ({
    status,
    count,
    percent: list.length ? Math.round((count / list.length) * 100) : 0,
  }))
})

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 18px;
}

.stat-card {
  display: grid;
  gap: 8px;
  padding: 18px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
  box-shadow: 0 1px 2px rgba(9, 30, 66, 0.08);
}

.stat-card span {
  color: #5e6c84;
  font-size: 13px;
  font-weight: 700;
}

.stat-card strong {
  color: #172b4d;
  font-size: 30px;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(360px, 0.8fr);
  gap: 18px;
}

.panel {
  padding: 20px;
}

.section-title {
  margin-bottom: 16px;
}

.section-title p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.status-list {
  display: grid;
  gap: 14px;
}

.status-row {
  display: grid;
  grid-template-columns: 130px minmax(0, 1fr) 36px;
  align-items: center;
  gap: 12px;
  color: #44546f;
  font-weight: 700;
}

.status-meter {
  height: 8px;
  overflow: hidden;
  border-radius: 999px;
  background: #f1f2f4;
}

.status-meter span {
  display: block;
  height: 100%;
  border-radius: inherit;
  background: #0c66e4;
}

.project-list {
  display: grid;
  gap: 8px;
}

.project-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  color: #172b4d;
  text-decoration: none;
}

.project-row:hover {
  background: #f7f8f9;
}

.project-row strong,
.project-row small {
  display: block;
}

.project-row small {
  color: #5e6c84;
}

@media (max-width: 1100px) {
  .stats-grid,
  .dashboard-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 720px) {
  .stats-grid,
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
}
</style>
