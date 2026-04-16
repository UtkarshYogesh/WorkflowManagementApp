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
        <p class="subtitle">Track tasks in a kanban board and update status quickly.</p>
      </div>
      <div class="page-actions">
        <button class="button" @click="showCreateForm = !showCreateForm">Create Task</button>
        <button
          class="button secondary"
          @click="viewMode = viewMode === 'kanban' ? 'list' : 'kanban'"
        >
          {{ viewMode === 'kanban' ? 'List View' : 'Kanban View' }}
        </button>
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

    <div v-if="viewMode === 'kanban'" class="kanban-section">
      <h2>Task board</h2>
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="!tasks?.length" class="empty-state">
        No tasks yet. Create your first task to see the kanban board.
      </div>
      <KanbanBoard v-else :tasks="tasks" @change-status="changeStatus" @delete-task="deleteTask" />
    </div>

    <div v-else class="list-panel">
      <h2>Task List</h2>
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="!tasks?.length" class="empty-state">
        No tasks yet. Create your first task.
      </div>
      <div v-else>
        <article v-for="task in tasks" :key="task.id" class="list-item">
          <h4>{{ task.title }}</h4>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import KanbanBoard from '../components/kanban/KanbanBoard.vue'
import { useBacklog } from '../composables/useBacklogs'
import {
  useTasks,
  useCreateTask,
  useDeleteTask,
  useUpdateTaskStatus,
} from '../composables/useTasks'

const route = useRoute()
const backlogId = String(route.params.backlogId || '')

const { data: backlog } = useBacklog(backlogId)
const { data: tasks, isLoading: isTasksLoading } = useTasks(backlogId)
const createTaskMutation = useCreateTask()
const deleteTaskMutation = useDeleteTask()
const updateTaskStatusMutation = useUpdateTaskStatus()

const title = ref('')
const description = ref('')
const showCreateForm = ref(false)
const viewMode = ref<'kanban' | 'list'>('kanban')

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

const changeStatus = async ({ taskId, status }: { taskId: string; status: string }) => {
  await updateTaskStatusMutation.mutateAsync({ taskId, status })
}

const deleteTask = async (taskId: string) => {
  await deleteTaskMutation.mutateAsync(taskId)
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
.kanban-section {
  margin-top: 12px;
}
.empty-state {
  padding: 28px;
  border-radius: 20px;
  background: #111827;
  color: #94a3b8;
}
.detail-meta {
  margin-top: 12px;
  color: #cbd5e1;
}
</style>
