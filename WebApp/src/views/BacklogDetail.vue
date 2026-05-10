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

    <div class="detail-grid">
      <section class="detail-card">
        <h2>Backlog details</h2>
        <div class="meta-grid">
          <span>Status <strong>{{ backlog?.status || '-' }}</strong></span>
          <span>Created <strong>{{ backlog?.createdAt ? formatDate(backlog.createdAt) : '-' }}</strong></span>
          <label>
            Assignee
            <select :value="backlog?.assignedToUserId || ''" @change="assignBacklogFromEvent($event)">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }}
              </option>
            </select>
          </label>
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
        </div>
        <article v-for="task in tasks" :key="task.id" class="work-row" @click="navigateToTask(task.id)">
          <div>
            <strong>{{ task.title }}</strong>
            <small>{{ task.description || 'No description' }}</small>
          </div>
          <span class="status-pill">{{ task.status }}</span>
          <select
            :value="task.assignedToUserId || ''"
            @click.stop
            @change="assignTaskFromEvent(task.id, $event)"
          >
            <option value="" disabled>Assign user</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }}
            </option>
          </select>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBacklog, useAssignBacklog } from '../composables/useBacklogs'
import { useTasks, useCreateTask, useAssignTask } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'

const route = useRoute()
const router = useRouter()
const backlogId = String(route.params.backlogId || '')

const { data: backlog } = useBacklog(backlogId)
const { data: tasks, isLoading: isTasksLoading } = useTasks(backlogId)
const { data: users } = useUsers()
const createTaskMutation = useCreateTask()
const assignBacklogMutation = useAssignBacklog()
const assignTaskMutation = useAssignTask()

const title = ref('')
const description = ref('')
const assignedToUserId = ref('')
const showCreateForm = ref(false)

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

const assignBacklogFromEvent = async (event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignBacklogMutation.mutateAsync({ backlogId, userId })
}

const assignTaskFromEvent = async (taskId: string, event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignTaskMutation.mutateAsync({ taskId, userId })
}

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 340px;
  gap: 18px;
  align-items: start;
  margin-bottom: 18px;
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
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

.section-header {
  margin-bottom: 14px;
}

.section-header p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.work-table {
  overflow: hidden;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.work-table-header,
.work-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 120px 200px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
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
  .meta-grid,
  .work-table-header,
  .work-row {
    grid-template-columns: 1fr;
  }
}
</style>
