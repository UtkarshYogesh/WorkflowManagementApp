<template>
  <div class="app-shell">
    <Sidebar />
    <main class="app-main">
      <router-view />
    </main>
  </div>
</template>

<script setup lang="ts">
import Sidebar from './components/layout/SideBar.vue'
</script>

<style scoped>
.app-shell {
  display: grid;
  grid-template-columns: 240px 1fr;
  min-height: 100vh;
  background: #090b13;
  color: #e2e8f0;
}

.app-main {
  padding: 24px;
  overflow-x: hidden;
}

* {
  box-sizing: border-box;
}
</style>

<style>
.page {
  padding: 28px 32px;
}

.page-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 28px;
}

.subtitle {
  color: #cbd5e1;
  margin-top: 8px;
}

.cards-grid {
  display: grid;
  gap: 16px;
}

.list-panel,
.form-panel,
.page-actions {
  margin-top: 12px;
}

.page-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
}

.card {
  padding: 20px;
  border-radius: 18px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.12);
  box-shadow: 0 20px 40px rgba(15, 23, 42, 0.08);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.card-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  align-items: center;
  margin-top: 18px;
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

.empty-state {
  padding: 28px;
  border-radius: 20px;
  background: #111827;
  color: #94a3b8;
}

.button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-height: 44px;
  padding: 0 18px;
  border-radius: 14px;
  border: 1px solid transparent;
  background: linear-gradient(135deg, #2563eb 0%, #0ea5e9 100%);
  color: #ffffff;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  transition:
    transform 0.18s ease,
    box-shadow 0.18s ease,
    background 0.18s ease;
}

.button:hover {
  transform: translateY(-1px);
  box-shadow: 0 12px 30px rgba(14, 165, 233, 0.24);
}

.button:disabled {
  opacity: 0.55;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.button.secondary {
  background: rgba(255, 255, 255, 0.08);
  color: #dbeafe;
  border-color: rgba(148, 163, 184, 0.14);
}

.button.ghost {
  background: transparent;
  color: #f8fafc;
  border-color: rgba(148, 163, 184, 0.16);
}

.link-button {
  color: #38bdf8;
  text-decoration: none;
  font-weight: 600;
}

.link-button:hover {
  text-decoration: underline;
}
</style>
