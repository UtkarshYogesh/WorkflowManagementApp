<template>
  <section class="page features">
    <div class="page-header">
      <div>
        <p class="eyebrow">Planning</p>
        <h1>Features</h1>
        <p class="subtitle">Track feature work and the backlog items underneath each feature.</p>
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
        {{ showCreateForm ? 'Close form' : 'Add feature' }}
      </button>
      <div class="segmented-control" aria-label="View mode">
        <button :class="{ active: viewMode === 'list' }" @click="viewMode = 'list'">List</button>
        <button :class="{ active: viewMode === 'board' }" @click="viewMode = 'board'">Board</button>
      </div>
    </div>

    <div class="feature-workspace" :class="{ 'form-open': showCreateForm }">
      <aside v-if="showCreateForm" class="form-card">
        <h2>Create feature</h2>
        <label>
          Name
          <input v-model="name" placeholder="Feature name" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Feature description"></textarea>
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
            <option v-for="option in featurePriorities" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
        </label>
        <div class="form-actions">
          <button class="button primary" :disabled="!selectedProjectId || !name" @click="submitFeature">
            Create feature
          </button>
          <button class="button ghost" @click="showCreateForm = false">Cancel</button>
        </div>
      </aside>

      <div class="feature-content">

        <div v-if="viewMode === 'list'" class="panel">
          <div v-if="isProjectsLoading || isFeaturesLoading" class="empty-state">Loading features...</div>
          <div v-else-if="!projects?.length" class="empty-state">No projects found. Create a project first.</div>
          <div v-else-if="!selectedProjectId" class="empty-state">Select a project to see its features.</div>
          <div v-else-if="!filteredFeatures.length" class="empty-state">No features yet for this project.</div>
          <div v-else class="feature-list">
        <article v-for="feature in filteredFeatures" :key="feature.id" class="feature-row">
              <div class="feature-main">
                <button class="toggle-button" @click="toggleFeature(feature.id)">
                  {{ expandedFeatures.includes(feature.id) ? '-' : '+' }}
                </button>
                <div>
                  <h3 @click="navigateToFeature(feature.id)" class="clickable-title">{{ feature.name }}</h3>
                  <p>{{ feature.description || 'No description' }}</p>
                </div>
              </div>
              <span class="priority-pill" :class="`priority-${getFeaturePriority(feature).toLowerCase()}`">
                {{ getFeaturePriority(feature) }}
              </span>
              <span class="status-pill">{{ feature.status }}</span>
              <span class="muted">{{ getBacklogsForFeature(feature.id).length }} backlog items</span>
              <button v-if="ability.can('delete', asSubject('Feature', feature))" class="button ghost delete-button" @click="deleteFeature(feature.id)">
                Delete
              </button>

              <div v-if="expandedFeatures.includes(feature.id)" class="nested-list">
                <div v-if="getBacklogsForFeature(feature.id).length === 0" class="nested-empty">
                  No backlog items yet.
                </div>
                <button
                  v-for="backlog in getBacklogsForFeature(feature.id)"
                  :key="backlog.id"
                  class="nested-row"
                  @click="navigateToBacklog(backlog.id)"
                >
                  <span>{{ backlog.title }}</span>
                  <span class="status-pill">{{ backlog.status }}</span>
                </button>
              </div>
            </article>
          </div>
        </div>

        <div v-else class="board-panel">
          <div v-if="isProjectsLoading || isFeaturesLoading || isBacklogsLoading" class="empty-state">Loading board...</div>
          <div v-else-if="!projects?.length" class="empty-state">No projects found. Create a project first.</div>
          <div v-else-if="!selectedProjectId" class="empty-state">Select a project to see its feature board.</div>
          <div v-else-if="!filteredFeatures.length" class="empty-state">No features yet for this project.</div>
          <div v-else class="feature-board">
            <section v-for="status in backlogStatuses" :key="status" class="board-column">
              <header>
                <h2>{{ status }}</h2>
                <span>{{ getBacklogsByStatus(status).length }}</span>
              </header>
              <article
                v-for="backlog in getBacklogsByStatus(status)"
                :key="backlog.id"
                class="work-card"
                @click="navigateToBacklog(backlog.id)"
              >
                <strong>{{ backlog.title }}</strong>
                <p>{{ backlog.description || 'No description' }}</p>
                <small>{{ getFeatureName(backlog.featureId) }}</small>
              </article>
              <div v-if="getBacklogsByStatus(status).length === 0" class="empty-column">No items</div>
            </section>
          </div>
        </div>
            </div>
          </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useProjects } from '../composables/useProjects'
import { useFeatures, useCreateFeature, useDeleteFeature } from '../composables/useFeatures'
import { useBacklogs } from '../composables/useBacklogs'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { BACKLOG_STATUSES } from '../constants/statuses'

