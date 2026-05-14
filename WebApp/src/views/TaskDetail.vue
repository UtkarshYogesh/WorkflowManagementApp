<template>
  <section class="page task-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/tasks">Tasks</router-link>
          <span>/</span>
          <span>{{ task?.title || 'Task' }}</span>
        </p>
        <h1>{{ task?.title || 'Loading task...' }}</h1>
        <p class="subtitle">{{ task?.description || 'No description added yet.' }}</p>
      </div>
    </div>

    <div class="task-layout">
      <section class="detail-card">
        <h2>Task details</h2>
        <p>{{ task?.description || 'No description added yet.' }}</p>
      </section>

      <aside class="detail-card">
        <h2>Fields</h2>
        <div class="field-list">
          <label>
            Status
            <select v-model="taskDraft.status">
              <option v-for="status in taskStatuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </label>
          <span>Created <strong>{{ task?.createdAt ? formatDate(task.createdAt) : '-' }}</strong></span>
          <label>
            Assignee
            <select v-model="taskDraft.assignedToUserId">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }} ({{ user.email }})
              </option>
            </select>
          </label>
          <button class="button primary" :disabled="!isTaskDirty" @click="saveTaskChanges">Save</button>
          <button v-if="task && ability.can('delete', asSubject('Task', task))" class="button ghost" @click="deleteTask">Delete</button>
        </div>
      </aside>
    </div>

    <CommentsSection
      v-if="taskId"
      class="mt-5"
      :entity-id="taskId"
      :entity-type="CommentEntityType.Task"
    />
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAssignTask, useDeleteTask, useTask, useUpdateTaskStatus } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { TASK_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'

const route = useRoute()
const router = useRouter()
const taskId = String(route.params.taskId || '')

const { data: task } = useTask(taskId)
const { data: users } = useUsers()
const assignTaskMutation = useAssignTask()
const updateTaskStatusMutation = useUpdateTaskStatus()
const deleteTaskMutation = useDeleteTask()
const ability = useAppAbility()
const taskStatuses = TASK_STATUSES
const taskDraft = reactive({
  status: 'Todo',
  assignedToUserId: '',
})

watch(
  task,
  (value) => {
    if (!value) return
    taskDraft.status = value.status || 'Todo'
    taskDraft.assignedToUserId = value.assignedToUserId || ''
  },
  { immediate: true },
)

const isTaskDirty = computed(() => {
  if (!task.value) return false
  return (
    taskDraft.status !== (task.value.status || 'Todo') ||
    taskDraft.assignedToUserId !== (task.value.assignedToUserId || '')
  )
})

const saveTaskChanges = async () => {
  if (!task.value) return
  if (taskDraft.status !== (task.value.status || 'Todo')) {
    await updateTaskStatusMutation.mutateAsync({ taskId, status: taskDraft.status })
  }
  if (taskDraft.assignedToUserId && taskDraft.assignedToUserId !== (task.value.assignedToUserId || '')) {
    await assignTaskMutation.mutateAsync({ taskId, userId: taskDraft.assignedToUserId })
  }
}

const deleteTask = async () => {
  await deleteTaskMutation.mutateAsync(taskId)
  router.push('/backlogs')
}

const formatDate = (value: string) => new Date(value).toLocaleString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.task-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 340px;
  gap: 18px;
  align-items: start;
}

.detail-card h2 {
  margin: 0 0 14px;
}

.detail-card p {
  color: #44546f;
}

.field-list {
  display: grid;
  gap: 14px;
}

.field-list span,
.field-list label {
  display: grid;
  gap: 7px;
  color: #5e6c84;
  font-size: 13px;
  font-weight: 700;
}

.field-list strong {
  color: #172b4d;
}

@media (max-width: 900px) {
  .task-layout {
    grid-template-columns: 1fr;
  }
}
</style>
