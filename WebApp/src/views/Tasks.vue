<template>
  <section class="page tasks">
    <div class="page-header">
      <div>
        <p class="eyebrow">Work items</p>
        <h1>Tasks</h1>
        <p class="subtitle">A compact list of all executable tasks across backlog items.</p>
      </div>
    </div>

    <div class="command-bar">
      <span class="command-title">{{ tasks?.length ?? 0 }} tasks</span>
      <router-link class="button secondary" to="/backlogs">Open backlog board</router-link>
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
@reference "../style.css";

.panel {
  @apply overflow-x-auto p-3 max-[900px]:overflow-x-visible;
}

.task-table {
  @apply min-w-190 overflow-hidden rounded-md border border-slate-300 max-[900px]:min-w-0;
}

.command-title {
  @apply text-sm font-bold text-slate-800;
}

.task-table-header,
.task-row {
  @apply grid grid-cols-[minmax(0,1fr)_120px_220px_80px] items-center gap-3 border-b border-slate-300 px-3 py-2.5 max-[900px]:grid-cols-1;
}

.task-table-header {
  @apply bg-slate-50 text-xs font-extrabold uppercase text-slate-600;
}

.task-row:last-child {
  @apply border-b-0;
}

.task-row:hover {
  @apply bg-slate-50;
}

.task-row .button {
  @apply justify-self-end max-[900px]:justify-self-start;
}

.task-name {
  @apply min-w-0;
}

.task-name strong,
.task-name small {
  @apply block;
}

.task-name small,
.muted {
  @apply truncate text-slate-600;
}

.status-pill {
  @apply justify-self-start;
}
</style>
