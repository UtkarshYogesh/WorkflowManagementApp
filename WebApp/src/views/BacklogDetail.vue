<template>
  <section class="page">
    <div class="rounded-md border border-slate-300 bg-white p-4">
      <p class="breadcrumb"><router-link to="/backlogs">Backlog</router-link><span>/</span><span>{{ backlog?.title || 'Backlog item' }}</span></p>
      <div class="grid gap-3 lg:grid-cols-[minmax(0,1fr)_240px_180px_auto] lg:items-end">
        <label>Name<input v-model="backlogDraft.title" /></label>
        <label>Assigned to<select v-model="backlogDraft.assignedToUserId"><option value="">Unassigned</option><option v-for="user in users" :key="user.userId" :value="user.userId">{{ user.username }}</option></select></label>
        <label>Status<select v-model="backlogDraft.status"><option v-for="status in backlogStatuses" :key="status" :value="status">{{ status }}</option></select></label>
        <div class="flex gap-2"><button class="button primary" :disabled="!isBacklogDirty" @click="saveBacklog">Save</button><button v-if="backlog && ability.can('delete', asSubject('Backlog', backlog))" class="button ghost" @click="deleteBacklog">Delete</button></div>
      </div>
    </div>

    <div class="mt-4 grid items-start gap-4 xl:grid-cols-[minmax(0,1fr)_340px]">
      <section class="grid gap-4">
        <article class="panel p-5">
          <div class="mb-3 flex items-center justify-between gap-3"><h2 class="m-0 text-lg font-semibold text-slate-900">Description</h2><button v-if="!isEditingDescription" class="button secondary" @click="startDescriptionEdit">Edit</button></div>
          <div v-if="!isEditingDescription" class="min-h-32 cursor-text whitespace-pre-wrap rounded border border-transparent p-3 text-sm leading-6 text-slate-700 hover:border-slate-300" @click="startDescriptionEdit">{{ backlog?.description || 'Click to add a description.' }}</div>
          <div v-else class="grid gap-3"><textarea v-model="backlogDraft.description" class="min-h-32"></textarea><div class="flex gap-2"><button class="button primary" :disabled="!isBacklogDirty" @click="saveBacklog">Save description</button><button class="button ghost" @click="cancelDescriptionEdit">Cancel</button></div></div>
        </article>
        <article class="panel p-5"><CommentsSection v-if="backlogId" :entity-id="backlogId" :entity-type="CommentEntityType.BacklogItem" /></article>
      </section>

      <aside class="grid gap-4">
        <section class="panel p-4">
          <h2 class="mb-4 mt-0 text-base font-semibold text-slate-800">Details</h2>
          <div class="grid gap-3">
            <label>Priority<select v-model="backlogDraft.priority"><option v-for="option in backlogPriorities" :key="option" :value="option">{{ option }}</option></select></label>
            <label>Type<select v-model="backlogDraft.type"><option v-for="option in backlogTypes" :key="option" :value="option">{{ option }}</option></select></label>
            <div class="field-row"><span>Created</span><strong>{{ backlog?.createdAt ? formatDate(backlog.createdAt) : '-' }}</strong></div>
            <div class="field-row"><span>Tasks</span><strong>{{ tasks?.length ?? 0 }}</strong></div>
          </div>
        </section>
        <section class="panel p-4">
          <div class="mb-3 flex items-center justify-between gap-2"><h2 class="m-0 text-base font-semibold text-slate-800">Tasks</h2><button class="button primary" @click="showCreateForm = !showCreateForm">Create</button></div>
          <div v-if="isTasksLoading" class="empty-state">Loading tasks...</div><div v-else-if="!tasks?.length" class="empty-state">No tasks yet.</div>
          <div v-else class="grid gap-1.5"><button v-for="task in tasks" :key="task.id" class="child-link" @click="router.push(`/tasks/${task.id}`)">{{ task.title }}</button></div>
          <form v-if="showCreateForm" class="mt-4 grid gap-2 border-t border-slate-300 pt-4" @submit.prevent="submitTask"><input v-model="title" placeholder="Task title" /><textarea v-model="description" class="min-h-20" placeholder="Description"></textarea><button class="button primary" :disabled="!title">Create task</button></form>
        </section>
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useBacklog, useAssignBacklog, useUpdateBacklog, useUpdateBacklogStatus, useDeleteBacklog } from '../composables/useBacklogs'
import { useTasks, useCreateTask } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { BACKLOG_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'
const route = useRoute(); const router = useRouter(); const backlogId = String(route.params.backlogId || '')
const { data: backlog } = useBacklog(backlogId); const { data: tasks, isLoading: isTasksLoading } = useTasks(backlogId); const { data: users } = useUsers()
const updateBacklogMutation = useUpdateBacklog(); const updateBacklogStatusMutation = useUpdateBacklogStatus(); const assignBacklogMutation = useAssignBacklog(); const deleteBacklogMutation = useDeleteBacklog(); const createTaskMutation = useCreateTask()
const ability = useAppAbility(); const backlogStatuses = BACKLOG_STATUSES; const backlogPriorities = ['P1', 'P2', 'P3']; const backlogTypes = ['Story', 'Bug', 'Improvement', 'Technical']
const backlogDraft = reactive({ title: '', description: '', status: 'New', priority: 'P3', type: 'Story', assignedToUserId: '' })
const isEditingDescription = ref(false); const showCreateForm = ref(false); const title = ref(''); const description = ref('')
watch(backlog, (value) => { if (!value) return; backlogDraft.title = value.title || ''; backlogDraft.description = value.description || ''; backlogDraft.status = value.status || 'New'; backlogDraft.priority = value.priority || 'P3'; backlogDraft.type = value.type || 'Story'; backlogDraft.assignedToUserId = value.assignedToUserId || '' }, { immediate: true })
const isBacklogDirty = computed(() => !!backlog.value && (backlogDraft.title !== (backlog.value.title || '') || backlogDraft.description !== (backlog.value.description || '') || backlogDraft.status !== (backlog.value.status || 'New') || backlogDraft.priority !== (backlog.value.priority || 'P3') || backlogDraft.type !== (backlog.value.type || 'Story') || backlogDraft.assignedToUserId !== (backlog.value.assignedToUserId || '')))
const startDescriptionEdit = () => { isEditingDescription.value = true }; const cancelDescriptionEdit = () => { backlogDraft.description = backlog.value?.description || ''; isEditingDescription.value = false }
const saveBacklog = async () => { if (!backlog.value || !isBacklogDirty.value) return; if (backlogDraft.status !== (backlog.value.status || 'New')) await updateBacklogStatusMutation.mutateAsync({ backlogId, status: backlogDraft.status }); await updateBacklogMutation.mutateAsync({ backlogId, data: { title: backlogDraft.title, description: backlogDraft.description, priority: backlogDraft.priority, type: backlogDraft.type, assignedToUserId: backlogDraft.assignedToUserId || null } }); if (backlogDraft.assignedToUserId && backlogDraft.assignedToUserId !== (backlog.value.assignedToUserId || '')) await assignBacklogMutation.mutateAsync({ backlogId, userId: backlogDraft.assignedToUserId }); isEditingDescription.value = false }
const submitTask = async () => { if (!title.value) return; await createTaskMutation.mutateAsync({ backlogId, data: { title: title.value, description: description.value } }); title.value = ''; description.value = ''; showCreateForm.value = false }
const deleteBacklog = async () => { await deleteBacklogMutation.mutateAsync(backlogId); router.push('/backlogs') }
const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>
<style scoped>
@reference "../style.css";
.breadcrumb span { @apply mx-1.5; }
.field-row { @apply grid gap-1.5 text-[13px] font-bold text-slate-600; }
.field-row strong { @apply text-sm text-slate-800; }
.child-link { @apply min-h-9 truncate rounded border border-slate-300 bg-white px-2.5 text-left text-sm font-semibold text-slate-800 hover:border-blue-700 hover:text-blue-700; }
</style>
