<template>
  <section class="page features">
    <div class="page-header">
      <div>
        <h1>Features</h1>
        <p class="subtitle">Manage all features across projects.</p>
      </div>
      <div class="page-actions">
        <button
          class="button secondary"
          @click="viewMode = viewMode === 'kanban' ? 'list' : 'kanban'"
        >
          {{ viewMode === 'kanban' ? 'List View' : 'Kanban View' }}
        </button>
      </div>
    </div>

    <div v-if="viewMode === 'list'" class="list-panel">
      <div v-if="isFeaturesLoading" class="empty-state">Loading features...</div>
      <div v-else-if="features?.length === 0" class="empty-state">
        No features yet. Create one in a project.
      </div>
      <div v-else class="features-list">
        <article v-for="feature in features" :key="feature.id" class="feature-item">
          <div class="feature-header" @click="toggleFeature(feature.id)">
            <div class="feature-info">
              <h3 @click.stop="navigateToFeature(feature.id)" class="clickable-title">
                {{ feature.name }}
              </h3>
              <span class="status-pill">{{ feature.status }}</span>
            </div>
            <div class="dropdown-icon">
              <span :class="{ rotated: expandedFeatures.includes(feature.id) }">▼</span>
            </div>
          </div>
          <p class="feature-description">{{ feature.description }}</p>

          <div v-if="expandedFeatures.includes(feature.id)" class="backlogs-section">
            <div v-if="getBacklogsForFeature(feature.id).length === 0" class="no-backlogs">
              No backlogs yet.
            </div>
            <div v-else class="backlogs-list">
              <div
                v-for="backlog in getBacklogsForFeature(feature.id)"
                :key="backlog.id"
                class="backlog-item"
              >
                <span
                  @click="navigateToBacklog(backlog.id)"
                  class="backlog-title clickable-title"
                  >{{ backlog.title }}</span
                >
                <span class="backlog-status">{{ backlog.status }}</span>
              </div>
            </div>
          </div>
        </article>
      </div>
    </div>

    <div v-else class="kanban-section">
      <h2>Feature Kanban</h2>
      <div v-if="isFeaturesLoading || isBacklogsLoading" class="empty-state">Loading...</div>
      <div v-else-if="!features?.length" class="empty-state">
        No features yet. Create one in a project.
      </div>
      <div v-else class="kanban-table">
        <!-- Table Header -->
        <div class="table-header">
          <div class="header-cell feature-header">Feature</div>
          <div class="header-cell" v-for="status in backlogStatuses" :key="status">
            {{ status }}
          </div>
        </div>

        <!-- Table Rows -->
        <div class="table-row" v-for="feature in features" :key="feature.id">
          <!-- Feature Column -->
          <div class="table-cell feature-cell">
            <div class="feature-info">
              <h4 @click="navigateToFeature(feature.id)" class="clickable-title">
                {{ feature.name }}
              </h4>
              <span class="feature-status">{{ feature.status }}</span>
              <p class="feature-description">{{ feature.description }}</p>
            </div>
          </div>

          <!-- Status Columns -->
          <div class="table-cell backlogs-cell" v-for="status in backlogStatuses" :key="status">
            <div class="backlogs-container">
              <div
                v-for="backlog in getBacklogsForFeatureAndStatus(feature.id, status)"
                :key="backlog.id"
                class="backlog-card"
              >
                <div class="backlog-top">
                  <h5 @click="navigateToBacklog(backlog.id)" class="clickable-title">
                    {{ backlog.title }}
                  </h5>
                </div>
                <p>{{ backlog.description }}</p>
              </div>
              <div
                v-if="getBacklogsForFeatureAndStatus(feature.id, status).length === 0"
                class="empty-cell"
              >
                No backlogs
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useFeatures } from '../composables/useFeatures'
import { useBacklogs } from '../composables/useBacklogs'

const router = useRouter()
const { data: features, isLoading: isFeaturesLoading } = useFeatures()
const { data: allBacklogs, isLoading: isBacklogsLoading } = useBacklogs()

const viewMode = ref<'list' | 'kanban'>('list')
const expandedFeatures = ref<string[]>([])

const backlogStatuses = computed(() => {
  const baseStatuses = ['Planned', 'Committed', 'Done']
  const backlogStatuses = Array.isArray(allBacklogs.value)
    ? allBacklogs.value.map((backlog: any) => backlog.status)
    : []
  const extraStatuses = [
    ...new Set(
      backlogStatuses.filter((status: string) => status && !baseStatuses.includes(status)),
    ),
  ]
  return [...baseStatuses, ...extraStatuses]
})

const toggleFeature = (featureId: string) => {
  const index = expandedFeatures.value.indexOf(featureId)
  if (index > -1) {
    expandedFeatures.value.splice(index, 1)
  } else {
    expandedFeatures.value.push(featureId)
  }
}

const getBacklogsForFeature = (featureId: string) => {
  return allBacklogs.value?.filter((backlog: any) => backlog.featureId === featureId) || []
}

const getBacklogsForFeatureAndStatus = (featureId: string, status: string) => {
  return (
    allBacklogs.value?.filter(
      (backlog: any) => backlog.featureId === featureId && backlog.status === status,
    ) || []
  )
}

const navigateToFeature = (featureId: string) => {
  router.push(`/features/${featureId}`)
}

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
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

