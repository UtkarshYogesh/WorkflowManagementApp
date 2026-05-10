<template>
  <section class="page project-detail-page">
    <div class="page-header">
      <div>
        <p class="eyebrow">Project details</p>
        <h1>{{ project?.name || 'Project details' }}</h1>
        <p class="subtitle">Manage the feature roadmap for this project.</p>
      </div>
      <router-link class="button secondary" to="/projects">Back to projects</router-link>
    </div>

    <div class="top-panel">
      <div class="info-card">
        <h2>Project overview</h2>
        <p>{{ project?.description || 'No description available.' }}</p>
        <p class="meta">
          Created on
          {{ project?.createdAt ? new Date(project.createdAt).toLocaleDateString() : '-' }}
        </p>
      </div>
      <div class="form-card">
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
        <button class="button" :disabled="!name" @click="submitFeature">Create feature</button>
      </div>
    </div>

    <div class="features-grid">
      <div class="section-header">
        <h2>Features</h2>
        <p>Open a feature to add backlog items and move tasks.</p>
      </div>
      <div v-if="isFeaturesLoading" class="empty-state">Loading features…</div>
      <div v-else-if="!features?.length" class="empty-state">
        No features yet. Add one to start your backlog.
      </div>
      <div class="cards-grid" v-else>
        <article v-for="feature in features" :key="feature.id" class="feature-card">
          <div>
            <h3>{{ feature.name }}</h3>
            <p>{{ feature.description || 'No description.' }}</p>
            <p class="assignee-label">Assigned to {{ getUserName(feature.assignedToUserId) }}</p>
          </div>
          <div class="feature-footer">
            <span class="status">{{ feature.status }}</span>
            <div class="feature-actions">
              <select
                class="assignee-select"
                :value="feature.assignedToUserId || ''"
                @click.stop
                @change="assignFeatureFromEvent(feature.id, $event)"
              >
                <option value="" disabled>Assign user</option>
                <option v-for="user in users" :key="user.userId" :value="user.userId">
                  {{ user.username }}
                </option>
              </select>
              <router-link :to="`/projects/${projectId}/features/${feature.id}`" class="link-button"
                >Open</router-link
              >
              <button class="button ghost" @click="deleteFeature(feature.id)">Delete</button>
            </div>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { useProject } from '../composables/useProjects'
import {
  useFeatures,
  useCreateFeature,
  useDeleteFeature,
  useAssignFeature,
} from '../composables/useFeatures'
import { useUsers } from '../composables/useUsers'

const route = useRoute()
const projectId = String(route.params.projectId || '')
const { data: project } = useProject(projectId)
const { data: features, isLoading: isFeaturesLoading } = useFeatures(projectId)
const { data: users } = useUsers()
const createFeatureMutation = useCreateFeature()
const deleteFeatureMutation = useDeleteFeature()
const assignFeatureMutation = useAssignFeature()

const name = ref('')
const description = ref('')
const assignedToUserId = ref('')

const submitFeature = async () => {
  if (!name.value) return
  await createFeatureMutation.mutateAsync({
    projectId,
    data: {
      name: name.value,
      description: description.value,
      assignedToUserId: assignedToUserId.value || null,
    },
  })
  name.value = ''
  description.value = ''
  assignedToUserId.value = ''
}

const deleteFeature = async (featureId: string) => {
  await deleteFeatureMutation.mutateAsync(featureId)
}

const assignFeatureFromEvent = async (featureId: string, event: Event) => {
  const userId = (event.target as HTMLSelectElement).value
  if (!userId) return
  await assignFeatureMutation.mutateAsync({ featureId, userId })
}

const getUserName = (userId?: string | null) => {
  if (!userId) return 'Unassigned'
  return users.value?.find((user) => user.userId === userId)?.username || 'Unknown user'
}
</script>

<style scoped>
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 18px;
  margin-bottom: 28px;
}
.eyebrow {
  color: #60a5fa;
  text-transform: uppercase;
  letter-spacing: 0.14em;
  margin-bottom: 10px;
}
.subtitle {
  color: #cbd5e1;
  margin-top: 8px;
}
.top-panel {
  display: grid;
  grid-template-columns: 1.3fr 0.95fr;
  gap: 20px;
  margin-bottom: 24px;
}
.info-card,
.form-card {
  padding: 24px;
  border-radius: 20px;
  background: #111827;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.info-card .meta {
  margin-top: 14px;
  color: #94a3b8;
}
.form-card label {
  display: block;
  margin-bottom: 16px;
  color: #cbd5e1;
}
.form-card input,
.form-card textarea,
.form-card select {
  width: 100%;
  margin-top: 8px;
  padding: 12px;
  border-radius: 14px;
  border: 1px solid #334155;
  background: #0f172a;
  color: #f8fafc;
}
.form-card textarea {
  min-height: 120px;
}
.features-grid {
  padding-top: 16px;
}
.section-header {
  margin-bottom: 16px;
}
.cards-grid {
  display: grid;
  gap: 16px;
}
.feature-card {
  padding: 20px;
  border-radius: 18px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.feature-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 14px;
  margin-top: 18px;
}
.status {
  padding: 8px 14px;
  border-radius: 999px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
}
.assignee-label {
  color: #94a3b8;
  margin-top: 10px;
}
.assignee-select {
  min-height: 38px;
  border-radius: 10px;
  border: 1px solid #334155;
  background: #111827;
  color: #f8fafc;
  padding: 0 10px;
}
.feature-actions {
  display: flex;
  gap: 10px;
  align-items: center;
}
.link-button {
  color: #38bdf8;
  text-decoration: none;
}
.button.ghost {
  background: transparent;
  color: #f8fafc;
  border: 1px solid rgba(148, 163, 184, 0.16);
}
.empty-state {
  padding: 24px;
  border-radius: 18px;
  background: #111827;
  color: #94a3b8;
}
</style>
