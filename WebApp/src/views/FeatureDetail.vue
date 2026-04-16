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
        <button
          class="button secondary"
          @click="viewMode = viewMode === 'kanban' ? 'list' : 'kanban'"
        >
          {{ viewMode === 'kanban' ? 'List View' : 'Kanban View' }}
        </button>
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

    <div v-if="viewMode === 'kanban'" class="kanban-section">
      <h2>Backlog Kanban</h2>
      <KanbanBoard
        :tasks="backlogs"
        :statuses="['New', 'Committed', 'Done']"
        @change-status="changeStatus"
        @delete-task="deleteBacklog"
      />
    </div>

    <div v-else class="list-panel">
      <h2>Backlog List</h2>
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlog items...</div>
      <div v-else-if="backlogs?.length === 0" class="empty-state">
        No backlog items yet. Create one now.
      </div>
      <div v-else>
        <article v-for="backlog in backlogs" :key="backlog.id" class="list-item">
          <h4>{{ backlog.title }}</h4>
          <p>{{ backlog.description }}</p>
          <span class="status-pill">{{ backlog.status }}</span>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import KanbanBoard from '../components/kanban/KanbanBoard.vue'
import { useFeature } from '../composables/useFeatures'
import {
  useBacklogs,
  useCreateBacklog,
  useDeleteBacklog,
  useUpdateBacklogStatus,
} from '../composables/useBacklogs'

const route = useRoute()
const featureId = String(route.params.featureId || '')

const { data: feature } = useFeature(featureId)
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId)
const createBacklogMutation = useCreateBacklog()
const deleteBacklogMutation = useDeleteBacklog()
const updateBacklogStatusMutation = useUpdateBacklogStatus()

const title = ref('')
const description = ref('')
const showCreateForm = ref(false)
const viewMode = ref<'kanban' | 'list'>('kanban')

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

const changeStatus = async ({ taskId, status }: { taskId: string; status: string }) => {
  await updateBacklogStatusMutation.mutateAsync({ backlogId: taskId, status })
}

const deleteBacklog = async (backlogId: string) => {
  await deleteBacklogMutation.mutateAsync(backlogId)
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
