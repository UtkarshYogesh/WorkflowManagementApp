<template>
  <section class="page">
    <div class="rounded-md border border-slate-300 bg-white p-4">
      <p class="breadcrumb"><router-link to="/tasks">Tasks</router-link><span>/</span><span>{{ task?.title || 'Task' }}</span></p>
      <div class="grid gap-3 lg:grid-cols-[minmax(0,1fr)_240px_180px_auto] lg:items-end">
        <label>Name<input v-model="taskDraft.title" /></label>
        <label>Assigned to<select v-model="taskDraft.assignedToUserId"><option value="">Unassigned</option><option v-for="user in users" :key="user.userId" :value="user.userId">{{ user.username }}</option></select></label>
        <label>Status<select v-model="taskDraft.status"><option v-for="status in taskStatuses" :key="status" :value="status">{{ status }}</option></select></label>
        <div class="flex gap-2"><button class="button primary" :disabled="!isTaskDirty" @click="saveTask">Save</button><button v-if="task && ability.can('delete', asSubject('Task', task))" class="button ghost" @click="deleteTask">Delete</button></div>
      </div>
    </div>

    <div class="mt-4 grid items-start gap-4 xl:grid-cols-[minmax(0,1fr)_340px]">
      <section class="grid gap-4">
        <article class="panel p-5">
          <div class="mb-3 flex items-center justify-between gap-3"><h2 class="m-0 text-lg font-semibold text-slate-900">Description</h2><button v-if="!isEditingDescription" class="button secondary" @click="startDescriptionEdit">Edit</button></div>
          <div v-if="!isEditingDescription" class="min-h-32 cursor-text whitespace-pre-wrap rounded border border-transparent p-3 text-sm leading-6 text-slate-700 hover:border-slate-300" @click="startDescriptionEdit">{{ task?.description || 'Click to add a description.' }}</div>
          <div v-else class="grid gap-3"><textarea v-model="taskDraft.description" class="min-h-32"></textarea><div class="flex gap-2"><button class="button primary" :disabled="!isTaskDirty" @click="saveTask">Save description</button><button class="button ghost" @click="cancelDescriptionEdit">Cancel</button></div></div>
        </article>
        <article class="panel p-5"><CommentsSection v-if="taskId" :entity-id="taskId" :entity-type="CommentEntityType.Task" /></article>
      </section>
      <aside class="grid gap-4">
        <section class="panel p-4">
          <h2 class="mb-4 mt-0 text-base font-semibold text-slate-800">Details</h2>
          <div class="grid gap-3"><div class="field-row"><span>Created</span><strong>{{ task?.createdAt ? formatDate(task.createdAt) : '-' }}</strong></div><div class="field-row"><span>Backlog</span><strong>{{ task?.backlogItemId || '-' }}</strong></div></div>
        </section>
        <section class="panel p-4"><h2 class="mb-3 mt-0 text-base font-semibold text-slate-800">Child items</h2><div class="empty-state">Tasks do not have child items.</div></section>
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAssignTask, useDeleteTask, useTask, useUpdateTask, useUpdateTaskStatus } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { TASK_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'
const route = useRoute(); const router = useRouter(); const taskId = String(route.params.taskId || '')
const { data: task } = useTask(taskId); const { data: users } = useUsers(); const updateTaskMutation = useUpdateTask(); const updateTaskStatusMutation = useUpdateTaskStatus(); const assignTaskMutation = useAssignTask(); const deleteTaskMutation = useDeleteTask(); const ability = useAppAbility(); const taskStatuses = TASK_STATUSES
const taskDraft = reactive({ title: '', description: '', status: 'Todo', assignedToUserId: '' }); const isEditingDescription = ref(false)
watch(task, (value) => { if (!value) return; taskDraft.title = value.title || ''; taskDraft.description = value.description || ''; taskDraft.status = value.status || 'Todo'; taskDraft.assignedToUserId = value.assignedToUserId || '' }, { immediate: true })
const isTaskDirty = computed(() => !!task.value && (taskDraft.title !== (task.value.title || '') || taskDraft.description !== (task.value.description || '') || taskDraft.status !== (task.value.status || 'Todo') || taskDraft.assignedToUserId !== (task.value.assignedToUserId || '')))
const startDescriptionEdit = () => { isEditingDescription.value = true }; const cancelDescriptionEdit = () => { taskDraft.description = task.value?.description || ''; isEditingDescription.value = false }
const saveTask = async () => { if (!task.value || !isTaskDirty.value) return; if (taskDraft.status !== (task.value.status || 'Todo')) await updateTaskStatusMutation.mutateAsync({ taskId, status: taskDraft.status }); await updateTaskMutation.mutateAsync({ taskId, data: { title: taskDraft.title, description: taskDraft.description, assignedToUserId: taskDraft.assignedToUserId || null } }); if (taskDraft.assignedToUserId && taskDraft.assignedToUserId !== (task.value.assignedToUserId || '')) await assignTaskMutation.mutateAsync({ taskId, userId: taskDraft.assignedToUserId }); isEditingDescription.value = false }
const deleteTask = async () => { await deleteTaskMutation.mutateAsync(taskId); router.push('/backlogs') }
const formatDate = (value: string) => new Date(value).toLocaleString()
</script>
<style scoped>
@reference "../style.css";
.breadcrumb span { @apply mx-1.5; }
.field-row { @apply grid gap-1.5 text-[13px] font-bold text-slate-600; }
.field-row strong { @apply text-sm text-slate-800; }
</style>
