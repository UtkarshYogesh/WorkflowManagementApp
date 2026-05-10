<template>
  <section class="page projects-page">
    <div class="page-header">
      <div>
        <p class="eyebrow">Projects</p>
        <h1>Project directory</h1>
        <p class="subtitle">
          {{ ability.can('create', 'Project') ? 'Create projects and open their delivery workspace.' : 'View organization projects and open their delivery workspace.' }}
        </p>
      </div>
    </div>

    <div class="projects-layout">
      <aside v-if="ability.can('create', 'Project')" class="form-card">
        <h2>Create project</h2>
        <label>
          Name
          <input v-model="name" placeholder="Project name" />
        </label>
        <label>
          Description
          <textarea v-model="description" placeholder="What is this project for?"></textarea>
        </label>
        <button class="button primary" :disabled="!name" @click="createProject">Create project</button>
      </aside>

      <section class="project-list-card">
        <div class="section-header">
          <div>
            <h2>All projects</h2>
            <p>{{ projects?.length ?? 0 }} projects in this workspace</p>
          </div>
        </div>

        <div v-if="isLoading" class="empty-state">Loading projects...</div>
        <div v-else-if="!projects?.length" class="empty-state">No projects created yet.</div>
        <div v-else class="project-table">
          <div class="project-table-header">
            <span>Name</span>
            <span>Created</span>
            <span></span>
          </div>
          <article v-for="project in projects" :key="project.projectId" class="project-row">
            <div class="project-name">
              <strong>{{ project.name }}</strong>
              <small>{{ project.description || 'No description' }}</small>
            </div>
            <span>{{ formatDate(project.createdAt) }}</span>
            <div class="project-actions">
              <router-link :to="`/projects/${project.projectId}`" class="button secondary">
                Open
              </router-link>
              <button v-if="ability.can('delete', 'Project')" class="button ghost" @click="deleteProject(project.projectId)">Delete</button>
            </div>
          </article>
        </div>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useProjects, useCreateProject, useDeleteProject } from '../composables/useProjects'
import { useAppAbility } from '../permissions/ability'

const { data: projects, isLoading } = useProjects()
const createMutation = useCreateProject()
const deleteMutation = useDeleteProject()
const ability = useAppAbility()

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

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
.projects-layout {
  display: grid;
  grid-template-columns: minmax(0, 1fr);
  gap: 18px;
  align-items: start;
}

.projects-layout:has(.form-card) {
  grid-template-columns: 340px minmax(0, 1fr);
}

.form-card h2,
.project-list-card h2 {
  margin: 0 0 16px;
}

.form-card textarea {
  min-height: 120px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  gap: 14px;
  margin-bottom: 14px;
}

.section-header p {
  margin: 4px 0 0;
  color: #5e6c84;
}

.project-table {
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  overflow: hidden;
}

.project-table-header,
.project-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 140px 180px;
  align-items: center;
  gap: 14px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.project-table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.project-row:last-child {
  border-bottom: 0;
}

.project-row:hover {
  background: #f7f8f9;
}

.project-name {
  min-width: 0;
}

.project-name strong,
.project-name small {
  display: block;
}

.project-name small {
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.project-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

@media (max-width: 980px) {
  .projects-layout,
  .project-table-header,
  .project-row {
    grid-template-columns: 1fr;
  }

  .project-actions {
    justify-content: flex-start;
  }
}
</style>
