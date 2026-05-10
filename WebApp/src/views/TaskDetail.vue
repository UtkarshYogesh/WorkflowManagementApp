<template>
  <section class="page task-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/tasks">Tasks</router-link>
          <span>/</span>
          <span>{{ task?.title || 'Task' }}</span>
        </p>
        <h1>{{ task?.title || 'Loading task...' }}</h1>
        <p class="subtitle">{{ task?.description || 'No description added yet.' }}</p>
      </div>
    </div>

    <div class="task-layout">
      <section class="detail-card">
        <h2>Task details</h2>
        <p>{{ task?.description || 'No description added yet.' }}</p>
      </section>

      <aside class="detail-card">
        <h2>Fields</h2>
        <div class="field-list">
          <span>Status <strong>{{ task?.status || 'Todo' }}</strong></span>
          <span>Created <strong>{{ task?.createdAt ? formatDate(task.createdAt) : '-' }}</strong></span>
          <label>
            Assignee
            <select :value="task?.assignedToUserId || ''" @change="assignTaskFromEvent($event)">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }} ({{ user.email }})
              </option>
            </select>
          </label>
        </div>
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useAssignTask, useTask } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'

const route = useRoute()
const taskId = String(route.params.taskId || '')

const { data: task } = useTask(taskId)
const { data: users } = useUsers()
const assignTaskMutation = useAssignTask()

const assignTaskFromEvent = async (event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignTaskMutation.mutateAsync({ taskId, userId })
}

const formatDate = (value: string) => new Date(value).toLocaleString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.task-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 340px;
  gap: 18px;
  align-items: start;
}

.detail-card h2 {
  margin: 0 0 14px;
}

.detail-card p {
  color: #44546f;
}

.field-list {
  display: grid;
  gap: 14px;
}

.field-list span,
.field-list label {
  display: grid;
  gap: 7px;
  color: #5e6c84;
  font-size: 13px;
  font-weight: 700;
}

.field-list strong {
  color: #172b4d;
}

@media (max-width: 900px) {
  .task-layout {
    grid-template-columns: 1fr;
  }
}
</style>
