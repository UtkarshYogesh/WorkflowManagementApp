<template>
  <section class="page backlogs">
    <div class="page-header">
      <div>
        <p class="eyebrow">Execution</p>
        <h1>Backlog</h1>
        <p class="subtitle">Review backlog items and the tasks attached to them.</p>
      </div>
      <div class="page-actions">
        <button class="button primary" @click="showCreateForm = !showCreateForm">
          {{ showCreateForm ? 'Close form' : 'Add backlog' }}
        </button>
        <button class="button secondary" @click="viewMode = viewMode === 'board' ? 'list' : 'board'">
          {{ viewMode === 'board' ? 'List view' : 'Board view' }}
        </button>
      </div>
    </div>

    <div class="scope-panel">
      <label>
        Project
        <select v-model="selectedProjectId">
          <option value="" disabled>Select project</option>
          <option v-for="project in projects" :key="project.projectId" :value="project.projectId">
            {{ project.name }}
          </option>
        </select>
      </label>
    </div>

    <div class="backlog-workspace" :class="{ 'form-open': showCreateForm }">
      <aside v-if="showCreateForm" class="form-card">
        <h2>Create backlog</h2>
        <label>
          Feature
          <select v-model="selectedFeatureId">
            <option value="" disabled>Select feature</option>
            <option v-for="feature in filteredFeatures" :key="feature.id" :value="feature.id">
              {{ feature.name }}
            </option>
          </select>
        </label>
        <label>
          Title
          <input v-model="title" placeholder="Backlog title" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Backlog description"></textarea>
        </label>
        <label>
          Assignee
          <select v-model="assignedToUserId">
            <option value="">Unassigned</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }} ({{ user.email }})
            </option>
          </select>
        </label>
        <div class="form-actions">
          <button class="button primary" :disabled="!selectedFeatureId || !title" @click="submitBacklog">
            Create backlog
          </button>
          <button class="button ghost" @click="showCreateForm = false">Cancel</button>
        </div>
      </aside>

      <div class="backlog-content">
        <div v-if="viewMode === 'list'" class="panel">
          <div v-if="isProjectsLoading || isFeaturesLoading || isBacklogsLoading" class="empty-state">Loading backlog...</div>
          <div v-else-if="!projects?.length" class="empty-state">No projects found. Create a project first.</div>
          <div v-else-if="!selectedProjectId" class="empty-state">Select a project to see its backlog.</div>
          <div v-else-if="!filteredFeatures.length" class="empty-state">No features found for this project.</div>
          <div v-else-if="!filteredBacklogs.length" class="empty-state">No backlog items yet for this project.</div>
          <div v-else class="backlog-list">
            <article v-for="backlog in filteredBacklogs" :key="backlog.id" class="backlog-row">
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
              <button v-if="ability.can('delete', asSubject('Backlog', backlog))" class="button ghost delete-button" @click="deleteBacklog(backlog.id)">
                Delete
              </button>

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
          <div v-if="isProjectsLoading || isFeaturesLoading || isBacklogsLoading || isTasksLoading" class="empty-state">Loading board...</div>
          <div v-else-if="!projects?.length" class="empty-state">No projects found. Create a project first.</div>
          <div v-else-if="!selectedProjectId" class="empty-state">Select a project to see its backlog board.</div>
          <div v-else-if="!filteredFeatures.length" class="empty-state">No features found for this project.</div>
          <div v-else-if="!filteredBacklogs.length" class="empty-state">No backlog items yet for this project.</div>
          <KanbanBoard
            v-else
            :backlogs="filteredBacklogs"
            :tasks="filteredTasks"
            :statuses="['Todo', 'In Progress', 'Done']"
            :on-navigate-to-backlog="navigateToBacklog"
            :on-navigate-to-task="navigateToTask"
            :can-delete-task="canDeleteTask"
            @change-status="changeTaskStatus"
            @delete-task="deleteTask"
          />
        </div>
            </div>
          </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useProjects } from '../composables/useProjects'
import { useFeatures } from '../composables/useFeatures'
import { useBacklogs, useCreateBacklog, useDeleteBacklog } from '../composables/useBacklogs'
import { useTasks, useUpdateTaskStatus, useDeleteTask } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import KanbanBoard from '../components/kanban/KanbanBoard.vue'

