<template>
  <div class="kanban-board">
    <section v-for="status in taskStatuses" :key="status" class="kanban-column">
      <header>
        <h3>{{ status }}</h3>
        <span>{{ getTasksByStatus(status).length }}</span>
      </header>

      <article
        v-for="task in getTasksByStatus(status)"
        :key="task.id"
        class="task-card"
        @click="props.onNavigateToTask?.(task.id)"
      >
        <strong>{{ task.title }}</strong>
        <p>{{ task.description || 'No description' }}</p>
        <small>{{ getBacklogTitle(task.backlogItemId) }}</small>
        <div class="task-actions" @click.stop>
          <button
            v-for="statusOption in getStatusOptions(task.status)"
            :key="statusOption"
            class="small"
            @click="$emit('change-status', { taskId: task.id, status: statusOption })"
          >
            {{ statusOption }}
          </button>
          <button class="small ghost" @click="$emit('delete-task', task.id)">Delete</button>
        </div>
      </article>

      <div v-if="getTasksByStatus(status).length === 0" class="empty-column">No tasks</div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

type TaskItem = {
  id: string
  title: string
  description: string
  status: string
  backlogItemId?: string
}

type BacklogItem = {
  id: string
  title: string
  description: string
  status: string
}

const props = defineProps<{
  backlogs?: BacklogItem[]
  tasks: TaskItem[]
  statuses?: string[]
  onNavigateToBacklog?: (backlogId: string) => void
  onNavigateToTask?: (taskId: string) => void
}>()

defineEmits<{
  (event: 'change-status', payload: { taskId: string; status: string }): void
  (event: 'delete-task', taskId: string): void
}>()

const taskStatuses = computed(() => {
  const baseStatuses = props.statuses ? [...props.statuses] : ['Todo', 'In Progress', 'Done']
  const statuses = Array.isArray(props.tasks) ? props.tasks.map((task) => task.status) : []
  return [...baseStatuses, ...new Set(statuses.filter((status) => status && !baseStatuses.includes(status)))]
})

const getStatusOptions = (currentStatus: string) => {
  return taskStatuses.value.filter((status) => status !== currentStatus)
}

const getTasksByStatus = (status: string) => {
  return props.tasks.filter((task) => task.status === status)
}

const getBacklogTitle = (backlogId?: string) => {
  if (!backlogId) return 'No backlog'
  return props.backlogs?.find((backlog) => backlog.id === backlogId)?.title || 'Backlog item'
}
</script>

<style scoped>
.kanban-board {
  display: flex;
  gap: 14px;
  overflow-x: auto;
  padding-bottom: 8px;
}

.kanban-column {
  flex: 0 0 300px;
  min-height: 460px;
  min-width: 0;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #f7f8f9;
}

.kanban-column header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.kanban-column h3 {
  margin: 0;
  color: #172b4d;
  font-size: 14px;
}

.kanban-column header span {
  color: #5e6c84;
  font-size: 13px;
  font-weight: 800;
}

.task-card {
  display: grid;
  gap: 7px;
  margin-bottom: 10px;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
  box-shadow: 0 1px 2px rgba(9, 30, 66, 0.08);
  cursor: pointer;
  min-width: 0;
}

.task-card:hover {
  border-color: #0c66e4;
}

.task-card strong {
  color: #172b4d;
}

.task-card p,
.task-card small {
  margin: 0;
  color: #5e6c84;
  overflow-wrap: anywhere;
}

.task-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 4px;
}

.small {
  min-height: 28px;
  max-width: 100%;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #f1f2f4;
  color: #172b4d;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
}

.small:hover {
  background: #dfe1e6;
}

.small.ghost {
  color: #ae2e24;
}

.empty-column {
  padding: 16px;
  border: 1px dashed #c1c7d0;
  border-radius: 8px;
  color: #5e6c84;
  text-align: center;
}

@media (max-width: 900px) {
  .kanban-column {
    flex-basis: 280px;
  }
}
</style>
