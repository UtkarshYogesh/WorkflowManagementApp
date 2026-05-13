<template>
  <section class="page backlog-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/backlogs">Backlog</router-link>
          <span>/</span>
          <span>{{ backlog?.title || 'Backlog item' }}</span>
        </p>
        <h1>{{ backlog?.title || 'Loading backlog...' }}</h1>
        <p class="subtitle">{{ backlog?.description || 'Manage tasks for this backlog item.' }}</p>
      </div>
      <div class="page-actions">
        <button class="button primary" @click="showCreateForm = !showCreateForm">Create task</button>
      </div>
    </div>

    <div class="detail-grid" :class="{ 'form-open': showCreateForm }">
      <section class="detail-card">
        <h2>Backlog details</h2>
        <div class="meta-grid">
          <label>
            Priority
            <select v-model="backlogDraft.priority">
              <option v-for="option in backlogPriorities" :key="option" :value="option">
                {{ option }}
              </option>
            </select>
          </label>
          <label>
            Type
            <select v-model="backlogDraft.type">
              <option v-for="option in backlogTypes" :key="option" :value="option">
                {{ option }}
              </option>
            </select>
          </label>
          <label>
            Status
            <select v-model="backlogDraft.status">
              <option v-for="status in backlogStatuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </label>
          <span>Created <strong>{{ backlog?.createdAt ? formatDate(backlog.createdAt) : '-' }}</strong></span>
          <label>
            Assignee
            <select v-model="backlogDraft.assignedToUserId">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }}
              </option>
            </select>
          </label>
          <button class="button primary save-button" :disabled="!isBacklogDirty" @click="saveBacklogChanges">
            Save
          </button>
        </div>
      </section>

      <aside v-if="showCreateForm" class="form-card">
        <h2>Create task</h2>
        <label>
          Title
          <input v-model="title" placeholder="Task title" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Task details"></textarea>
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
          <button class="button primary" :disabled="!title" @click="submitTask">Add task</button>
          <button class="button ghost" @click="showCreateForm = false">Cancel</button>
        </div>
      </aside>
    </div>

    <section class="panel">
      <div class="section-header">
        <div>
          <h2>Tasks</h2>
          <p>Track the concrete work needed for this backlog item.</p>
        </div>
      </div>
      <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div>
      <div v-else-if="!tasks?.length" class="empty-state">No tasks yet.</div>
      <div v-else class="work-table">
        <div class="work-table-header">
          <span>Name</span>
          <span>Status</span>
          <span>Assignee</span>
          <span></span>
        </div>
        <article v-for="task in tasks" :key="task.id" class="work-row" @click="navigateToTask(task.id)">
          <div>
            <strong>{{ task.title }}</strong>
            <small>{{ task.description || 'No description' }}</small>
          </div>
          <select v-model="getTaskDraft(task).status" @click.stop>
            <option v-for="status in taskStatuses" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
          <select
            v-model="getTaskDraft(task).assignedToUserId"
            @click.stop
          >
            <option value="" disabled>Assign user</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }}
            </option>
          </select>
          <button class="button secondary" :disabled="!isTaskDirty(task)" @click.stop="saveTaskChanges(task)">
            Save
          </button>
          <button v-if="ability.can('delete', asSubject('Task', task))" class="button ghost" @click.stop="deleteTask(task.id)">
            Delete
          </button>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBacklog, useAssignBacklog, useUpdateBacklog, useUpdateBacklogStatus } from '../composables/useBacklogs'
import { useTasks, useCreateTask, useAssignTask, useUpdateTaskStatus, useDeleteTask } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { BACKLOG_STATUSES, TASK_STATUSES } from '../constants/statuses'

const route = useRoute()
const router = useRouter()
const backlogId = String(route.params.backlogId || '')

const { data: backlog } = useBacklog(backlogId)
const { data: tasks, isLoading: isTasksLoading } = useTasks(backlogId)
const { data: users } = useUsers()
const createTaskMutation = useCreateTask()
const assignBacklogMutation = useAssignBacklog()
const updateBacklogMutation = useUpdateBacklog()
const assignTaskMutation = useAssignTask()
const updateBacklogStatusMutation = useUpdateBacklogStatus()
const updateTaskStatusMutation = useUpdateTaskStatus()
const deleteTaskMutation = useDeleteTask()
const ability = useAppAbility()

const title = ref('')
const description = ref('')
const assignedToUserId = ref('')
const showCreateForm = ref(false)
const backlogStatuses = BACKLOG_STATUSES
const backlogPriorities = ['P1', 'P2', 'P3']
const backlogTypes = ['Story', 'Bug', 'Improvement', 'Technical']
const taskStatuses = TASK_STATUSES
type WorkDraft = { status: string; assignedToUserId: string }
const backlogDraft = reactive({
  status: 'New',
  priority: 'P3',
  type: 'Story',
  assignedToUserId: '',
})
const taskDrafts = reactive<Record<string, WorkDraft>>({})

