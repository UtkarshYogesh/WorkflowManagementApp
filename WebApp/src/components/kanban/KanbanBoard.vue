<template>
  <div class="kanban-board">
    <div class="kanban-column" v-for="status in statuses" :key="status">
      <h3>{{ status }}</h3>
      <div class="column-body">
        <article v-for="task in grouped[status]" :key="task.id" class="task-card">
          <div class="task-top">
            <h4>{{ task.title }}</h4>
            <span class="task-status">{{ task.status }}</span>
          </div>
          <p>{{ task.description }}</p>
          <div class="task-actions">
            <button
              class="small"
              v-for="s in statuses"
              :key="s"
              v-if="task.status !== s"
              @click="$emit('change-status', { taskId: task.id, status: s })"
            >
              {{ s }}
            </button>
            <button class="small ghost" @click="$emit('delete-task', task.id)">Delete</button>
          </div>
        </article>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

type TaskItem = {
  id: string
  title: string
  description: string
  status: string
}

const props = defineProps<{ tasks: TaskItem[]; statuses?: string[] }>()
const defaultStatuses = ['Todo', 'In Progress', 'Done'] as const
const statuses = computed(() => props.statuses || defaultStatuses)

const grouped = computed(() => {
  return Object.fromEntries(
    statuses.map((status) => [status, props.tasks.filter((task) => task.status === status)]),
  ) as Record<string, TaskItem[]>
})
</script>

<style scoped>
.kanban-board {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 18px;
}
.kanban-column {
  background: rgba(148, 163, 184, 0.07);
  border-radius: 18px;
  padding: 18px;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.column-body {
  display: grid;
  gap: 14px;
  margin-top: 14px;
}
.task-card {
  padding: 16px;
  border-radius: 16px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.16);
}
.task-top {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
  margin-bottom: 10px;
}
.task-status {
  font-size: 0.8rem;
  color: #cbd5e1;
  background: rgba(148, 163, 184, 0.14);
  border-radius: 999px;
  padding: 4px 10px;
}
.task-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  margin-top: 14px;
}
button.small {
  border: none;
  border-radius: 999px;
  padding: 8px 12px;
  color: #f8fafc;
  background: #334155;
  cursor: pointer;
}
button.small.success {
  background: #16a34a;
}
button.small.ghost {
  background: transparent;
  color: #f8fafc;
  border: 1px solid rgba(148, 163, 184, 0.32);
}
button.small:hover {
  opacity: 0.92;
}
</style>
