<template>
  <section class="page features">
    <div class="page-header">
      <div>
        <p class="eyebrow">Planning</p>
        <h1>Features</h1>
        <p class="subtitle">Track feature work and the backlog items underneath each feature.</p>
      </div>
      <div class="page-actions">
        <button class="button primary" @click="showCreateForm = !showCreateForm">
          {{ showCreateForm ? 'Close form' : 'Add feature' }}
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

.feature-workspace {
  display: grid;
  grid-template-columns: minmax(0, 1fr);
  gap: 18px;
  align-items: start;
}

.feature-workspace.form-open {
  grid-template-columns: 340px minmax(0, 1fr);
}

.feature-content {
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

.feature-list {
  display: grid;
  gap: 10px;
}

.feature-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 58px max-content 140px max-content;
  gap: 14px;
  align-items: center;
  padding: 14px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.priority-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 24px;
  padding: 0 10px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: 800;
}

.priority-p1 {
  background: #ffebe6;
  color: #ae2e24;
}

.priority-p2 {
  background: #fff7d6;
  color: #7f5f01;
}

.priority-p3 {
  background: #e3fcef;
  color: #216e4e;
}

.delete-button {
  justify-self: end;
}

.feature-main {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  min-width: 0;
}

.feature-main > div {
  min-width: 0;
}

.feature-main h3 {
  margin: 0;
  font-size: 15px;
}

.feature-main p {
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

.nested-empty,
.empty-column {
  color: #5e6c84;
  font-size: 13px;
}

.feature-board {
  display: flex;
  gap: 14px;
  overflow-x: auto;
  padding-bottom: 8px;
}

.board-column {
  flex: 0 0 300px;
  min-height: 420px;
  min-width: 0;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #f7f8f9;
}

.board-column header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.board-column h2 {
  margin: 0;
  font-size: 14px;
  color: #172b4d;
}

.board-column header span {
  color: #5e6c84;
  font-weight: 700;
}

.work-card {
  display: grid;
  gap: 6px;
  margin-bottom: 10px;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
  cursor: pointer;
  min-width: 0;
}

.work-card:hover {
  border-color: #0c66e4;
}

.work-card p,
.work-card small {
  margin: 0;
  color: #5e6c84;
  overflow-wrap: anywhere;
}

@media (max-width: 900px) {
  .scope-panel {
    justify-content: stretch;
  }

  .feature-workspace {
    grid-template-columns: 1fr;
  }

  .feature-row {
    grid-template-columns: 1fr;
  }

  .muted {
    justify-self: start;
  }

  .nested-list {
    padding-left: 0;
  }

  .board-column {
    flex-basis: 280px;
  }
}
</style>
