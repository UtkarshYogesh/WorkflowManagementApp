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

    <div class="command-bar">
      <span class="command-title">{{ projects?.length ?? 0 }} projects</span>
      <span class="command-muted">Open a project to manage features, backlog, and tasks.</span>
      <button v-if="ability.can('create', 'Project')" class="button primary ml-auto" @click="showCreateForm = !showCreateForm">
        {{ showCreateForm ? 'Close form' : 'Create project' }}
      </button>
    </div>

    <div class="projects-layout" :class="{ 'form-open': showCreateForm }">
      <aside v-if="ability.can('create', 'Project') && showCreateForm" class="form-card">
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
            <span>Status</span>
            <span>Created</span>
            <span></span>
          </div>
          <article v-for="project in projects" :key="project.projectId" class="project-row">
            <div class="project-name">
              <strong>{{ project.name }}</strong>
              <small>{{ project.description || 'No description' }}</small>
            </div>
            <span class="status-pill">{{ project.status || 'New' }}</span>
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

const showCreateForm = ref(false)
const name = ref('')
const description = ref('')

const createProject = async () => {
  if (!name.value) return
  await createMutation.mutateAsync({ name: name.value, description: description.value })
  name.value = ''
  description.value = ''
  showCreateForm.value = false
}

const deleteProject = async (projectId: string) => {
  await deleteMutation.mutateAsync(projectId)
}

const formatDate = (value: string) => new Date(value).toLocaleDateString()
</script>

<style scoped>
@reference "../style.css";

.projects-layout {
  @apply grid grid-cols-1 items-start gap-3.5;
}

.projects-layout.form-open {
  @apply grid-cols-[320px_minmax(0,1fr)] max-[980px]:grid-cols-1;
}

.command-title {
  @apply text-sm font-bold text-slate-800;
}

.command-muted {
  @apply text-[13px] text-slate-600;
}

.form-card h2,
.project-list-card h2 {
  @apply mb-4 mt-0;
}

.form-card textarea {
  @apply min-h-30;
}

.section-header {
  @apply mb-3.5 flex justify-between gap-3.5;
}

.section-header p {
  @apply mt-1 mb-0 text-slate-600;
}

.project-table {
  @apply overflow-hidden rounded-md border border-slate-300;
}

.project-table-header,
.project-row {
  @apply grid grid-cols-[minmax(0,1fr)_150px_130px_150px] items-center gap-3 border-b border-slate-300 px-3 py-2.5 max-[980px]:grid-cols-1;
}

.project-table-header {
  @apply bg-slate-50 text-xs font-extrabold uppercase text-slate-600;
}

.project-row:last-child {
  @apply border-b-0;
}

.project-row:hover {
  @apply bg-slate-50;
}

.project-name {
  @apply min-w-0;
}

.project-name strong,
.project-name small {
  @apply block;
}

.project-name small {
  @apply truncate text-slate-600;
}

.project-actions {
  @apply flex justify-end gap-2 max-[980px]:justify-start;
}
</style>
