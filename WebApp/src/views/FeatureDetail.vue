<template>
  <section class="page">
    <div class="rounded-md border border-slate-300 bg-white p-4">
      <p class="breadcrumb"><router-link to="/features">Features</router-link><span>/</span><span>{{ feature?.name || 'Feature' }}</span></p>
      <div class="grid gap-3 lg:grid-cols-[minmax(0,1fr)_240px_180px_auto] lg:items-end">
        <label>Name<input v-model="featureDraft.name" /></label>
        <label>Assigned to<select v-model="featureDraft.assignedToUserId"><option value="">Unassigned</option><option v-for="user in users" :key="user.userId" :value="user.userId">{{ user.username }}</option></select></label>
        <label>Status<select v-model="featureDraft.status"><option v-for="status in featureStatuses" :key="status" :value="status">{{ status }}</option></select></label>
        <div class="flex gap-2"><button class="button primary" :disabled="!isFeatureDirty" @click="saveFeature">Save</button><button v-if="feature && ability.can('delete', asSubject('Feature', feature))" class="button ghost" @click="deleteFeature">Delete</button></div>
      </div>
    </div>

    <div class="mt-4 grid items-start gap-4 xl:grid-cols-[minmax(0,1fr)_340px]">
      <section class="grid gap-4">
        <article class="panel p-5">
          <div class="mb-3 flex items-center justify-between gap-3"><h2 class="m-0 text-lg font-semibold text-slate-900">Description</h2><button v-if="!isEditingDescription" class="button secondary" @click="startDescriptionEdit">Edit</button></div>
          <div v-if="!isEditingDescription" class="min-h-32 cursor-text whitespace-pre-wrap rounded border border-transparent p-3 text-sm leading-6 text-slate-700 hover:border-slate-300" @click="startDescriptionEdit">{{ feature?.description || 'Click to add a description.' }}</div>
          <div v-else class="grid gap-3"><textarea v-model="featureDraft.description" class="min-h-32"></textarea><div class="flex gap-2"><button class="button primary" :disabled="!isFeatureDirty" @click="saveFeature">Save description</button><button class="button ghost" @click="cancelDescriptionEdit">Cancel</button></div></div>
        </article>
        <article class="panel p-5"><CommentsSection v-if="featureId" :entity-id="featureId" :entity-type="CommentEntityType.Feature" /></article>
      </section>

      <aside class="grid gap-4">
        <section class="panel p-4">
          <h2 class="mb-4 mt-0 text-base font-semibold text-slate-800">Details</h2>
          <div class="grid gap-3">
            <label>Priority<select v-model="featureDraft.priority"><option v-for="option in featurePriorities" :key="option" :value="option">{{ option }}</option></select></label>
            <div class="field-row"><span>Created</span><strong>{{ feature?.createdAt ? formatDate(feature.createdAt) : '-' }}</strong></div>
            <div class="field-row"><span>Backlog items</span><strong>{{ backlogs?.length ?? 0 }}</strong></div>
          </div>
        </section>

        <section class="panel p-4">
          <div class="mb-3 flex items-center justify-between gap-2"><h2 class="m-0 text-base font-semibold text-slate-800">Backlog</h2><button class="button primary" @click="showCreateForm = !showCreateForm">Create</button></div>
          <div v-if="isBacklogsLoading" class="empty-state">Loading backlog...</div>
          <div v-else-if="!backlogs?.length" class="empty-state">No backlog items yet.</div>
          <div v-else class="grid gap-1.5"><button v-for="backlog in backlogs" :key="backlog.id" class="child-link" @click="router.push(`/backlogs/${backlog.id}`)">{{ backlog.title }}</button></div>
          <form v-if="showCreateForm" class="mt-4 grid gap-2 border-t border-slate-300 pt-4" @submit.prevent="submitBacklog">
            <input v-model="title" placeholder="Backlog title" /><textarea v-model="description" class="min-h-20" placeholder="Description"></textarea>
            <select v-model="type"><option v-for="option in backlogTypes" :key="option" :value="option">{{ option }}</option></select>
            <button class="button primary" :disabled="!title">Create backlog</button>
          </form>
        </section>
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useFeature, useAssignFeature, useUpdateFeature, useUpdateFeatureStatus, useDeleteFeature } from '../composables/useFeatures'
import { useBacklogs, useCreateBacklog } from '../composables/useBacklogs'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { FEATURE_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'

const route = useRoute(); const router = useRouter(); const featureId = String(route.params.featureId || '')
const { data: feature } = useFeature(featureId); const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId); const { data: users } = useUsers()
const updateFeatureMutation = useUpdateFeature(); const updateFeatureStatusMutation = useUpdateFeatureStatus(); const assignFeatureMutation = useAssignFeature(); const deleteFeatureMutation = useDeleteFeature(); const createBacklogMutation = useCreateBacklog()
const ability = useAppAbility(); const featureStatuses = FEATURE_STATUSES; const featurePriorities = ['P1', 'P2', 'P3']; const backlogTypes = ['Story', 'Bug', 'Improvement', 'Technical']
const featureDraft = reactive({ name: '', description: '', status: 'Planned', priority: 'P3', assignedToUserId: '' })
const isEditingDescription = ref(false); const showCreateForm = ref(false); const title = ref(''); const description = ref(''); const type = ref('Story')

