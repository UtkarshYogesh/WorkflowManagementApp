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
        <p class="subtitle">Task details and management.</p>
      </div>
    </div>

    <div class="detail-panel">
      <div class="detail-card">
        <h2>Task Info</h2>
        <p>{{ task?.description || 'No description added yet.' }}</p>
        <p class="detail-meta">
          <strong>Status:</strong> <span>{{ task?.status || 'Todo' }}</span>
        </p>
        <label class="assignment-field">
          Assignee
          <select :value="task?.assignedToUserId || ''" @change="assignTaskFromEvent($event)">
            <option value="" disabled>Assign user</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }} ({{ user.email }})
            </option>
          </select>
        </label>
        <p class="detail-meta">
          <strong>Created:</strong>
          {{ task?.createdAt ? new Date(task.createdAt).toLocaleString() : '-' }}
        </p>
      </div>
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
</script>

<style scoped>
.assignment-field {
  display: grid;
  gap: 8px;
  max-width: 360px;
  margin: 18px 0;
  color: #cbd5e1;
}

.assignment-field select {
  min-height: 40px;
  border-radius: 10px;
  border: 1px solid #334155;
  background: #0f172a;
  color: #f8fafc;
  padding: 0 10px;
}
</style>
