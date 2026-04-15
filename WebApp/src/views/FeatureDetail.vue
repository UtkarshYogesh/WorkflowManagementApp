<template>
  <section class="page feature-detail">
    <div class="page-header">
      <div>
        <p class="breadcrumb">
          <router-link :to="`/projects/${projectId}`">Projects</router-link>
          <span>/</span>
          <span>{{ feature?.name || 'Feature' }}</span>
        </p>
        <h1>{{ feature?.name || 'Loading feature...' }}</h1>
        <p class="subtitle">Manage backlog items and task flow for this feature.</p>
      </div>
      <router-link class="button secondary" :to="`/projects/${projectId}`"
        >Back to project</router-link
      >
    </div>

    <div class="detail-grid">
      <div class="detail-card">
        <h3>Feature Info</h3>
        <p>
          <strong>Status:</strong> <span class="badge">{{ feature?.status || 'New' }}</span>
        </p>
        <p><strong>Description:</strong> {{ feature?.description || 'No description yet.' }}</p>
        <p>
          <strong>Created:</strong>
          {{ feature?.createdAt ? new Date(feature.createdAt).toLocaleDateString() : '-' }}
        </p>
      </div>

      <div class="detail-card form-card">
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
      </div>
    </div>

    <div class="list-panel">
      <h2>Backlog items</h2>
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlog items...</div>
      <div v-else-if="backlogs?.length === 0" class="empty-state">
        No backlog items yet. Add one now.
      </div>
      <div class="cards-grid">
        <article v-for="backlog in backlogs" :key="backlog.id" class="card">
          <div class="card-header">
            <h3>{{ backlog.title }}</h3>
            <span class="status-pill">{{ backlog.status }}</span>
          </div>
          <p>{{ backlog.description }}</p>
          <div class="card-actions">
            <router-link :to="`/projects/${projectId}/features/${featureId}/backlogs/${backlog.id}`"
              >Open tasks</router-link
            >
            <button class="button ghost" @click="deleteBacklog(backlog.id)">Delete</button>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import {
  useFeature,
  useFeatures,
  useCreateFeature,
  useDeleteFeature,
  useUpdateFeatureStatus,
} from '../composables/useFeatures'
import { useBacklogs, useCreateBacklog, useDeleteBacklog } from '../composables/useBacklogs'

const route = useRoute()
const projectId = String(route.params.projectId || '')
const featureId = String(route.params.featureId || '')

const { data: feature, isLoading: isFeatureLoading } = useFeature(featureId)
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs(featureId)
const createBacklogMutation = useCreateBacklog()
const deleteBacklogMutation = useDeleteBacklog()

const title = ref('')
const description = ref('')

const submitBacklog = async () => {
  if (!title.value) return
  await createBacklogMutation.mutateAsync({
    featureId,
    data: { title: title.value, description: description.value },
  })
  title.value = ''
  description.value = ''
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