watch(
  backlog,
  (value) => {
    if (!value) return
    backlogDraft.status = value.status || 'New'
    backlogDraft.priority = value.priority || 'P3'
    backlogDraft.type = value.type || 'Story'
    backlogDraft.assignedToUserId = value.assignedToUserId || ''
  },
  { immediate: true },
)

watch(
  tasks,
  (items) => {
    ;(items || []).forEach((task: any) => {
      taskDrafts[task.id] = {
        status: task.status || 'Todo',
        assignedToUserId: task.assignedToUserId || '',
      }
    })
  },
  { immediate: true },
)

const isBacklogDirty = computed(() => {
  if (!backlog.value) return false
  return (
    backlogDraft.status !== (backlog.value.status || 'New') ||
    backlogDraft.priority !== (backlog.value.priority || 'P3') ||
    backlogDraft.type !== (backlog.value.type || 'Story') ||
    backlogDraft.assignedToUserId !== (backlog.value.assignedToUserId || '')
  )
})

const submitTask = async () => {
  if (!title.value) return
  await createTaskMutation.mutateAsync({
    backlogId,
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

const navigateToTask = (taskId: string) => {
  router.push(`/tasks/${taskId}`)
}

const saveBacklogChanges = async () => {
  if (!backlog.value) return
  if (backlogDraft.status !== (backlog.value.status || 'New')) {
    await updateBacklogStatusMutation.mutateAsync({ backlogId, status: backlogDraft.status })
  }
  if (
    backlogDraft.priority !== (backlog.value.priority || 'P3') ||
    backlogDraft.type !== (backlog.value.type || 'Story')
  ) {
    await updateBacklogMutation.mutateAsync({
      backlogId,
      data: {
        title: backlog.value.title,
        description: backlog.value.description || '',
        priority: backlogDraft.priority,
        type: backlogDraft.type,
        assignedToUserId: backlogDraft.assignedToUserId || null,
      },
    })
  }
  if (
    backlogDraft.assignedToUserId &&
    backlogDraft.assignedToUserId !== (backlog.value.assignedToUserId || '')
  ) {
    await assignBacklogMutation.mutateAsync({ backlogId, userId: backlogDraft.assignedToUserId })
  }
}

const getTaskDraft = (task: any): WorkDraft => {
  if (!taskDrafts[task.id]) {
    taskDrafts[task.id] = {
      status: task.status || 'Todo',
      assignedToUserId: task.assignedToUserId || '',
    }
  }
  return taskDrafts[task.id] as WorkDraft
}

const isTaskDirty = (task: any) => {
  const draft = getTaskDraft(task)
  return (
    draft.status !== (task.status || 'Todo') ||
    draft.assignedToUserId !== (task.assignedToUserId || '')
  )
}

const saveTaskChanges = async (task: any) => {
  const draft = getTaskDraft(task)
  if (draft.status !== (task.status || 'Todo')) {
    await updateTaskStatusMutation.mutateAsync({ taskId: task.id, status: draft.status })
  }
  if (draft.assignedToUserId && draft.assignedToUserId !== (task.assignedToUserId || '')) {
    await assignTaskMutation.mutateAsync({ taskId: task.id, userId: draft.assignedToUserId })
  }
}

const deleteTask = async (taskId: string) => {
  await deleteTaskMutation.mutateAsync(taskId)
}

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 1fr);
  gap: 18px;
  align-items: start;
  margin-bottom: 18px;
}

.detail-grid.form-open {
  grid-template-columns: minmax(0, 1fr) 340px;
}

.meta-grid {
  display: grid;
  grid-template-columns: 110px 150px minmax(160px, 1fr) 160px minmax(180px, 1fr) max-content;
  gap: 14px;
  align-items: end;
  margin-top: 14px;
}

.meta-grid span,
.meta-grid label {
  display: grid;
  gap: 7px;
  color: #5e6c84;
  font-size: 13px;
  font-weight: 700;
}

.meta-grid strong {
  color: #172b4d;
}

.form-card textarea {
  min-height: 110px;
}

.form-actions {
  display: flex;
  gap: 8px;
}

.save-button {
  align-self: end;
}

.section-header {
  margin-bottom: 14px;
}

.section-header p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.work-table {
  overflow-x: auto;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.work-table-header,
.work-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 140px 200px 90px 90px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.work-table-header,
.work-row {
  min-width: 760px;
}

.work-table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.work-row {
  cursor: pointer;
}

.work-row:hover {
  background: #f7f8f9;
}

.work-row:last-child {
  border-bottom: 0;
}

.work-row strong,
.work-row small {
  display: block;
}

.work-row small {
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

@media (max-width: 980px) {
  .detail-grid,
  .detail-grid.form-open,
  .meta-grid,
  .work-table-header,
  .work-row {
    grid-template-columns: 1fr;
  }

  .work-table-header,
  .work-row {
    min-width: 0;
  }
}
</style>
