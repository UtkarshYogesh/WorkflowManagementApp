<template>
  <section class="page feature-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link to="/features">Features</router-link>
          <span>/</span>
          <span>{{ feature?.name || 'Feature' }}</span>
        </p>
        <h1>{{ feature?.name || 'Loading feature...' }}</h1>
        <p class="subtitle">{{ feature?.description || 'Manage backlog items for this feature.' }}</p>
      </div>
      <div class="page-actions">
        <button class="button primary" @click="showCreateForm = !showCreateForm">Create backlog</button>
      </div>
    </div>

    <div class="detail-grid" :class="{ 'form-open': showCreateForm }">
      <section class="detail-card">
        <div class="feature-summary">
          <div>
            <h2>Feature details</h2>
            <p>{{ feature?.description || 'No description added yet.' }}</p>
          </div>
          <span class="status-pill">{{ feature?.status || 'Planned' }}</span>
        </div>
        <div class="meta-grid">
          <span>Created <strong>{{ feature?.createdAt ? formatDate(feature.createdAt) : '-' }}</strong></span>
          <label>
            Status
            <select v-model="featureDraft.status">
              <option v-for="status in featureStatuses" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </label>
          <label>
            Assignee
            <select v-model="featureDraft.assignedToUserId">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }}
              </option>
            </select>
          </label>
          <button class="button primary save-button" :disabled="!isFeatureDirty" @click="saveFeatureChanges">
            Save
          </button>
        </div>
      </section>

      <aside v-if="showCreateForm" class="form-card">
        <h2>Add backlog item</h2>
        <label>
          Title
          <input v-model="title" placeholder="Backlog title" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Backlog description"></textarea>
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
          <button class="button primary" :disabled="!title" @click="submitBacklog">Create backlog</button>
          <button class="button ghost" @click="showCreateForm = false">Cancel</button>
        </div>
      </aside>
    </div>

    <section class="panel">
      <div class="section-header">
        <div>
          <h2>Backlog items</h2>
          <p>Break this feature into deliverable work items.</p>
        </div>
      </div>
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlog items...</div>
      <div v-else-if="!backlogs?.length" class="empty-state">No backlog items yet.</div>
      <div v-else class="work-table">
        <div class="work-table-header">
          <span>Name</span>
          <span>Status</span>
          <span>Assignee</span>
          <span></span>
        </div>
        <article v-for="backlog in backlogs" :key="backlog.id" class="work-row" @click="navigateToBacklog(backlog.id)">
          <div>
            <strong>{{ backlog.title }}</strong>
            <small>{{ backlog.description || 'No description' }}</small>
          </div>
          <select v-model="getBacklogDraft(backlog).status" @click.stop>
            <option v-for="status in backlogStatuses" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
          <select
            v-model="getBacklogDraft(backlog).assignedToUserId"
            @click.stop
          >
            <option value="" disabled>Assign user</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }}
            </option>
          </select>
          <button class="button secondary" :disabled="!isBacklogDirty(backlog)" @click.stop="saveBacklogChanges(backlog)">
            Save
          </button>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useFeature, useAssignFeature, useUpdateFeatureStatus } from '../composables/useFeatures'
import {
  useBacklogs,
  useCreateBacklog,
  useAssignBacklog,
  useUpdateBacklogStatus,
} from '../composables/useBacklogs'
import { useUsers } from '../composables/useUsers'

const route = useRoute()
const router = useRouter()
const featureId = String(route.params.featureId || '')

const { data: feature } = useFeature(featureId)
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId)
const { data: users } = useUsers()
const createBacklogMutation = useCreateBacklog()
const assignFeatureMutation = useAssignFeature()
const assignBacklogMutation = useAssignBacklog()
const updateFeatureStatusMutation = useUpdateFeatureStatus()
const updateBacklogStatusMutation = useUpdateBacklogStatus()

