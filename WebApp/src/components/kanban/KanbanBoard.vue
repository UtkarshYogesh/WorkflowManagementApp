<template>
  <div class="kanban-table">
    <!-- Table Header -->
    <div class="table-header">
      <div class="header-cell backlog-header">Backlog</div>
      <div class="header-cell" v-for="status in taskStatuses" :key="status">
        {{ status }}
      </div>
    </div>

    <!-- Table Rows -->
    <div class="table-row" v-for="backlog in props.backlogs" :key="backlog.id">
      <!-- Backlog Column -->
      <div class="table-cell backlog-cell">
        <div class="backlog-info">
          <h4
            v-if="props.onNavigateToBacklog"
            @click="props.onNavigateToBacklog(backlog.id)"
            class="clickable-title"
          >
            {{ backlog.title }}
          </h4>
          <h4 v-else>{{ backlog.title }}</h4>
          <span class="backlog-status">{{ backlog.status }}</span>
          <p class="backlog-description">{{ backlog.description }}</p>
        </div>
      </div>

      <!-- Status Columns -->
      <div class="table-cell tasks-cell" v-for="status in taskStatuses" :key="status">
        <div class="tasks-container">
          <div
            v-for="task in getTasksForBacklogAndStatus(backlog.id, status)"
            :key="task.id"
            class="task-card"
          >
            <div class="task-top">
              <h5
                v-if="props.onNavigateToTask"
                @click="props.onNavigateToTask(task.id)"
                class="clickable-title"
              >
                {{ task.title }}
              </h5>
              <h5 v-else>{{ task.title }}</h5>
            </div>
            <p>{{ task.description }}</p>
            <div class="task-actions">
              <button
                class="small"
                v-for="statusOption in getStatusOptions(task.status)"
                :key="statusOption"
                @click="$emit('change-status', { taskId: task.id, status: statusOption })"
              >
                {{ statusOption }}
              </button>
              <button class="small ghost" @click="$emit('delete-task', task.id)">Delete</button>
            </div>
          </div>
          <div
            v-if="getTasksForBacklogAndStatus(backlog.id, status).length === 0"
            class="empty-cell"
          >
            No tasks
          </div>
        </div>
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

const taskStatuses = computed(() => {
  const baseStatuses = props.statuses ? [...props.statuses] : ['Todo', 'In Progress', 'Done']
  const taskStatuses = Array.isArray(props.tasks) ? props.tasks.map((task) => task.status) : []
  const extraStatuses = [
    ...new Set(taskStatuses.filter((status) => status && !baseStatuses.includes(status))),
  ]
  return [...baseStatuses, ...extraStatuses]
})

const getStatusOptions = computed(() => {
  return (currentStatus: string) => taskStatuses.value.filter((status) => status !== currentStatus)
})

const groupedTasks = computed(() => {
  return Object.fromEntries(
    taskStatuses.value.map((status) => [
      status,
      props.tasks.filter((task) => task.status === status),
    ]),
  ) as Record<string, TaskItem[]>
})

const getTasksForBacklogAndStatus = (backlogId: string, status: string) => {
  return props.tasks.filter((task) => task.backlogItemId === backlogId && task.status === status)
}
</script>

<style scoped>
.kanban-table {
  display: table;
  width: 100%;
  border-collapse: collapse;
  background: #0f172a;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.table-header {
  display: table-row;
  background: rgba(148, 163, 184, 0.05);
  border-bottom: 1px solid rgba(148, 163, 184, 0.12);
}

.table-row {
  display: table-row;
  border-bottom: 1px solid rgba(148, 163, 184, 0.08);
}

.table-row:last-child {
  border-bottom: none;
}

.header-cell,
.table-cell {
  display: table-cell;
  padding: 16px;
  vertical-align: top;
  border-right: 1px solid rgba(148, 163, 184, 0.08);
}

.header-cell:last-child,
.table-cell:last-child {
  border-right: none;
}

.backlog-header {
  width: 250px;
  font-weight: 600;
  color: #f8fafc;
}

.header-cell {
  text-align: center;
  font-weight: 600;
  color: #cbd5e1;
  background: rgba(59, 130, 246, 0.08);
}

.backlog-cell {
  width: 250px;
  background: rgba(148, 163, 184, 0.02);
}

.tasks-cell {
  width: 200px;
  min-height: 120px;
}

.backlog-info h4 {
  margin: 0 0 8px 0;
  color: #f8fafc;
  font-size: 1rem;
  font-weight: 600;
}

.backlog-status {
  display: inline-block;
  padding: 4px 10px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 600;
  margin-bottom: 8px;
}

.backlog-description {
  color: #94a3b8;
  font-size: 0.9rem;
  margin: 0;
  line-height: 1.4;
}

.tasks-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-height: 80px;
}

.task-card {
  padding: 12px;
  border-radius: 8px;
  background: #1e293b;
  border: 1px solid rgba(148, 163, 184, 0.16);
}

.task-card h5 {
  margin: 0 0 6px 0;
  color: #f8fafc;
  font-size: 0.9rem;
  font-weight: 600;
}

.task-card p {
  margin: 0 0 10px 0;
  color: #cbd5e1;
  font-size: 0.8rem;
  line-height: 1.3;
}

.task-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}

button.small {
  border: none;
  border-radius: 999px;
  padding: 6px 10px;
  color: #f8fafc;
  background: #334155;
  cursor: pointer;
  font-size: 0.75rem;
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

.empty-cell {
  color: #64748b;
  font-size: 0.8rem;
  font-style: italic;
  text-align: center;
  padding: 20px;
}

.clickable-title {
  cursor: pointer;
  transition: color 0.2s ease;
}

.clickable-title:hover {
  color: #38bdf8;
}

/* Responsive adjustments */
@media (max-width: 1200px) {
  .backlog-cell,
  .backlog-header {
    width: 200px;
  }

  .tasks-cell {
    width: 150px;
  }
}

@media (max-width: 768px) {
  .kanban-table {
    font-size: 0.9rem;
  }

  .header-cell,
  .table-cell {
    padding: 12px 8px;
  }

  .backlog-cell,
  .backlog-header {
    width: 150px;
  }

  .tasks-cell {
    width: 120px;
  }
}
</style>
