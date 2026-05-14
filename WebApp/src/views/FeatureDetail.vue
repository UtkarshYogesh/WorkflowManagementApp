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
          <div class="summary-pills">
            <span class="priority-pill" :class="`priority-${featureDraft.priority.toLowerCase()}`">
              {{ featureDraft.priority }}
            </span>
            <span class="status-pill">{{ feature?.status || 'Planned' }}</span>
          </div>
        </div>
        <div class="meta-grid">
          <span>Created <strong>{{ feature?.createdAt ? formatDate(feature.createdAt) : '-' }}</strong></span>
          <label>
            Priority
            <select v-model="featureDraft.priority">
              <option v-for="option in featurePriorities" :key="option" :value="option">
                {{ option }}
              </option>
            </select>
          </label>
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
        <label>
          Priority
          <select v-model="priority">
            <option v-for="option in backlogPriorities" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
        </label>
        <label>
          Type
          <select v-model="type">
            <option v-for="option in backlogTypes" :key="option" :value="option">
              {{ option }}
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
          <span>Priority</span>
          <span>Type</span>
          <span>Status</span>
          <span>Assignee</span>
          <span></span>
        </div>
        <article v-for="backlog in backlogs" :key="backlog.id" class="work-row" @click="navigateToBacklog(backlog.id)">
          <div>
            <strong>{{ backlog.title }}</strong>
            <small>{{ backlog.description || 'No description' }}</small>
          </div>
          <select v-model="getBacklogDraft(backlog).priority" @click.stop>
            <option v-for="option in backlogPriorities" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
          <select v-model="getBacklogDraft(backlog).type" @click.stop>
            <option v-for="option in backlogTypes" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
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
          <button v-if="ability.can('delete', asSubject('Backlog', backlog))" class="button ghost" @click.stop="deleteBacklog(backlog.id)">
            Delete
          </button>
        </article>
      </div>
    </section>

    <CommentsSection
      v-if="featureId"
      class="mt-5"
      :entity-id="featureId"
      :entity-type="CommentEntityType.Feature"
    />
  </section>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useFeature, useAssignFeature, useUpdateFeature, useUpdateFeatureStatus } from '../composables/useFeatures'
import {
  useBacklogs,
  useCreateBacklog,
  useUpdateBacklog,
  useAssignBacklog,
  useUpdateBacklogStatus,
  useDeleteBacklog,
} from '../composables/useBacklogs'
import { useUsers } from '../composables/useUsers'
import { asSubject, useAppAbility } from '../permissions/ability'
import { BACKLOG_STATUSES, FEATURE_STATUSES } from '../constants/statuses'
import CommentsSection from '../components/comments/CommentsSection.vue'
import { CommentEntityType } from '../services/commentApi'

const route = useRoute()
const router = useRouter()
const featureId = String(route.params.featureId || '')

const { data: feature } = useFeature(featureId)
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId)
const { data: users } = useUsers()
const createBacklogMutation = useCreateBacklog()
const assignFeatureMutation = useAssignFeature()
const updateFeatureMutation = useUpdateFeature()
const updateBacklogMutation = useUpdateBacklog()
const assignBacklogMutation = useAssignBacklog()
const updateFeatureStatusMutation = useUpdateFeatureStatus()
const updateBacklogStatusMutation = useUpdateBacklogStatus()
const deleteBacklogMutation = useDeleteBacklog()
const ability = useAppAbility()

const title = ref('')
const description = ref('')
const assignedToUserId = ref('')
const priority = ref('P3')
const type = ref('Story')
const showCreateForm = ref(false)
const featureStatuses = FEATURE_STATUSES
const featurePriorities = ['P1', 'P2', 'P3']
const backlogStatuses = BACKLOG_STATUSES
const backlogPriorities = ['P1', 'P2', 'P3']
const backlogTypes = ['Story', 'Bug', 'Improvement', 'Technical']
type WorkDraft = { status: string; priority: string; type: string; assignedToUserId: string }
const featureDraft = reactive({
  status: 'Planned',
  priority: 'P3',
  assignedToUserId: '',
})
const backlogDrafts = reactive<Record<string, WorkDraft>>({})

watch(
  feature,
  (value) => {
    if (!value) return
    featureDraft.status = value.status || 'Planned'
    featureDraft.priority = value.priority || 'P3'
    featureDraft.assignedToUserId = value.assignedToUserId || ''
  },
  { immediate: true },
)

watch(
  backlogs,
  (items) => {
    ;(items || []).forEach((backlog: any) => {
      backlogDrafts[backlog.id] = {
        status: backlog.status || 'New',
        priority: backlog.priority || 'P3',
        type: backlog.type || 'Story',
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
    featureDraft.priority !== (feature.value.priority || 'P3') ||
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
      priority: priority.value,
      type: type.value,
      assignedToUserId: assignedToUserId.value || null,
    },
  })
  title.value = ''
  description.value = ''
  assignedToUserId.value = ''
  priority.value = 'P3'
  type.value = 'Story'
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
  if (featureDraft.priority !== (feature.value.priority || 'P3')) {
    await updateFeatureMutation.mutateAsync({
      featureId,
      data: {
        name: feature.value.name,
        description: feature.value.description || '',
        priority: featureDraft.priority,
        assignedToUserId: featureDraft.assignedToUserId || null,
      },
    })
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
    draft.status !== (backlog.status || 'New') ||
    draft.priority !== (backlog.priority || 'P3') ||
    draft.type !== (backlog.type || 'Story') ||
    draft.assignedToUserId !== (backlog.assignedToUserId || '')
  )
}

const saveBacklogChanges = async (backlog: any) => {
  const draft = getBacklogDraft(backlog)
  if (draft.status !== (backlog.status || 'New')) {
    await updateBacklogStatusMutation.mutateAsync({ backlogId: backlog.id, status: draft.status })
  }
  if (
    draft.priority !== (backlog.priority || 'P3') ||
    draft.type !== (backlog.type || 'Story')
  ) {
    await updateBacklogMutation.mutateAsync({
      backlogId: backlog.id,
      data: {
        title: backlog.title,
        description: backlog.description || '',
        priority: draft.priority,
        type: draft.type,
        assignedToUserId: draft.assignedToUserId || null,
      },
    })
  }
  if (draft.assignedToUserId && draft.assignedToUserId !== (backlog.assignedToUserId || '')) {
    await assignBacklogMutation.mutateAsync({ backlogId: backlog.id, userId: draft.assignedToUserId })
  }
}

const deleteBacklog = async (backlogId: string) => {
  await deleteBacklogMutation.mutateAsync(backlogId)
}

const getBacklogDraft = (backlog: any): WorkDraft => {
  if (!backlogDrafts[backlog.id]) {
    backlogDrafts[backlog.id] = {
      status: backlog.status || 'New',
      priority: backlog.priority || 'P3',
      type: backlog.type || 'Story',
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

.summary-pills {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: flex-end;
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

.feature-summary p {
  margin: 6px 0 0;
  color: #5e6c84;
}

.meta-grid {
  display: grid;
  grid-template-columns: 160px minmax(110px, 0.7fr) minmax(180px, 1fr) minmax(180px, 1fr) max-content;
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
  grid-template-columns: minmax(0, 1fr) 90px 130px 140px 200px 90px 90px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.work-table-header,
.work-row {
  min-width: 980px;
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
