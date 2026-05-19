<template>
  <div class="grid min-h-screen grid-cols-[232px_minmax(0,1fr)] bg-[#f3f5f8] text-[#1f2328] max-[900px]:grid-cols-1">
    <Sidebar />
    <div class="min-w-0">
      <header class="sticky top-0 z-5 flex min-h-14 items-center justify-between gap-5 border-b border-slate-300 bg-white px-7 max-[900px]:static max-[900px]:min-h-0 max-[900px]:flex-wrap max-[900px]:px-4 max-[900px]:py-3">
        <div class="flex min-w-0 items-baseline gap-3">
          <p class="m-0 text-xs font-bold uppercase tracking-wide text-slate-600">TaskFlow</p>
          <h1 class="m-0 text-lg font-semibold text-slate-800">Boards</h1>
        </div>
        <div class="flex items-center gap-1.5 max-[900px]:w-full max-[900px]:overflow-x-auto">
          <router-link class="topbar-tab" to="/">Overview</router-link>
          <router-link class="topbar-tab" to="/projects">Projects</router-link>
          <router-link class="topbar-tab" to="/backlogs">Backlogs</router-link>
          <router-link v-if="ability.can('create', 'Project')" class="button primary" to="/projects">Create project</router-link>
        </div>
      </header>
      <main class="min-w-0 px-7 pb-10 pt-6 max-[900px]:px-4 max-[900px]:py-4">
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
@reference "../style.css";

:root {
  font-family:
    "Segoe UI", Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, sans-serif;
}

* {
  box-sizing: border-box;
}

body {
  @apply m-0 bg-[#f3f5f8];
}

button,
input,
textarea,
select {
  font: inherit;
}

.topbar-tab {
  @apply inline-flex min-h-8.5 items-center rounded px-2.5 text-[13px] font-semibold text-slate-600 no-underline;
}

.topbar-tab:hover,
.topbar-tab.router-link-active {
  @apply bg-blue-50 text-blue-700;
}

.page {
  @apply p-0;
}

.page-header {
  @apply mb-3.5 flex flex-wrap items-start justify-between gap-4;
}

.page-header h1,
.section-header h2,
.section-title h2 {
  @apply m-0 text-slate-800;
  letter-spacing: 0;
}

.page-header h1 {
  @apply text-2xl;
  font-weight: 650;
}

.subtitle {
  @apply mt-1.5 max-w-180 text-sm text-slate-600;
}

.eyebrow,
.breadcrumb {
  @apply mb-2 mt-0 text-xs font-bold uppercase tracking-wide text-slate-600;
}

.breadcrumb a {
  @apply text-blue-700 no-underline;
}

.page-actions,
.command-bar {
  @apply flex flex-wrap items-center gap-2;
}

.command-bar {
  @apply mb-3.5 min-h-12 rounded-md border border-slate-300 bg-white px-2.5 py-1.5;
}

.button,
.link-button {
  @apply inline-flex min-h-8 cursor-pointer items-center justify-center rounded border border-transparent bg-[#0078d4] px-3 text-[13px] font-semibold text-white no-underline;
}

.button:hover,
.link-button:hover {
  @apply bg-[#106ebe];
}

.button.secondary {
  @apply border-slate-300 bg-white text-slate-800;
}

.button.secondary:hover {
  @apply bg-slate-50;
}

.button.ghost {
  @apply border-slate-300 bg-transparent text-slate-600;
}

.button.ghost:hover {
  @apply bg-slate-50;
}

.button:disabled {
  @apply cursor-not-allowed opacity-55;
}

.cards-grid {
  @apply grid gap-3;
}

.panel,
.form-card,
.project-list-card,
.info-card,
.detail-card,
.card {
  @apply rounded-md border border-slate-300 bg-white shadow-none;
}

.form-card,
.project-list-card,
.info-card,
.detail-card,
.card {
  @apply p-5;
}

.form-card {
  @apply sticky top-18 max-[900px]:static;
}

.form-card h2 {
  @apply border-b border-slate-300 pb-3 text-base text-slate-800;
}

.empty-state {
  @apply rounded-md border border-dashed border-slate-300 bg-white p-6 text-center text-slate-600;
}

.status-pill,
.status {
  @apply inline-flex min-h-6 items-center justify-center rounded-full bg-blue-50 px-2.5 text-xs font-bold text-blue-800;
}

.form-card label,
.field {
  @apply mb-3.5 grid gap-1.5 text-[13px] font-bold text-slate-600;
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
  @apply w-full rounded border border-slate-300 bg-white px-2.5 py-2 text-slate-800;
}

textarea {
  resize: vertical;
}

select:focus,
input:focus,
textarea:focus {
  @apply border-[#0078d4] outline-2 outline-[#0078d438];
}

.data-table {
  @apply overflow-hidden rounded-md border border-slate-300 bg-white;
}

.table-header,
.table-row {
  @apply grid items-center border-b border-slate-300;
}

.table-header {
  @apply bg-slate-50 text-xs font-extrabold uppercase text-slate-600;
}

.table-row {
  @apply cursor-pointer text-slate-800;
}

.table-row:hover {
  @apply bg-slate-50;
}

.table-row:last-child {
  @apply border-b-0;
}

.header-cell,
.table-cell {
  @apply min-w-0 px-3.5 py-3;
}

.clickable-title {
  @apply cursor-pointer text-slate-800;
}

.clickable-title:hover {
  @apply text-blue-700;
}
</style>
