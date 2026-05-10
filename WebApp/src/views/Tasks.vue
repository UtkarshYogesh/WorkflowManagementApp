<template>
  <section class="page tasks">
    <div class="page-header">
      <div>
        <p class="eyebrow">Work items</p>
        <h1>Tasks</h1>
        <p class="subtitle">A compact list of all executable tasks across backlog items.</p>
      </div>
    </div>

    <section class="panel">
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="!tasks?.length" class="empty-state">No tasks yet. Create one in a backlog.</div>
      <div v-else class="task-table">
        <div class="task-table-header">
          <span>Task</span>
          <span>Status</span>
          <span>Backlog</span>
          <span></span>
        </div>
        <article v-for="task in tasks" :key="task.id" class="task-row">
          <div class="task-name">
            <strong>{{ task.title }}</strong>
            <small>{{ task.description || 'No description' }}</small>
          </div>
          <span class="status-pill">{{ task.status }}</span>
          <span class="muted">{{ getBacklogTitle(task.backlogItemId) }}</span>
          <router-link :to="`/tasks/${task.id}`" class="button secondary">Open</router-link>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { useBacklogs } from '../composables/useBacklogs'
import { useTasks } from '../composables/useTasks'

const { data: tasks, isLoading: isTasksLoading } = useTasks()
const { data: backlogs } = useBacklogs()

const getBacklogTitle = (backlogId?: string) => {
  if (!backlogId) return 'No backlog'
  return backlogs.value?.find((backlog: any) => backlog.id === backlogId)?.title || 'Backlog item'
}
</script>

<style scoped>
.panel {
  overflow-x: auto;
}

.task-table {
  min-width: 760px;
  overflow: hidden;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.task-table-header,
.task-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 120px 220px 90px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.task-table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.task-row:last-child {
  border-bottom: 0;
}

.task-row:hover {
  background: #f7f8f9;
}

.task-row .button {
  justify-self: end;
}

.task-name {
  min-width: 0;
}

.task-name strong,
.task-name small {
  display: block;
}

.task-name small,
.muted {
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.status-pill {
  justify-self: start;
}

@media (max-width: 900px) {
  .panel {
    overflow-x: visible;
  }

  .task-table {
    min-width: 0;
  }

  .task-table-header,
  .task-row {
    grid-template-columns: 1fr;
  }

  .task-row .button {
    justify-self: start;
  }
}
</style>
