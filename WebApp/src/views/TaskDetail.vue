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
import { useTask } from '../composables/useTasks'

const route = useRoute()
const taskId = String(route.params.taskId || '')

const { data: task } = useTask(taskId)
</script>
