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

    <div class="detail-grid">
      <section class="detail-card">
        <h2>Feature details</h2>
        <div class="meta-grid">
          <span>Status <strong>{{ feature?.status || '-' }}</strong></span>
          <span>Created <strong>{{ feature?.createdAt ? formatDate(feature.createdAt) : '-' }}</strong></span>
          <label>
            Assignee
            <select :value="feature?.assignedToUserId || ''" @change="assignFeatureFromEvent($event)">
              <option value="" disabled>Assign user</option>
              <option v-for="user in users" :key="user.userId" :value="user.userId">
                {{ user.username }}
              </option>
            </select>
          </label>
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
        </div>
        <article v-for="backlog in backlogs" :key="backlog.id" class="work-row" @click="navigateToBacklog(backlog.id)">
          <div>
            <strong>{{ backlog.title }}</strong>
            <small>{{ backlog.description || 'No description' }}</small>
          </div>
          <span class="status-pill">{{ backlog.status }}</span>
          <select
            :value="backlog.assignedToUserId || ''"
            @click.stop
            @change="assignBacklogFromEvent(backlog.id, $event)"
          >
            <option value="" disabled>Assign user</option>
            <option v-for="user in users" :key="user.userId" :value="user.userId">
              {{ user.username }}
            </option>
          </select>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useFeature, useAssignFeature } from '../composables/useFeatures'
import { useBacklogs, useCreateBacklog, useAssignBacklog } from '../composables/useBacklogs'
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

const title = ref('')
const description = ref('')
const assignedToUserId = ref('')
const showCreateForm = ref(false)

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

const assignFeatureFromEvent = async (event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignFeatureMutation.mutateAsync({ featureId, userId })
}

const assignBacklogFromEvent = async (backlogId: string, event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignBacklogMutation.mutateAsync({ backlogId, userId })
}

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.breadcrumb span {
  margin: 0 6px;
}

.detail-grid {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 340px;
  gap: 18px;
  align-items: start;
  margin-bottom: 18px;
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 14px;
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

.form-card textarea {
  min-height: 110px;
}

.form-actions {
  display: flex;
  gap: 8px;
}

.section-header {
  margin-bottom: 14px;
}

.section-header p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.work-table {
  overflow: hidden;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.work-table-header,
.work-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 120px 200px;
  align-items: center;
  gap: 12px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
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
  .meta-grid,
  .work-table-header,
  .work-row {
    grid-template-columns: 1fr;
  }
}
</style>
