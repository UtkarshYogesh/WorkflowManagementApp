<template>
  <section class="page">
    <div class="rounded-md border border-slate-300 bg-white p-4">
      <p class="breadcrumb">
        <router-link to="/projects">Projects</router-link><span>/</span><span>{{ project?.name || 'Project' }}</span>
      </p>
      <div class="grid gap-3 lg:grid-cols-[minmax(0,1fr)_240px_180px_auto] lg:items-end">
        <label>
          Name
          <input v-model="projectDraft.name" />
        </label>
        <div class="field-row">
          <span>Assigned to</span>
          <strong>Project team</strong>
        </div>
        <label>
          Status
          <select v-model="projectDraft.status">
            <option v-for="status in projectStatuses" :key="status" :value="status">{{ status }}</option>
          </select>
        </label>
        <div class="flex gap-2">
          <button class="button primary" :disabled="!isProjectDirty" @click="saveProject">Save</button>
          <button v-if="project && ability.can('delete', asSubject('Project', project))" class="button ghost" @click="deleteProject">Delete</button>
        </div>
      </div>
    </div>

    <div class="mt-4 grid items-start gap-4 xl:grid-cols-[minmax(0,1fr)_340px]">
      <section class="grid gap-4">
        <article class="panel p-5">
          <div class="mb-3 flex items-center justify-between gap-3">
            <h2 class="m-0 text-lg font-semibold text-slate-900">Description</h2>
            <button v-if="!isEditingDescription" class="button secondary" @click="startDescriptionEdit">Edit</button>
          </div>
          <div v-if="!isEditingDescription" class="min-h-32 cursor-text whitespace-pre-wrap rounded border border-transparent p-3 text-sm leading-6 text-slate-700 hover:border-slate-300" @click="startDescriptionEdit">
            {{ project?.description || 'Click to add a description.' }}
          </div>
          <div v-else class="grid gap-3">
            <textarea v-model="projectDraft.description" class="min-h-32"></textarea>
            <div class="flex gap-2">
              <button class="button primary" :disabled="!isProjectDirty" @click="saveProject">Save description</button>
              <button class="button ghost" @click="cancelDescriptionEdit">Cancel</button>
            </div>
          </div>
        </article>

        <article class="panel p-5">
          <CommentsSection v-if="projectId" :entity-id="projectId" :entity-type="CommentEntityType.Project" />
        </article>
      </section>

      <aside class="grid gap-4">
        <section class="panel p-4">
          <h2 class="mb-4 mt-0 text-base font-semibold text-slate-800">Details</h2>
          <div class="grid gap-3">
            <div class="field-row"><span>Created</span><strong>{{ project?.createdAt ? formatDate(project.createdAt) : '-' }}</strong></div>
            <div class="field-row"><span>Features</span><strong>{{ features?.length ?? 0 }}</strong></div>
            <div class="field-row"><span>Assigned features</span><strong>{{ assignedFeatureCount }}</strong></div>
          </div>
        </section>

        <section class="panel p-4">
          <div class="mb-3 flex items-center justify-between gap-2">
            <h2 class="m-0 text-base font-semibold text-slate-800">Features</h2>
            <button v-if="ability.can('create', 'Feature')" class="button primary" @click="showCreateForm = !showCreateForm">Create</button>
          </div>
          <div v-if="isFeaturesLoading" class="empty-state">Loading features...</div>
          <div v-else-if="!features?.length" class="empty-state">No features yet.</div>
          <div v-else class="grid gap-1.5">
            <button v-for="feature in features" :key="feature.id" class="child-link" @click="router.push(`/features/${feature.id}`)">{{ feature.name }}</button>
          </div>
          <form v-if="showCreateForm" class="mt-4 grid gap-2 border-t border-slate-300 pt-4" @submit.prevent="submitFeature">
            <input v-model="name" placeholder="Feature name" />
            <textarea v-model="description" class="min-h-20" placeholder="Description"></textarea>
            <select v-model="assignedToUserId">
              <option value="">Unassigned</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">{{ user.username }}</option>
            </select>
            <button class="button primary" :disabled="!name">Create feature</button>
          </form>
        </section>
      </aside>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProject, useUpdateProject, useDeleteProject } from '../composables/useProjects'
import { useFeatures, useCreateFeature } from '../composables/useFeatures'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { PROJECT_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'

const route = useRoute()
const router = useRouter()
const projectId = String(route.params.projectId || '')
const { data: project } = useProject(projectId)
const { data: features, isLoading: isFeaturesLoading } = useFeatures(projectId)
const { data: users } = useUsers()
const updateProjectMutation = useUpdateProject()
const deleteProjectMutation = useDeleteProject()
const createFeatureMutation = useCreateFeature()
const ability = useAppAbility()

const projectStatuses = PROJECT_STATUSES
const projectDraft = reactive({ name: '', description: '', status: 'New' })
const isEditingDescription = ref(false)
const showCreateForm = ref(false)
const name = ref('')
const description = ref('')
const assignedToUserId = ref('')

watch(project, (value) => {
  if (!value) return
  projectDraft.name = value.name || ''
  projectDraft.description = value.description || ''
  projectDraft.status = value.status || 'New'
}, { immediate: true })

const isProjectDirty = computed(() => {
  if (!project.value) return false
  return projectDraft.name !== (project.value.name || '') || projectDraft.description !== (project.value.description || '') || projectDraft.status !== (project.value.status || 'New')
})
const assignedFeatureCount = computed(() => features.value?.filter((feature: any) => feature.assignedToUserId).length ?? 0)

const startDescriptionEdit = () => { isEditingDescription.value = true }
const cancelDescriptionEdit = () => {
  projectDraft.description = project.value?.description || ''
  isEditingDescription.value = false
}
const saveProject = async () => {
  if (!isProjectDirty.value) return
  await updateProjectMutation.mutateAsync({ projectId, data: { ...projectDraft } })
  isEditingDescription.value = false
}
const deleteProject = async () => {
  await deleteProjectMutation.mutateAsync(projectId)
  router.push('/projects')
}
const submitFeature = async () => {
  if (!name.value) return
  await createFeatureMutation.mutateAsync({ projectId, data: { name: name.value, description: description.value, priority: 'P3', assignedToUserId: assignedToUserId.value || null } })
  name.value = ''
  description.value = ''
  assignedToUserId.value = ''
  showCreateForm.value = false
}
const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
@reference "../style.css";
.breadcrumb span { @apply mx-1.5; }
.field-row { @apply grid gap-1.5 text-[13px] font-bold text-slate-600; }
.field-row strong { @apply text-sm text-slate-800; }
.child-link { @apply min-h-9 truncate rounded border border-slate-300 bg-white px-2.5 text-left text-sm font-semibold text-slate-800 hover:border-blue-700 hover:text-blue-700; }
</style>
