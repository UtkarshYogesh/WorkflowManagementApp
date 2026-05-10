<template>
  <div class="app-shell">
    <Sidebar />
    <div class="workspace">
      <header class="topbar">
        <div>
          <p class="topbar-kicker">TaskFlow workspace</p>
          <h1>Plan, track, and ship work</h1>
        </div>
        <div class="topbar-actions">
          <router-link v-if="ability.can('create', 'Project')" class="button primary" to="/projects">Create project</router-link>
        </div>
      </header>
      <main class="app-main">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import Sidebar from '@/components/layout/SideBar.vue'
import { useAppAbility } from '@/permissions/ability'

const ability = useAppAbility()
</script>

<style>
:root {
  color: #172b4d;
  background: #f6f8fb;
  font-family:
    Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
}

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  background: #f6f8fb;
}

button,
input,
textarea,
select {
  font: inherit;
}

.app-shell {
  display: grid;
  grid-template-columns: 260px minmax(0, 1fr);
  min-height: 100vh;
  background: #f6f8fb;
  color: #172b4d;
}

.workspace {
  min-width: 0;
}

.topbar {
  position: sticky;
  top: 0;
  z-index: 5;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
  min-height: 76px;
  padding: 16px 32px;
  background: rgba(255, 255, 255, 0.92);
  border-bottom: 1px solid #dfe1e6;
  backdrop-filter: blur(12px);
}

.topbar h1 {
  margin: 2px 0 0;
  color: #172b4d;
  font-size: 20px;
  font-weight: 700;
  letter-spacing: 0;
}

.topbar-kicker {
  margin: 0;
  color: #5e6c84;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.topbar-actions {
  display: flex;
  align-items: center;
  gap: 10px;
}

.app-main {
  min-width: 0;
  padding: 28px 32px 48px;
}

.page {
  padding: 0;
}

.page-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 22px;
}

.page-header h1,
.section-header h2,
.section-title h2 {
  margin: 0;
  color: #172b4d;
  letter-spacing: 0;
}

.page-header h1 {
  font-size: 28px;
}

.subtitle {
  color: #5e6c84;
  margin: 6px 0 0;
}

.eyebrow,
.breadcrumb {
  margin: 0 0 8px;
  color: #5e6c84;
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.breadcrumb a {
  color: #0c66e4;
  text-decoration: none;
}

.button,
.link-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 36px;
  padding: 0 14px;
  border-radius: 6px;
  border: 1px solid transparent;
  background: #0c66e4;
  color: #ffffff;
  font-weight: 700;
  font-size: 14px;
  text-decoration: none;
  cursor: pointer;
}

.button:hover,
.link-button:hover {
  background: #0055cc;
}

.button.secondary {
  background: #f1f2f4;
  color: #172b4d;
  border-color: #dfe1e6;
}

.button.secondary:hover {
  background: #dfe1e6;
}

.button.ghost {
  background: transparent;
  color: #44546f;
  border-color: #dfe1e6;
}

.button.ghost:hover {
  background: #f1f2f4;
}

.button:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.cards-grid {
  display: grid;
  gap: 12px;
}

.panel,
.form-card,
.project-list-card,
.info-card,
.detail-card,
.card {
  background: #ffffff;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  box-shadow: 0 1px 2px rgba(9, 30, 66, 0.08);
}

.form-card,
.project-list-card,
.info-card,
.detail-card,
.card {
  padding: 20px;
}

.empty-state {
  padding: 24px;
  border: 1px dashed #c1c7d0;
  border-radius: 8px;
  background: #ffffff;
  color: #5e6c84;
  text-align: center;
}

.status-pill,
.status {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 24px;
  padding: 0 10px;
  border-radius: 999px;
  background: #e9f2ff;
  color: #0c66e4;
  font-size: 12px;
  font-weight: 700;
}

.form-card label,
.field {
  display: grid;
  gap: 7px;
  margin-bottom: 14px;
  color: #44546f;
  font-size: 13px;
  font-weight: 700;
}

.form-card input,
.form-card textarea,
.form-card select,
.field input,
.field textarea,
.field select,
select,
input,
textarea {
  width: 100%;
  border-radius: 6px;
  border: 1px solid #c1c7d0;
  background: #ffffff;
  color: #172b4d;
  padding: 9px 11px;
}

textarea {
  resize: vertical;
}

select:focus,
input:focus,
textarea:focus {
  outline: 2px solid rgba(12, 102, 228, 0.28);
  border-color: #0c66e4;
}

.data-table {
  overflow: hidden;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.table-header,
.table-row {
  display: grid;
  align-items: center;
  border-bottom: 1px solid #dfe1e6;
}

.table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.table-row {
  color: #172b4d;
  cursor: pointer;
}

.table-row:hover {
  background: #f7f8f9;
}

.table-row:last-child {
  border-bottom: 0;
}

.header-cell,
.table-cell {
  min-width: 0;
  padding: 12px 14px;
}

.clickable-title {
  color: #172b4d;
  cursor: pointer;
}

.clickable-title:hover {
  color: #0c66e4;
}

@media (max-width: 900px) {
  .app-shell {
    grid-template-columns: 1fr;
  }

  .topbar {
    position: static;
    padding: 16px 20px;
  }

  .app-main {
    padding: 20px;
  }
}
</style>
