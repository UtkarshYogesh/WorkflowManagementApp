<template>
  <section class="page projects-page">
    <div class="page-header">
      <div>
        <p class="eyebrow">Projects</p>
        <h1>All projects</h1>
        <p class="subtitle">
          Create a project to begin building features, backlog items, and task boards.
        </p>
      </div>
    </div>

    <div class="form-panel">
      <div class="form-card">
        <h2>Create project</h2>
        <label>
          Name
          <input v-model="name" placeholder="Project name" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="Project description"></textarea>
        </label>
        <button class="button" :disabled="!name" @click="createProject">Create project</button>
      </div>

      <div class="project-list-card">
        <h2>Project list</h2>
        <div v-if="isLoading" class="empty-state">Loading projects…</div>
        <div v-else-if="!projects?.length" class="empty-state">No projects created yet.</div>
        <div class="cards-grid" v-else>
          <article v-for="project in projects" :key="project.projectId" class="project-card">
            <div>
              <h3>{{ project.name }}</h3>
              <p>{{ project.description || 'No description' }}</p>
            </div>
            <div class="project-card-actions">
              <router-link :to="`/projects/${project.projectId}`" class="link-button"
                >Open</router-link
              >
              <button class="button ghost" @click="deleteProject(project.projectId)">Delete</button>
            </div>
          </article>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useProjects, useCreateProject, useDeleteProject } from '../composables/useProjects'

const { data: projects, isLoading } = useProjects()
const createMutation = useCreateProject()
const deleteMutation = useDeleteProject()

const name = ref('')
const description = ref('')

const createProject = async () => {
  if (!name.value) return
  await createMutation.mutateAsync({ name: name.value, description: description.value })
  name.value = ''
  description.value = ''
}

const deleteProject = async (projectId: string) => {
  await deleteMutation.mutateAsync(projectId)
}
</script>

<style scoped>
.page-header {
  margin-bottom: 24px;
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
.form-panel {
  display: grid;
  grid-template-columns: 1.05fr 1fr;
  gap: 22px;
}
.form-card,
.project-list-card {
  padding: 24px;
  border-radius: 20px;
  background: #111827;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.form-card label {
  display: block;
  margin-bottom: 18px;
  color: #cbd5e1;
}
.form-card input,
.form-card textarea {
  width: 100%;
  border-radius: 14px;
  border: 1px solid #334155;
  background: #0f172a;
  color: #f8fafc;
  margin-top: 8px;
  padding: 12px;
}
.form-card textarea {
  min-height: 120px;
}
.cards-grid {
  display: grid;
  gap: 16px;
}
.project-card {
  padding: 18px;
  border-radius: 18px;
  background: #0f172a;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  border: 1px solid rgba(148, 163, 184, 0.12);
}
.project-card-actions {
  display: flex;
  gap: 12px;
  align-items: center;
}
.link-button {
  color: #38bdf8;
  text-decoration: none;
}
.button.ghost {
  background: transparent;
  color: #f8fafc;
  border: 1px solid rgba(148, 163, 184, 0.18);
}
.empty-state {
  padding: 24px;
  border-radius: 18px;
  background: #111827;
  color: #94a3b8;
}
</style>
