<template>
  <section class="page backlog-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/backlogs">Backlogs</router-link>
          <span>/</span>
          <span>{{ backlog?.title || 'Backlog item' }}</span>
        </p>
        <h1>{{ backlog?.title || 'Loading backlog...' }}</h1>
        <p class="subtitle">Manage tasks for this backlog item.</p>
      </div>
      <div class="page-actions">
        <button class="button" @click="showCreateForm = !showCreateForm">Create Task</button>
      </div>
    </div>

    <div v-if="showCreateForm" class="form-panel">
      <div class="form-card">
        <h2>Create a task</h2>
        <label>
          Title
          <input v-model="title" placeholder="Task title" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Task details"></textarea>
        </label>
        <button class="button" :disabled="!title" @click="submitTask">Add task</button>
        <button class="button ghost" @click="showCreateForm = false">Cancel</button>
      </div>
    </div>

    <div class="tasks-section">
      <h2>Tasks</h2>
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="!tasks?.length" class="empty-state">
        No tasks yet. Create your first task.
      </div>
      <div v-else class="tasks-table">
        <div class="table-header">
          <div class="header-cell name-header">Name</div>
          <div class="header-cell status-header">Status</div>
        </div>
        <div
          v-for="task in tasks"
          :key="task.id"
          class="table-row"
          @click="navigateToTask(task.id)"
        >
          <div class="table-cell name-cell">{{ task.title }}</div>
          <div class="table-cell status-cell">
            <span class="status-pill">{{ task.status }}</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBacklog } from '../composables/useBacklogs'
import { useTasks, useCreateTask } from '../composables/useTasks'

const route = useRoute()
const router = useRouter()
const backlogId = String(route.params.backlogId || '')

const { data: backlog } = useBacklog(backlogId)
const { data: tasks, isLoading: isTasksLoading } = useTasks(backlogId)
const createTaskMutation = useCreateTask()

const title = ref('')
const description = ref('')
const showCreateForm = ref(false)

const submitTask = async () => {
  if (!title.value) return
  await createTaskMutation.mutateAsync({
    backlogId,
    data: { title: title.value, description: description.value },
  })
  title.value = ''
  description.value = ''
  showCreateForm.value = false
}

const navigateToTask = (taskId: string) => {
  router.push(`/tasks/${taskId}`)
}
</script>

<style scoped>
.page {
  padding: 28px 32px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  align-items: flex-start;
  margin-bottom: 28px;
}
.breadcrumb {
  color: #94a3b8;
  font-size: 0.95rem;
  margin-bottom: 10px;
}
.breadcrumb span {
  margin: 0 8px;
}
.detail-panel {
  display: grid;
  gap: 20px;
  grid-template-columns: 1.2fr 0.9fr;
  margin-bottom: 26px;
}
.detail-card {
  padding: 22px;
  border-radius: 18px;
  background: #111827;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.form-card input,
.form-card textarea {
  width: 100%;
  margin-top: 8px;
  border-radius: 12px;
  border: 1px solid #334155;
  background: #0f172a;
  color: #f8fafc;
  padding: 10px 12px;
}
.form-card textarea {
  min-height: 120px;
}
.tasks-section {
  margin-top: 12px;
}
.tasks-section h2 {
  color: #f8fafc;
  margin-bottom: 20px;
}
.tasks-table {
  background: #0f172a;
  border-radius: 12px;
  border: 1px solid rgba(148, 163, 184, 0.12);
  overflow: hidden;
}
.table-header {
  display: flex;
  background: rgba(148, 163, 184, 0.05);
  border-bottom: 1px solid rgba(148, 163, 184, 0.12);
}
.header-cell {
  padding: 16px;
  font-weight: 600;
  color: #cbd5e1;
  flex: 1;
}
.name-header {
  flex: 2;
}
.status-header {
  flex: 1;
}
.table-row {
  display: flex;
  border-bottom: 1px solid rgba(148, 163, 184, 0.08);
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.table-row:hover {
  background: rgba(148, 163, 184, 0.03);
}
.table-row:last-child {
  border-bottom: none;
}
.table-cell {
  padding: 16px;
  display: flex;
  align-items: center;
  flex: 1;
}
.name-cell {
  flex: 2;
  color: #f8fafc;
  font-weight: 500;
}
.status-cell {
  flex: 1;
}
.status-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 6px 14px;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  font-size: 0.85rem;
  font-weight: 600;
}
.empty-state {
  padding: 28px;
  border-radius: 20px;
  background: #111827;
  color: #94a3b8;
}
</style>
