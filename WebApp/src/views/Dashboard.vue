<template>
  <section class="page dashboard-page">
    <div class="hero-panel">
      <div>
        <p class="eyebrow">Project operations</p>
        <h1>Manage projects, features, backlogs, and tasks.</h1>
        <p class="hero-copy">
          Create your next project with Azure DevOps-style hierarchy and move tasks through a
          lightweight kanban board.
        </p>
      </div>
      <router-link class="button primary" to="/projects">Go to Projects</router-link>
    </div>

    <div class="stats-grid">
      <article class="stat-card">
        <span>Projects</span>
        <strong>{{ projects?.length ?? 0 }}</strong>
      </article>
      <article class="stat-card">
        <span>Features</span>
        <strong>{{ featureCount }}</strong>
      </article>
      <article class="stat-card">
        <span>Backlog items</span>
        <strong>{{ backlogCount }}</strong>
      </article>
    </div>

    <section class="projects-section">
      <div class="section-title">
        <h2>Active projects</h2>
        <p>Open a project to define features and plan backlog items.</p>
      </div>
      <div v-if="isLoading" class="empty-state">Loading projects…</div>
      <div v-else-if="!projects?.length" class="empty-state">
        No projects found. Create one on the Projects page.
      </div>
      <div class="cards-grid" v-else>
        <article class="project-card" v-for="project in projects" :key="project.projectId">
          <div>
            <h3>{{ project.name }}</h3>
            <p>{{ project.description || 'No description yet.' }}</p>
          </div>
          <div class="project-meta">
            <span>{{ new Date(project.createdAt).toLocaleDateString() }}</span>
            <router-link class="link-button" :to="`/projects/${project.projectId}`"
              >Open</router-link
            >
          </div>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useProjects } from '../composables/useProjects'

const { data: projects, isLoading } = useProjects()
const featureCount = computed(() => 0)
const backlogCount = computed(() => 0)
</script>

<style scoped>
.page {
  padding: 24px 0;
}
.hero-panel {
  display: flex;
  justify-content: space-between;
  gap: 20px;
  padding: 28px;
  border-radius: 22px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  border: 1px solid rgba(148, 163, 184, 0.14);
  margin-bottom: 28px;
}
.eyebrow {
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.14em;
  color: #60a5fa;
  margin-bottom: 12px;
}
.hero-copy {
  color: #cbd5e1;
  max-width: 680px;
  margin-top: 12px;
}
.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
  margin-bottom: 28px;
}
.stat-card {
  padding: 22px;
  border-radius: 18px;
  background: #111827;
  border: 1px solid rgba(148, 163, 184, 0.12);
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.stat-card span {
  color: #94a3b8;
}
.stat-card strong {
  font-size: 2.2rem;
}
.projects-section {
  padding: 0;
}
.section-title {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 16px;
  margin-bottom: 18px;
}
.project-card {
  padding: 22px;
  border-radius: 18px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.12);
  display: flex;
  justify-content: space-between;
  gap: 20px;
  align-items: center;
}
.project-card h3 {
  margin: 0;
}
.project-meta {
  display: flex;
  flex-direction: column;
  gap: 10px;
  text-align: right;
}
.link-button {
  color: #38bdf8;
  text-decoration: none;
}
.empty-state {
  padding: 28px;
  border-radius: 18px;
  background: #111827;
  color: #94a3b8;
}
.button.primary {
  align-self: flex-start;
}
</style>