const title = ref('')
const description = ref('')
const assignedToUserId = ref('')
const showCreateForm = ref(false)
const featureStatuses = ['Planned', 'Committed', 'Done']
const backlogStatuses = ['Planned', 'Committed', 'Done']
type WorkDraft = { status: string; assignedToUserId: string }
const featureDraft = reactive({
  status: 'Planned',
  assignedToUserId: '',
})
const backlogDrafts = reactive<Record<string, WorkDraft>>({})

watch(
  feature,
  (value) => {
    if (!value) return
    featureDraft.status = value.status || 'Planned'
    featureDraft.assignedToUserId = value.assignedToUserId || ''
  },
  { immediate: true },
)

watch(
  backlogs,
  (items) => {
    ;(items || []).forEach((backlog: any) => {
      backlogDrafts[backlog.id] = {
        status: backlog.status || 'Planned',
        assignedToUserId: backlog.assignedToUserId || '',
      }
    })
  },
  { immediate: true },
)

const isFeatureDirty = computed(() => {
  if (!feature.value) return false
  return (
    featureDraft.status !== (feature.value.status || 'Planned') ||
    featureDraft.assignedToUserId !== (feature.value.assignedToUserId || '')
  )
})

const submitBacklog = async () => {
  if (!title.value) return
  await createBacklogMutation.mutateAsync({
    featureId,
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

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
}

const saveFeatureChanges = async () => {
  if (!feature.value) return
  if (featureDraft.status !== (feature.value.status || 'Planned')) {
    await updateFeatureStatusMutation.mutateAsync({ featureId, status: featureDraft.status })
  }
  if (
    featureDraft.assignedToUserId &&
    featureDraft.assignedToUserId !== (feature.value.assignedToUserId || '')
  ) {
    await assignFeatureMutation.mutateAsync({ featureId, userId: featureDraft.assignedToUserId })
  }
}

const isBacklogDirty = (backlog: any) => {
  const draft = getBacklogDraft(backlog)
  return (
    draft.status !== (backlog.status || 'Planned') ||
    draft.assignedToUserId !== (backlog.assignedToUserId || '')
  )
}

const saveBacklogChanges = async (backlog: any) => {
  const draft = getBacklogDraft(backlog)
  if (draft.status !== (backlog.status || 'Planned')) {
    await updateBacklogStatusMutation.mutateAsync({ backlogId: backlog.id, status: draft.status })
  }
  if (draft.assignedToUserId && draft.assignedToUserId !== (backlog.assignedToUserId || '')) {
    await assignBacklogMutation.mutateAsync({ backlogId: backlog.id, userId: draft.assignedToUserId })
  }
}

const getBacklogDraft = (backlog: any): WorkDraft => {
  if (!backlogDrafts[backlog.id]) {
    backlogDrafts[backlog.id] = {
      status: backlog.status || 'Planned',
      assignedToUserId: backlog.assignedToUserId || '',
    }
  }
  return backlogDrafts[backlog.id] as WorkDraft
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
  grid-template-columns: minmax(0, 1fr) 360px;
}

.feature-detail .detail-card,
.feature-detail .form-card,
.feature-detail .panel {
  background: #ffffff;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  color: #172b4d;
  box-shadow: 0 1px 2px rgba(9, 30, 66, 0.08);
}

.feature-detail .detail-card h2,
.feature-detail .form-card h2,
.feature-detail .panel h2 {
  margin: 0;
  color: #172b4d;
}

.feature-summary {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
}

.feature-summary p {
  margin: 6px 0 0;
  color: #5e6c84;
}

.meta-grid {
  display: grid;
  grid-template-columns: 160px minmax(180px, 1fr) minmax(180px, 1fr) max-content;
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

.feature-detail select,
.feature-detail input,
.feature-detail textarea {
  background: #ffffff;
  color: #172b4d;
  border-color: #c1c7d0;
}

.feature-detail .form-card label {
  color: #44546f;
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
  grid-template-columns: minmax(0, 1fr) 140px 200px 90px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.work-table-header,
.work-row {
  min-width: 680px;
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

  .feature-summary {
    display: grid;
  }
}
</style>
