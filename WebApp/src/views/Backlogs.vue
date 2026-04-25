<template>
  <section class="page backlogs">
    <div class="page-header">
      <div>
        <h1>Backlogs</h1>
        <p class="subtitle">Manage all backlog items across features.</p>
      </div>
      <div class="page-actions">
        <button
          class="button secondary"
          @click="viewMode = viewMode === 'kanban' ? 'list' : 'kanban'"
        >
          {{ viewMode === 'kanban' ? 'List View' : 'Kanban View' }}
        </button>
      </div>
    </div>

    <div v-if="viewMode === 'list'" class="list-panel">
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlogs...</div>
      <div v-else-if="backlogs?.length === 0" class="empty-state">
        No backlogs yet. Create one in a feature.
      </div>
      <div v-else class="backlogs-list">
        <article v-for="backlog in backlogs" :key="backlog.id" class="backlog-item">
          <div class="backlog-header" @click="toggleBacklog(backlog.id)">
            <div class="backlog-info">
              <h3 @click.stop="navigateToBacklog(backlog.id)" class="clickable-title">
                {{ backlog.title }}
              </h3>
              <span class="status-pill">{{ backlog.status }}</span>
            </div>
            <div class="dropdown-icon">
              <span :class="{ rotated: expandedBacklogs.includes(backlog.id) }">▼</span>
            </div>
          </div>
          <p class="backlog-description">{{ backlog.description }}</p>

          <div v-if="expandedBacklogs.includes(backlog.id)" class="tasks-section">
            <div v-if="getTasksForBacklog(backlog.id).length === 0" class="no-tasks">
              No tasks yet.
            </div>
            <div v-else class="tasks-list">
              <div v-for="task in getTasksForBacklog(backlog.id)" :key="task.id" class="task-item">
                <span @click="navigateToTask(task.id)" class="task-title clickable-title">{{
                  task.title
                }}</span>
                <span class="task-status">{{ task.status }}</span>
              </div>
            </div>
          </div>
        </article>
      </div>
    </div>

    <div v-else class="kanban-section">
      <h2>Backlog Kanban</h2>
      <div v-if="isBacklogsLoading || isTasksLoading" class="empty-state">Loading...</div>
      <div v-else-if="!backlogs?.length" class="empty-state">
        No backlogs yet. Create one in a feature.
      </div>
      <KanbanBoard
        v-else
        :backlogs="backlogs"
        :tasks="allTasks"
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
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useBacklogs } from '../composables/useBacklogs'
import { useTasks, useUpdateTaskStatus, useDeleteTask } from '../composables/useTasks'
import KanbanBoard from '../components/kanban/KanbanBoard.vue'

const router = useRouter()
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs()
const { data: allTasks, isLoading: isTasksLoading } = useTasks()
const updateTaskStatusMutation = useUpdateTaskStatus()
const deleteTaskMutation = useDeleteTask()

const viewMode = ref<'list' | 'kanban'>('list')
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
.backlogs-list {
  display: grid;
  gap: 16px;
}

.backlog-item {
  padding: 20px;
  border-radius: 18px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.12);
  box-shadow: 0 20px 40px rgba(15, 23, 42, 0.08);
}

.backlog-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  padding: 12px 0;
}

.backlog-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.backlog-info h3 {
  margin: 0;
  color: #f8fafc;
}

.backlog-description {
  color: #cbd5e1;
  margin: 8px 0 16px 0;
}

.dropdown-icon {
  transition: transform 0.2s ease;
}

.dropdown-icon .rotated {
  transform: rotate(180deg);
}

.tasks-section {
  border-top: 1px solid rgba(148, 163, 184, 0.12);
  padding-top: 16px;
}

.no-tasks {
  color: #94a3b8;
  font-style: italic;
}

.tasks-list {
  display: grid;
  gap: 8px;
}

.task-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: rgba(148, 163, 184, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(148, 163, 184, 0.08);
}

.task-title {
  color: #f8fafc;
  font-weight: 500;
}

.task-status {
  color: #bfdbfe;
  background: rgba(59, 130, 246, 0.14);
  padding: 4px 8px;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 600;
}

.kanban-section {
  margin-top: 12px;
}

.kanban-section h2 {
  color: #f8fafc;
  margin-bottom: 20px;
}

.clickable-title {
  cursor: pointer;
  transition: color 0.2s ease;
}

.clickable-title:hover {
  color: #38bdf8;
}
</style>