.page-header h1 {
  color: #f8fafc;
  margin: 0 0 8px 0;
}

.subtitle {
  color: #cbd5e1;
  margin: 0;
}

.page-actions {
  display: flex;
  gap: 12px;
}

.button {
  border: none;
  border-radius: 12px;
  padding: 10px 16px;
  color: #f8fafc;
  background: #3b82f6;
  cursor: pointer;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.button:hover {
  background: #2563eb;
}

.button.secondary {
  background: #475569;
}

.button.secondary:hover {
  background: #334155;
}

.empty-state {
  padding: 32px;
  border-radius: 16px;
  background: #111827;
  color: #94a3b8;
  text-align: center;
}

.features-list {
  display: grid;
  gap: 16px;
}

.feature-item {
  padding: 20px;
  border-radius: 18px;
  background: #0f172a;
  border: 1px solid rgba(148, 163, 184, 0.12);
  box-shadow: 0 20px 40px rgba(15, 23, 42, 0.08);
}

.feature-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  padding: 12px 0;
}

.feature-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.feature-info h3 {
  margin: 0;
  color: #f8fafc;
}

.feature-description {
  color: #cbd5e1;
  margin: 8px 0 16px 0;
}

.dropdown-icon {
  transition: transform 0.2s ease;
}

.dropdown-icon .rotated {
  transform: rotate(180deg);
}

.backlogs-section {
  border-top: 1px solid rgba(148, 163, 184, 0.12);
  padding-top: 16px;
}

.no-backlogs {
  color: #94a3b8;
  font-style: italic;
}

.backlogs-list {
  display: grid;
  gap: 8px;
}

.backlog-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  background: rgba(148, 163, 184, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(148, 163, 184, 0.08);
}

.backlog-title {
  color: #f8fafc;
  font-weight: 500;
}

.backlog-status {
  color: #bfdbfe;
  background: rgba(59, 130, 246, 0.14);
  padding: 4px 8px;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 600;
}

.status-pill {
  display: inline-block;
  padding: 4px 10px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 600;
}

.kanban-section {
  margin-top: 12px;
}

.kanban-section h2 {
  color: #f8fafc;
  margin-bottom: 20px;
}

.kanban-table {
  display: table;
  width: 100%;
  border-collapse: collapse;
  background: #0f172a;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid rgba(148, 163, 184, 0.12);
}

.table-header {
  display: table-row;
  background: rgba(148, 163, 184, 0.05);
  border-bottom: 1px solid rgba(148, 163, 184, 0.12);
}

.table-row {
  display: table-row;
  border-bottom: 1px solid rgba(148, 163, 184, 0.08);
}

.table-row:last-child {
  border-bottom: none;
}

.header-cell,
.table-cell {
  display: table-cell;
  padding: 16px;
  vertical-align: top;
  border-right: 1px solid rgba(148, 163, 184, 0.08);
}

.header-cell:last-child,
.table-cell:last-child {
  border-right: none;
}

.feature-header {
  width: 250px;
  font-weight: 600;
  color: #f8fafc;
}

.header-cell {
  text-align: center;
  font-weight: 600;
  color: #cbd5e1;
  background: rgba(59, 130, 246, 0.08);
}

.feature-cell {
  width: 250px;
  background: rgba(148, 163, 184, 0.02);
}

.backlogs-cell {
  width: 200px;
  min-height: 120px;
}

.feature-info h4 {
  margin: 0 0 8px 0;
  color: #f8fafc;
  font-size: 1rem;
  font-weight: 600;
}

.feature-status {
  display: inline-block;
  padding: 4px 10px;
  background: rgba(59, 130, 246, 0.14);
  color: #bfdbfe;
  border-radius: 999px;
  font-size: 0.8rem;
  font-weight: 600;
  margin-bottom: 8px;
}

.feature-description {
  color: #94a3b8;
  font-size: 0.9rem;
  margin: 0;
  line-height: 1.4;
}

.backlogs-container {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-height: 80px;
}

.backlog-card {
  padding: 12px;
  border-radius: 8px;
  background: #1e293b;
  border: 1px solid rgba(148, 163, 184, 0.16);
}

.backlog-card h5 {
  margin: 0 0 6px 0;
  color: #f8fafc;
  font-size: 0.9rem;
  font-weight: 600;
}

.backlog-card p {
  margin: 0;
  color: #cbd5e1;
  font-size: 0.8rem;
  line-height: 1.3;
}

.empty-cell {
  color: #64748b;
  font-size: 0.8rem;
  font-style: italic;
  text-align: center;
  padding: 20px;
}

.clickable-title {
  cursor: pointer;
  transition: color 0.2s ease;
}

.clickable-title:hover {
  color: #38bdf8;
}

/* Responsive adjustments */
@media (max-width: 1200px) {
  .feature-cell,
  .feature-header {
    width: 200px;
  }

  .backlogs-cell {
    width: 150px;
  }
}

@media (max-width: 768px) {
  .kanban-table {
    font-size: 0.9rem;
  }

  .header-cell,
  .table-cell {
    padding: 12px 8px;
  }

  .feature-cell,
  .feature-header {
    width: 150px;
  }

  .backlogs-cell {
    width: 120px;
  }
}
</style>
