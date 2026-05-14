<template>
  <section class="page project-detail-page">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/projects">Projects</router-link>
          <span>/</span>
          <span>{{ project?.name || 'Project' }}</span>
        </p>
        <h1>{{ project?.name || 'Project details' }}</h1>
        <p class="subtitle">{{ project?.description || 'No description available.' }}</p>
      </div>
      <div class="page-actions">
        <div v-if="ability.can('manage', 'Project')" class="status-control">
          <label>
            Status
            <select v-model="projectStatusDraft">
              <option v-for="status in projectStatuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </label>
          <button class="button primary" :disabled="!isProjectStatusDirty" @click="saveProjectStatus">
            Save
          </button>
        </div>
        <router-link class="button secondary" to="/projects">Back to projects</router-link>
      </div>
    </div>

    <div class="summary-grid">
      <article class="summary-card">
        <span>Features</span>
        <strong>{{ features?.length ?? 0 }}</strong>
      </article>
      <article class="summary-card">
        <span>Created</span>
        <strong>{{ project?.createdAt ? formatDate(project.createdAt) : '-' }}</strong>
      </article>
      <article class="summary-card">
        <span>Status</span>
        <strong>{{ project?.status || 'New' }}</strong>
      </article>
      <article class="summary-card">
        <span>Assigned features</span>
        <strong>{{ assignedFeatureCount }}</strong>
      </article>
    </div>

    <div class="project-workspace">
      <aside class="form-card">
        <h2>Add feature</h2>
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
        <button class="button primary" :disabled="!name" @click="submitFeature">Create feature</button>
      </aside>

      <section class="feature-panel panel">
        <div class="section-header">
          <div>
            <h2>Features</h2>
            <p>Plan large pieces of work and break them into backlog items.</p>
          </div>
        </div>
        <div v-if="isFeaturesLoading" class="empty-state">Loading features...</div>
        <div v-else-if="!features?.length" class="empty-state">
          No features yet. Add one to start your backlog.
        </div>
        <div v-else class="feature-table">
          <div class="feature-table-header">
            <span>Feature</span>
            <span>Priority</span>
            <span>Status</span>
            <span>Assignee</span>
            <span></span>
          </div>
          <article v-for="feature in features" :key="feature.id" class="feature-row">
            <div class="feature-name">
              <strong>{{ feature.name }}</strong>
              <small>{{ feature.description || 'No description.' }}</small>
            </div>
            <select v-model="getFeatureDraft(feature).priority">
              <option v-for="option in featurePriorities" :key="option" :value="option">
                {{ option }}
              </option>
            </select>
            <select v-model="getFeatureDraft(feature).status">
              <option v-for="status in featureStatuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
            <select v-model="getFeatureDraft(feature).assignedToUserId">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }}
              </option>
            </select>
            <div class="feature-actions">
              <router-link :to="`/projects/${projectId}/features/${feature.id}`" class="button secondary">
                Open
              </router-link>
              <button class="button secondary" :disabled="!isFeatureDirty(feature)" @click="saveFeatureChanges(feature)">
                Save
              </button>
              <button v-if="ability.can('delete', asSubject('Feature', feature))" class="button ghost" @click="deleteFeature(feature.id)">
                Delete
              </button>
            </div>
          </article>
        </div>
      </section>
    </div>

    <CommentsSection
      v-if="projectId"
      class="mt-5"
      :entity-id="projectId"
      :entity-type="CommentEntityType.Project"
    />
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useProject, useUpdateProjectStatus } from '../composables/useProjects'
import {
  useFeatures,
  useCreateFeature,
  useDeleteFeature,
  useUpdateFeature,
  useAssignFeature,
  useUpdateFeatureStatus,
} from '../composables/useFeatures'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { FEATURE_STATUSES, PROJECT_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'

const route = useRoute()
const projectId = String(route.params.projectId || '')
const { data: project } = useProject(projectId)
const { data: features, isLoading: isFeaturesLoading } = useFeatures(projectId)
const { data: users } = useUsers()
const createFeatureMutation = useCreateFeature()
const deleteFeatureMutation = useDeleteFeature()
const updateProjectStatusMutation = useUpdateProjectStatus()
const updateFeatureMutation = useUpdateFeature()
const assignFeatureMutation = useAssignFeature()
const updateFeatureStatusMutation = useUpdateFeatureStatus()
const ability = useAppAbility()

const name = ref('')
const description = ref('')
const assignedToUserId = ref('')
const priority = ref('P3')
const projectStatusDraft = ref('New')
const projectStatuses = PROJECT_STATUSES
const featureStatuses = FEATURE_STATUSES
const featurePriorities = ['P1', 'P2', 'P3']
type WorkDraft = { status: string; priority: string; assignedToUserId: string }
const featureDrafts = reactive<Record<string, WorkDraft>>({})

watch(
  project,
  (value) => {
    projectStatusDraft.value = value?.status || 'New'
  },
  { immediate: true },
)

watch(
  features,
  (items) => {
    ;(items || []).forEach((feature: any) => {
      featureDrafts[feature.id] = {
        status: feature.status || 'Planned',
        priority: feature.priority || 'P3',
        assignedToUserId: feature.assignedToUserId || '',
      }
    })
  },
  { immediate: true },
)

const assignedFeatureCount = computed(() => {
  return features.value?.filter((feature: any) => feature.assignedToUserId).length ?? 0
})

const isProjectStatusDirty = computed(() => {
  return projectStatusDraft.value !== (project.value?.status || 'New')
})

const saveProjectStatus = async () => {
  if (!projectId || !isProjectStatusDirty.value) return
  await updateProjectStatusMutation.mutateAsync({ projectId, status: projectStatusDraft.value })
}

const submitFeature = async () => {
  if (!name.value) return
  await createFeatureMutation.mutateAsync({
    projectId,
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
}

const deleteFeature = async (featureId: string) => {
  await deleteFeatureMutation.mutateAsync(featureId)
}

const getFeatureDraft = (feature: any): WorkDraft => {
  if (!featureDrafts[feature.id]) {
    featureDrafts[feature.id] = {
      status: feature.status || 'Planned',
      priority: feature.priority || 'P3',
      assignedToUserId: feature.assignedToUserId || '',
    }
  }
  return featureDrafts[feature.id] as WorkDraft
}

const isFeatureDirty = (feature: any) => {
  const draft = getFeatureDraft(feature)
  return (
    draft.status !== (feature.status || 'Planned') ||
    draft.priority !== (feature.priority || 'P3') ||
    draft.assignedToUserId !== (feature.assignedToUserId || '')
  )
}

const saveFeatureChanges = async (feature: any) => {
  const draft = getFeatureDraft(feature)
  if (draft.status !== (feature.status || 'Planned')) {
    await updateFeatureStatusMutation.mutateAsync({ featureId: feature.id, status: draft.status })
  }
  if (draft.priority !== (feature.priority || 'P3')) {
    await updateFeatureMutation.mutateAsync({
      featureId: feature.id,
      data: {
        name: feature.name,
        description: feature.description || '',
        priority: draft.priority,
        assignedToUserId: draft.assignedToUserId || null,
      },
    })
  }
  if (draft.assignedToUserId && draft.assignedToUserId !== (feature.assignedToUserId || '')) {
    await assignFeatureMutation.mutateAsync({ featureId: feature.id, userId: draft.assignedToUserId })
  }
}

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 14px;
  margin-bottom: 18px;
}

.status-control {
  display: flex;
  align-items: end;
  gap: 8px;
}

.status-control label {
  display: grid;
  gap: 7px;
  min-width: 220px;
  color: #44546f;
  font-size: 13px;
  font-weight: 700;
}

.summary-card {
  display: grid;
  gap: 8px;
  padding: 16px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.summary-card span {
  color: #5e6c84;
  font-size: 13px;
  font-weight: 700;
}

.summary-card strong {
  color: #172b4d;
  font-size: 22px;
}

.project-workspace {
  display: grid;
  grid-template-columns: 340px minmax(0, 1fr);
  gap: 18px;
  align-items: start;
}

.form-card textarea {
  min-height: 120px;
}

.section-header {
  margin-bottom: 14px;
}

.section-header p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.feature-table {
  overflow-x: auto;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.feature-table-header,
.feature-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 100px 140px 190px 230px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.feature-table-header,
.feature-row {
  min-width: 860px;
}

.feature-table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.feature-row:last-child {
  border-bottom: 0;
}

.feature-row:hover {
  background: #f7f8f9;
}

.feature-name {
  min-width: 0;
}

.feature-name strong,
.feature-name small {
  display: block;
}

.feature-name small {
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.feature-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

@media (max-width: 1100px) {
  .summary-grid,
  .project-workspace,
  .feature-table-header,
  .feature-row {
    grid-template-columns: 1fr;
  }

  .feature-table-header,
  .feature-row {
    min-width: 0;
  }

  .feature-actions {
    justify-content: flex-start;
  }
}
</style>
