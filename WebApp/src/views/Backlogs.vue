<template>
  <section class="page backlogs">
    <div class="page-header">
      <div>
        <p class="eyebrow">Execution</p>
        <h1>Backlog</h1>
        <p class="subtitle">Review backlog items and the tasks attached to them.</p>
      </div>
      <div class="page-actions">
        <button class="button secondary" @click="viewMode = viewMode === 'board' ? 'list' : 'board'">
          {{ viewMode === 'board' ? 'List view' : 'Board view' }}
        </button>
      </div>
    </div>

    <div v-if="viewMode === 'list'" class="panel">
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlog...</div>
      <div v-else-if="!backlogs?.length" class="empty-state">No backlog items yet. Create one in a feature.</div>
      <div v-else class="backlog-list">
        <article v-for="backlog in backlogs" :key="backlog.id" class="backlog-row">
          <div class="backlog-main">
            <button class="toggle-button" @click="toggleBacklog(backlog.id)">
              {{ expandedBacklogs.includes(backlog.id) ? '-' : '+' }}
            </button>
            <div>
              <h3 @click="navigateToBacklog(backlog.id)" class="clickable-title">{{ backlog.title }}</h3>
              <p>{{ backlog.description || 'No description' }}</p>
            </div>
          </div>
          <span class="status-pill">{{ backlog.status }}</span>
          <span class="muted">{{ getTasksForBacklog(backlog.id).length }} tasks</span>

          <div v-if="expandedBacklogs.includes(backlog.id)" class="nested-list">
            <div v-if="getTasksForBacklog(backlog.id).length === 0" class="nested-empty">No tasks yet.</div>
            <button
              v-for="task in getTasksForBacklog(backlog.id)"
              :key="task.id"
              class="nested-row"
              @click="navigateToTask(task.id)"
            >
              <span>{{ task.title }}</span>
              <span class="status-pill">{{ task.status }}</span>
            </button>
          </div>
        </article>
      </div>
    </div>

    <div v-else class="board-panel">
      <div v-if="isBacklogsLoading || isTasksLoading" class="empty-state">Loading board...</div>
      <div v-else-if="!backlogs?.length" class="empty-state">No backlog items yet. Create one in a feature.</div>
      <KanbanBoard
        v-else
        :backlogs="backlogs"
        :tasks="allTasks || []"
        :statuses="['Todo', 'In Progress', 'Done']"
        :on-navigate-to-backlog="navigateToBacklog"
        :on-navigate-to-task="navigateToTask"
        @change-status="changeTaskStatus"
        @delete-task="deleteTask"
      />
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useBacklogs } from '../composables/useBacklogs'
import { useTasks, useUpdateTaskStatus, useDeleteTask } from '../composables/useTasks'
import KanbanBoard from '../components/kanban/KanbanBoard.vue'

const router = useRouter()
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs()
const { data: allTasks, isLoading: isTasksLoading } = useTasks()
const updateTaskStatusMutation = useUpdateTaskStatus()
const deleteTaskMutation = useDeleteTask()

const viewMode = ref<'list' | 'board'>('list')
const expandedBacklogs = ref<string[]>([])

const toggleBacklog = (backlogId: string) => {
  const index = expandedBacklogs.value.indexOf(backlogId)
  if (index > -1) {
    expandedBacklogs.value.splice(index, 1)
  } else {
    expandedBacklogs.value.push(backlogId)
  }
}

const getTasksForBacklog = (backlogId: string) => {
  return allTasks.value?.filter((task: any) => task.backlogItemId === backlogId) || []
}

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
}

const navigateToTask = (taskId: string) => {
  router.push(`/tasks/${taskId}`)
}

const changeTaskStatus = async ({ taskId, status }: { taskId: string; status: string }) => {
  await updateTaskStatusMutation.mutateAsync({ taskId, status })
}

const deleteTask = async (taskId: string) => {
  await deleteTaskMutation.mutateAsync(taskId)
}
</script>

<style scoped>
.panel,
.board-panel {
  padding: 18px;
  overflow: hidden;
}

.board-panel {
  overflow-x: auto;
}

.backlog-list {
  display: grid;
  gap: 10px;
}

.backlog-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) max-content 110px;
  gap: 14px;
  align-items: center;
  padding: 14px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.backlog-main {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  min-width: 0;
}

.backlog-main > div {
  min-width: 0;
}

.backlog-main h3 {
  margin: 0;
  font-size: 15px;
}

.backlog-main p {
  margin: 4px 0 0;
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.toggle-button {
  width: 28px;
  height: 28px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #f7f8f9;
  color: #44546f;
  cursor: pointer;
}

.muted {
  color: #5e6c84;
  font-size: 13px;
  justify-self: end;
  white-space: nowrap;
}

.nested-list {
  grid-column: 1 / -1;
  display: grid;
  gap: 6px;
  padding: 10px 0 0 40px;
}

.nested-row {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
  min-height: 36px;
  padding: 8px 10px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #f7f8f9;
  color: #172b4d;
  text-align: left;
  cursor: pointer;
  min-width: 0;
}

.nested-row span:first-child {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.nested-empty {
  color: #5e6c84;
  font-size: 13px;
}

@media (max-width: 900px) {
  .backlog-row {
    grid-template-columns: 1fr;
  }

  .muted {
    justify-self: start;
  }

  .nested-list {
    padding-left: 0;
  }
}
</style>