const router = useRouter()
const { data: projects, isLoading: isProjectsLoading } = useProjects()
const { data: features, isLoading: isFeaturesLoading } = useFeatures()
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs()
const { data: allTasks, isLoading: isTasksLoading } = useTasks()
const { data: users } = useUsers()
const createBacklogMutation = useCreateBacklog()
const deleteBacklogMutation = useDeleteBacklog()
const updateTaskStatusMutation = useUpdateTaskStatus()
const deleteTaskMutation = useDeleteTask()
const ability = useAppAbility()

const viewMode = ref<'list' | 'board'>('list')
const expandedBacklogs = ref<string[]>([])
const selectedProjectId = ref('')
const selectedFeatureId = ref('')
const showCreateForm = ref(false)
const title = ref('')
const description = ref('')
const assignedToUserId = ref('')

watch(
  projects,
  (items) => {
    if (!selectedProjectId.value && items?.length) {
      selectedProjectId.value = items[0].projectId
    }
  },
  { immediate: true },
)

const filteredFeatures = computed(() => {
  if (!selectedProjectId.value) return []
  return features.value?.filter((feature: any) => feature.projectId === selectedProjectId.value) || []
})

watch(
  filteredFeatures,
  (items) => {
    if (!items.some((feature: any) => feature.id === selectedFeatureId.value)) {
      selectedFeatureId.value = items[0]?.id || ''
    }
  },
  { immediate: true },
)

const selectedFeatureIds = computed(() => {
  return filteredFeatures.value.map((feature: any) => feature.id)
})

const filteredBacklogs = computed(() => {
  const featureIds = new Set(selectedFeatureIds.value)
  return backlogs.value?.filter((backlog: any) => featureIds.has(backlog.featureId)) || []
})

const filteredTasks = computed(() => {
  const backlogIds = new Set(filteredBacklogs.value.map((backlog: any) => backlog.id))
  return allTasks.value?.filter((task: any) => backlogIds.has(task.backlogItemId)) || []
})

const toggleBacklog = (backlogId: string) => {
  const index = expandedBacklogs.value.indexOf(backlogId)
  if (index > -1) {
    expandedBacklogs.value.splice(index, 1)
  } else {
    expandedBacklogs.value.push(backlogId)
  }
}

const getTasksForBacklog = (backlogId: string) => {
  return filteredTasks.value.filter((task: any) => task.backlogItemId === backlogId)
}

const canDeleteTask = (task: any) => ability.can('delete', asSubject('Task', task))

const submitBacklog = async () => {
  if (!selectedFeatureId.value || !title.value) return
  await createBacklogMutation.mutateAsync({
    featureId: selectedFeatureId.value,
    data: {
      title: title.value,
      description: description.value,
      assignedToUserId: assignedToUserId.value || null,
    },
  })
  title.value = ''
  description.value = ''
  assignedToUserId.value = ''
  showCreateForm.value = false
}

const deleteBacklog = async (backlogId: string) => {
  await deleteBacklogMutation.mutateAsync(backlogId)
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
.scope-panel {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 14px;
  padding: 14px 18px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.scope-panel label {
  display: grid;
  gap: 7px;
  width: min(360px, 100%);
  color: #44546f;
  font-size: 13px;
  font-weight: 700;
}

.backlog-workspace {
  display: grid;
  grid-template-columns: minmax(0, 1fr);
  gap: 18px;
  align-items: start;
}

.backlog-workspace.form-open {
  grid-template-columns: 340px minmax(0, 1fr);
}

.backlog-content {
  min-width: 0;
}

.form-card textarea {
  min-height: 110px;
}

.form-actions {
  display: flex;
  gap: 8px;
}

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
  grid-template-columns: minmax(0, 1fr) max-content 110px max-content;
  gap: 14px;
  align-items: center;
  padding: 14px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.delete-button {
  justify-self: end;
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
  .scope-panel {
    justify-content: stretch;
  }

  .backlog-workspace {
    grid-template-columns: 1fr;
  }

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
