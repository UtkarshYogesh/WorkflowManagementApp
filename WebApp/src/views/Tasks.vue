<template>
  <section class="page tasks">
    <div class="page-header">
      <h1>Tasks</h1>
      <p class="subtitle">Manage all tasks across backlogs.</p>
    </div>

    <div class="list-panel">
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="tasks?.length === 0" class="empty-state">
        No tasks yet. Create one in a backlog.
      </div>
      <div v-else class="cards-grid">
        <article v-for="task in tasks" :key="task.id" class="card">
          <div class="card-header">
            <h3>{{ task.title }}</h3>
            <span class="status-pill">{{ task.status }}</span>
          </div>
          <p>{{ task.description }}</p>
          <div class="card-actions">
            <router-link :to="`/tasks/${task.id}`">View details</router-link>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useTasks } from '../composables/useTasks'

const { data: tasks, isLoading: isTasksLoading } = useTasks()
</script>

<style scoped>
.page {
  padding: 28px 32px;
}
.page-header {
  margin-bottom: 28px;
}
h1 {
  color: #f8fafc;
  margin-bottom: 8px;
}
.subtitle {
  color: #cbd5e1;
  margin-top: 8px;
}
.list-panel {
  padding: 0;
}
.empty-state {
  padding: 28px;
  border-radius: 20px;
  background: #111827;
  color: #94a3b8;
  text-align: center;
}
.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}
.card {
  padding: 16px;
  border-radius: 16px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.16);
  transition: border-color 0.2s ease;
}
.card:hover {
  border-color: rgba(37, 99, 235, 0.3);
}
.card-header {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
  margin-bottom: 10px;
}
.card-header h3 {
  margin: 0;
  color: #f8fafc;
  font-size: 0.95rem;
  flex: 1;
}
.status-pill {
  font-size: 0.75rem;
  color: #cbd5e1;
  background: rgba(148, 163, 184, 0.14);
  border-radius: 999px;
  padding: 4px 10px;
  white-space: nowrap;
}
.card p {
  color: #cbd5e1;
  font-size: 0.9rem;
  margin: 8px 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.card-actions {
  display: flex;
  gap: 8px;
  margin-top: 12px;
}
.card-actions a {
  flex: 1;
  padding: 8px 12px;
  border-radius: 8px;
  background: rgba(37, 99, 235, 0.1);
  color: #2563eb;
  text-decoration: none;
  font-size: 0.85rem;
  text-align: center;
  transition: background 0.2s ease;
}
.card-actions a:hover {
  background: rgba(37, 99, 235, 0.2);
}
</style>
