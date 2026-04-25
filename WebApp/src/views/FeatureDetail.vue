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
        <p class="subtitle">Manage backlog items for this feature.</p>
      </div>
      <div class="page-actions">
        <button class="button" @click="showCreateForm = !showCreateForm">Create Backlog</button>
      </div>
    </div>

    <div v-if="showCreateForm" class="form-panel">
      <div class="form-card">
        <h3>Add a backlog item</h3>
        <label>
          Title
          <input v-model="title" placeholder="Backlog title" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Backlog description"></textarea>
        </label>
        <button class="button" :disabled="!title" @click="submitBacklog">Create backlog</button>
        <button class="button ghost" @click="showCreateForm = false">Cancel</button>
      </div>
    </div>

    <div class="backlogs-section">
      <h2>Backlogs</h2>
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlogs...</div>
      <div v-else-if="!backlogs?.length" class="empty-state">
        No backlogs yet. Create your first backlog.
      </div>
      <div v-else class="backlogs-table">
        <div class="table-header">
          <div class="header-cell name-header">Name</div>
          <div class="header-cell status-header">Status</div>
        </div>
        <div
          v-for="backlog in backlogs"
          :key="backlog.id"
          class="table-row"
          @click="navigateToBacklog(backlog.id)"
        >
          <div class="table-cell name-cell">{{ backlog.title }}</div>
          <div class="table-cell status-cell">
            <span class="status-pill">{{ backlog.status }}</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useFeature } from '../composables/useFeatures'
import {
  useBacklogs,
  useCreateBacklog,
  useDeleteBacklog,
  useUpdateBacklogStatus,
} from '../composables/useBacklogs'

const route = useRoute()
const router = useRouter()
const featureId = String(route.params.featureId || '')

const { data: feature } = useFeature(featureId)
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId)
const createBacklogMutation = useCreateBacklog()
const deleteBacklogMutation = useDeleteBacklog()
const updateBacklogStatusMutation = useUpdateBacklogStatus()

const title = ref('')
const description = ref('')
const showCreateForm = ref(false)

const submitBacklog = async () => {
  if (!title.value) return
  await createBacklogMutation.mutateAsync({
    featureId,
    data: { title: title.value, description: description.value },
  })
  title.value = ''
  description.value = ''
  showCreateForm.value = false
}

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
}
</script>

<style scoped>
.page {
  padding: 28px 32px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  align-items: flex-start;
  margin-bottom: 28px;
}
.breadcrumb {
  color: #a0aec0;
  font-size: 0.95rem;
  margin-bottom: 8px;
}
.breadcrumb span {
  margin: 0 8px;
}
.subtitle {
  color: #cbd5e1;
  margin-top: 8px;
}
.detail-grid {
  display: grid;
  gap: 20px;
  grid-template-columns: 1.4fr 0.9fr;
  margin-bottom: 24px;
}
.detail-card {
  padding: 22px;
  border-radius: 18px;
  background: #111827;
  border: 1px solid rgba(148, 163, 184, 0.16);
}
.form-card input,
.form-card textarea {
  width: 100%;
  margin-top: 8px;
  border-radius: 12px;
  border: 1px solid #334155;
  background: #0f172a;
  color: #f8fafc;
  padding: 10px 12px;
}
.form-card textarea {
  min-height: 110px;
}
.button {
  margin-top: 14px;
}
.status-pill {
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  padding: 4px 12px;
  border-radius: 999px;
  font-size: 0.85rem;
}
.list-panel {
  margin-top: 12px;
}
.backlogs-section {
  margin-top: 12px;
}
.backlogs-section h2 {
  color: #f8fafc;
  margin-bottom: 20px;
}
.backlogs-table {
  background: #0f172a;
  border-radius: 12px;
  border: 1px solid rgba(148, 163, 184, 0.12);
  overflow: hidden;
}
.table-header {
  display: flex;
  background: rgba(148, 163, 184, 0.05);
  border-bottom: 1px solid rgba(148, 163, 184, 0.12);
}
.header-cell {
  padding: 16px;
  font-weight: 600;
  color: #cbd5e1;
  flex: 1;
}
.name-header {
  flex: 2;
}
.status-header {
  flex: 1;
}
.table-row {
  display: flex;
  border-bottom: 1px solid rgba(148, 163, 184, 0.08);
  cursor: pointer;
  transition: background-color 0.2s ease;
}
.table-row:hover {
  background: rgba(148, 163, 184, 0.03);
}
.table-row:last-child {
  border-bottom: none;
}
.table-cell {
  padding: 16px;
  display: flex;
  align-items: center;
  flex: 1;
}
.name-cell {
  flex: 2;
  color: #f8fafc;
  font-weight: 500;
}
.status-cell {
  flex: 1;
}
.status-pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 6px 14px;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  font-size: 0.85rem;
  font-weight: 600;
}
.cards-grid {
  display: grid;
  gap: 14px;
}
.card {
  padding: 18px;
  border-radius: 18px;
  background: #0f172a;
  box-shadow: 0 16px 30px rgba(15, 23, 42, 0.12);
  border: 1px solid rgba(148, 163, 184, 0.1);
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}
.card-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 16px;
}
.empty-state {
  padding: 32px;
  border-radius: 16px;
  background: #111827;
  color: #94a3b8;
}
</style>