const router = useRouter()
const { data: projects, isLoading: isProjectsLoading } = useProjects()
const { data: features, isLoading: isFeaturesLoading } = useFeatures()
const { data: allBacklogs, isLoading: isBacklogsLoading } = useBacklogs()
const { data: users } = useUsers()
const createFeatureMutation = useCreateFeature()
const deleteFeatureMutation = useDeleteFeature()
const ability = useAppAbility()

const viewMode = ref<'list' | 'board'>('list')
const expandedFeatures = ref<string[]>([])
const selectedProjectId = ref('')
const showCreateForm = ref(false)
const name = ref('')
const description = ref('')
const assignedToUserId = ref('')
const priority = ref('P3')
const featurePriorities = ['P1', 'P2', 'P3']

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

const selectedFeatureIds = computed(() => filteredFeatures.value.map((feature: any) => feature.id))

const filteredBacklogs = computed(() => {
  const featureIds = new Set(selectedFeatureIds.value)
  return allBacklogs.value?.filter((backlog: any) => featureIds.has(backlog.featureId)) || []
})

const backlogStatuses = computed(() => {
  const baseStatuses = [...BACKLOG_STATUSES]
  const statuses = Array.isArray(filteredBacklogs.value)
    ? filteredBacklogs.value.map((backlog: any) => backlog.status)
    : []
  return [...baseStatuses, ...new Set(statuses.filter((status: string) => status && !baseStatuses.includes(status)))]
})

const toggleFeature = (featureId: string) => {
  const index = expandedFeatures.value.indexOf(featureId)
  if (index > -1) {
    expandedFeatures.value.splice(index, 1)
  } else {
    expandedFeatures.value.push(featureId)
  }
}

const getBacklogsForFeature = (featureId: string) => {
  return filteredBacklogs.value.filter((backlog: any) => backlog.featureId === featureId)
}

const getBacklogsByStatus = (status: string) => {
  return filteredBacklogs.value.filter((backlog: any) => backlog.status === status)
}

const getFeatureName = (featureId: string) => {
  return filteredFeatures.value.find((feature: any) => feature.id === featureId)?.name || 'Feature'
}

const getFeaturePriority = (feature: any) => feature.priority || 'P3'

const submitFeature = async () => {
  if (!selectedProjectId.value || !name.value) return
  await createFeatureMutation.mutateAsync({
    projectId: selectedProjectId.value,
    data: {
      name: name.value,
      description: description.value,
      priority: priority.value,
      assignedToUserId: assignedToUserId.value || null,
    },
  })
  name.value = ''
  description.value = ''
  assignedToUserId.value = ''
  priority.value = 'P3'
  showCreateForm.value = false
}

const deleteFeature = async (featureId: string) => {
  await deleteFeatureMutation.mutateAsync(featureId)
}

const navigateToFeature = (featureId: string) => {
  router.push(`/features/${featureId}`)
}

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
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

.feature-workspace {
  @apply grid grid-cols-1 items-start gap-4.5;
}

.feature-workspace.form-open {
  @apply grid-cols-[320px_minmax(0,1fr)] max-[900px]:grid-cols-1;
}

.feature-content {
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

.feature-list {
  @apply grid gap-2.5;
}

.feature-row {
  @apply grid grid-cols-[minmax(0,1fr)_58px_112px_120px_max-content] items-center gap-3 rounded border border-slate-300 bg-white p-3 max-[900px]:grid-cols-1;
}

.priority-pill {
  @apply inline-flex min-h-6 items-center justify-center rounded-full px-2.5 text-xs font-extrabold;
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

.delete-button {
  @apply justify-self-end;
}

.feature-main {
  @apply flex min-w-0 items-start gap-3;
}

.feature-main > div {
  @apply min-w-0;
}

.feature-main h3 {
  @apply m-0 text-[15px];
}

.feature-main p {
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

.nested-empty,
.empty-column {
  @apply text-[13px] text-slate-600;
}

.feature-board {
  @apply flex gap-3.5 overflow-x-auto pb-2;
}

.board-column {
  @apply min-h-105 min-w-0 flex-[0_0_300px] rounded-md border border-slate-300 bg-slate-50 p-3 max-[900px]:basis-70;
}

.board-column header {
  @apply mb-3 flex items-center justify-between;
}

.board-column h2 {
  @apply m-0 text-sm text-slate-800;
}

.board-column header span {
  @apply font-bold text-slate-600;
}

.work-card {
  @apply mb-2.5 grid min-w-0 cursor-pointer gap-1.5 rounded border border-slate-300 bg-white p-3;
}

.work-card:hover {
  @apply border-blue-700;
}

.work-card p,
.work-card small {
  @apply m-0 text-slate-600;
  overflow-wrap: anywhere;
}
</style>