watch(feature, (value) => { if (!value) return; featureDraft.name = value.name || ''; featureDraft.description = value.description || ''; featureDraft.status = value.status || 'Planned'; featureDraft.priority = value.priority || 'P3'; featureDraft.assignedToUserId = value.assignedToUserId || '' }, { immediate: true })
const isFeatureDirty = computed(() => !!feature.value && (featureDraft.name !== (feature.value.name || '') || featureDraft.description !== (feature.value.description || '') || featureDraft.status !== (feature.value.status || 'Planned') || featureDraft.priority !== (feature.value.priority || 'P3') || featureDraft.assignedToUserId !== (feature.value.assignedToUserId || '')))
const startDescriptionEdit = () => { isEditingDescription.value = true }
const cancelDescriptionEdit = () => { featureDraft.description = feature.value?.description || ''; isEditingDescription.value = false }
const saveFeature = async () => { if (!feature.value || !isFeatureDirty.value) return; if (featureDraft.status !== (feature.value.status || 'Planned')) await updateFeatureStatusMutation.mutateAsync({ featureId, status: featureDraft.status }); await updateFeatureMutation.mutateAsync({ featureId, data: { name: featureDraft.name, description: featureDraft.description, priority: featureDraft.priority, assignedToUserId: featureDraft.assignedToUserId || null } }); if (featureDraft.assignedToUserId && featureDraft.assignedToUserId !== (feature.value.assignedToUserId || '')) await assignFeatureMutation.mutateAsync({ featureId, userId: featureDraft.assignedToUserId }); isEditingDescription.value = false }
const submitBacklog = async () => { if (!title.value) return; await createBacklogMutation.mutateAsync({ featureId, data: { title: title.value, description: description.value, type: type.value, priority: 'P3' } }); title.value = ''; description.value = ''; type.value = 'Story'; showCreateForm.value = false }
const deleteFeature = async () => { await deleteFeatureMutation.mutateAsync(featureId); router.push('/features') }
const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
@reference "../style.css";
.breadcrumb span { @apply mx-1.5; }
.field-row { @apply grid gap-1.5 text-[13px] font-bold text-slate-600; }
.field-row strong { @apply text-sm text-slate-800; }
.child-link { @apply min-h-9 truncate rounded border border-slate-300 bg-white px-2.5 text-left text-sm font-semibold text-slate-800 hover:border-blue-700 hover:text-blue-700; }
</style>
