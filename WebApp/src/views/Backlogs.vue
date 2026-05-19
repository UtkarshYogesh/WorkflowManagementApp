<template>
  <section class="page backlogs">
    <div class="page-header">
      <div>
        <p class="eyebrow">Execution</p>
        <h1>Backlog</h1>
        <p class="subtitle">Review backlog items and the tasks attached to them.</p>
      </div>
    </div>

    <div class="command-bar">
      <label>
        Project
        <select v-model="selectedProjectId">
          <option value="" disabled>Select project</option>
          <option v-for="project in projects" :key="project.projectId" :value="project.projectId">
            {{ project.name }}
          </option>
        </select>
      </label>
      <span class="command-divider"></span>
      <button class="button primary" @click="showCreateForm = !showCreateForm">
        {{ showCreateForm ? 'Close form' : 'Add backlog' }}
      </button>
      <div class="segmented-control" aria-label="View mode">
        <button :class="{ active: viewMode === 'list' }" @click="viewMode = 'list'">List</button>
        <button :class="{ active: viewMode === 'board' }" @click="viewMode = 'board'">Board</button>
      </div>
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
        <label>
          Priority
          <select v-model="priority">
            <option v-for="option in backlogPriorities" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
        </label>
        <label>
          Type
          <select v-model="type">
            <option v-for="option in backlogTypes" :key="option" :value="option">
              {{ option }}
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
              <span class="priority-pill" :class="`priority-${getBacklogPriority(backlog).toLowerCase()}`">
                {{ getBacklogPriority(backlog) }}
              </span>
              <span class="type-pill">{{ getBacklogType(backlog) }}</span>
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
            :statuses="TASK_STATUSES"
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
import { TASK_STATUSES } from '../constants/statuses'

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
const priority = ref('P3')
const type = ref('Story')
const backlogPriorities = ['P1', 'P2', 'P3']
const backlogTypes = ['Story', 'Bug', 'Improvement', 'Technical']

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

const getBacklogPriority = (backlog: any) => backlog.priority || 'P3'
const getBacklogType = (backlog: any) => backlog.type || 'Story'

const canDeleteTask = (task: any) => ability.can('delete', asSubject('Task', task))

const submitBacklog = async () => {
  if (!selectedFeatureId.value || !title.value) return
  await createBacklogMutation.mutateAsync({
    featureId: selectedFeatureId.value,
    data: {
      title: title.value,
      description: description.value,
      priority: priority.value,
      type: type.value,
      assignedToUserId: assignedToUserId.value || null,
    },
  })
  title.value = ''
  description.value = ''
  assignedToUserId.value = ''
  priority.value = 'P3'
  type.value = 'Story'
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
@reference "../style.css";

.command-bar {
  @apply flex items-end;
}

.command-bar label {
  @apply grid w-full max-w-[340px] gap-1 text-[13px] font-semibold text-slate-600;
}

.command-divider {
  @apply h-[30px] w-px bg-slate-300;
}

.segmented-control {
  @apply inline-flex overflow-hidden rounded border border-slate-300 bg-white;
}

.segmented-control button {
  @apply min-h-8 cursor-pointer border-0 border-r border-slate-300 bg-transparent px-3 text-[13px] font-semibold text-slate-600 last:border-r-0;
}

.segmented-control button.active {
  @apply bg-blue-50 text-blue-800;
}

.backlog-workspace {
  @apply grid grid-cols-1 items-start gap-4.5;
}

.backlog-workspace.form-open {
  @apply grid-cols-[320px_minmax(0,1fr)] max-[900px]:grid-cols-1;
}

.backlog-content {
  @apply min-w-0;
}

.form-card textarea {
  @apply min-h-[110px];
}

.form-actions {
  @apply flex gap-2;
}

.panel,
.board-panel {
  @apply overflow-hidden p-3;
}

.board-panel {
  @apply overflow-x-auto;
}

.backlog-list {
  @apply grid gap-2.5;
}

.backlog-row {
  @apply grid grid-cols-[minmax(0,1fr)_58px_104px_112px_88px_max-content] items-center gap-3 rounded border border-slate-300 bg-white p-3 max-[900px]:grid-cols-1;
}

.priority-pill,
.type-pill {
  @apply inline-flex min-h-6 items-center justify-center whitespace-nowrap rounded-full px-2.5 text-xs font-extrabold;
}

.priority-p1 {
  @apply bg-red-50 text-red-700;
}

.priority-p2 {
  @apply bg-amber-50 text-amber-700;
}

.priority-p3 {
  @apply bg-emerald-50 text-emerald-700;
}

.type-pill {
  @apply bg-slate-50 text-slate-600;
}

.delete-button {
  @apply justify-self-end;
}

.backlog-main {
  @apply flex min-w-0 items-start gap-3;
}

.backlog-main > div {
  @apply min-w-0;
}

.backlog-main h3 {
  @apply m-0 text-[15px];
}

.backlog-main p {
  @apply mt-1 truncate text-slate-600;
}

.toggle-button {
  @apply h-7 w-7 cursor-pointer rounded border border-slate-300 bg-slate-50 text-slate-600;
}

.muted {
  @apply justify-self-end whitespace-nowrap text-[13px] text-slate-600 max-[900px]:justify-self-start;
}

.nested-list {
  @apply col-span-full grid gap-1.5 pt-2.5 pl-10 max-[900px]:pl-0;
}

.nested-row {
  @apply flex min-h-9 min-w-0 cursor-pointer items-center justify-between gap-3 rounded border border-slate-300 bg-slate-50 px-2.5 py-2 text-left text-slate-800;
}

.nested-row span:first-child {
  @apply min-w-0 truncate;
}

.nested-empty {
  @apply text-[13px] text-slate-600;
}
</style>
