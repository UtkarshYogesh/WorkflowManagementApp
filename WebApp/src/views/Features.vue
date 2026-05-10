<template>
  <section class="page features">
    <div class="page-header">
      <div>
        <p class="eyebrow">Planning</p>
        <h1>Features</h1>
        <p class="subtitle">Track feature work and the backlog items underneath each feature.</p>
      </div>
      <div class="page-actions">
        <button class="button secondary" @click="viewMode = viewMode === 'board' ? 'list' : 'board'">
          {{ viewMode === 'board' ? 'List view' : 'Board view' }}
        </button>
      </div>
    </div>

    <div v-if="viewMode === 'list'" class="panel">
      <div v-if="isFeaturesLoading" class="empty-state">Loading features...</div>
      <div v-else-if="!features?.length" class="empty-state">No features yet. Create one in a project.</div>
      <div v-else class="feature-list">
        <article v-for="feature in features" :key="feature.id" class="feature-row">
          <div class="feature-main">
            <button class="toggle-button" @click="toggleFeature(feature.id)">
              {{ expandedFeatures.includes(feature.id) ? '-' : '+' }}
            </button>
            <div>
              <h3 @click="navigateToFeature(feature.id)" class="clickable-title">{{ feature.name }}</h3>
              <p>{{ feature.description || 'No description' }}</p>
            </div>
          </div>
          <span class="status-pill">{{ feature.status }}</span>
          <span class="muted">{{ getBacklogsForFeature(feature.id).length }} backlog items</span>

          <div v-if="expandedFeatures.includes(feature.id)" class="nested-list">
            <div v-if="getBacklogsForFeature(feature.id).length === 0" class="nested-empty">
              No backlog items yet.
            </div>
            <button
              v-for="backlog in getBacklogsForFeature(feature.id)"
              :key="backlog.id"
              class="nested-row"
              @click="navigateToBacklog(backlog.id)"
            >
              <span>{{ backlog.title }}</span>
              <span class="status-pill">{{ backlog.status }}</span>
            </button>
          </div>
        </article>
      </div>
    </div>

    <div v-else class="board-panel">
      <div v-if="isFeaturesLoading || isBacklogsLoading" class="empty-state">Loading board...</div>
      <div v-else-if="!features?.length" class="empty-state">No features yet. Create one in a project.</div>
      <div v-else class="feature-board">
        <section v-for="status in backlogStatuses" :key="status" class="board-column">
          <header>
            <h2>{{ status }}</h2>
            <span>{{ getBacklogsByStatus(status).length }}</span>
          </header>
          <article
            v-for="backlog in getBacklogsByStatus(status)"
            :key="backlog.id"
            class="work-card"
            @click="navigateToBacklog(backlog.id)"
          >
            <strong>{{ backlog.title }}</strong>
            <p>{{ backlog.description || 'No description' }}</p>
            <small>{{ getFeatureName(backlog.featureId) }}</small>
          </article>
          <div v-if="getBacklogsByStatus(status).length === 0" class="empty-column">No items</div>
        </section>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useFeatures } from '../composables/useFeatures'
import { useBacklogs } from '../composables/useBacklogs'

const router = useRouter()
const { data: features, isLoading: isFeaturesLoading } = useFeatures()
const { data: allBacklogs, isLoading: isBacklogsLoading } = useBacklogs()

const viewMode = ref<'list' | 'board'>('list')
const expandedFeatures = ref<string[]>([])

const backlogStatuses = computed(() => {
  const baseStatuses = ['Planned', 'Committed', 'Done']
  const statuses = Array.isArray(allBacklogs.value)
    ? allBacklogs.value.map((backlog: any) => backlog.status)
    : []
  return [...baseStatuses, ...new Set(statuses.filter((status: string) => status && !baseStatuses.includes(status)))]
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

const getBacklogsByStatus = (status: string) => {
  return allBacklogs.value?.filter((backlog: any) => backlog.status === status) || []
}

const getFeatureName = (featureId: string) => {
  return features.value?.find((feature: any) => feature.id === featureId)?.name || 'Feature'
}

const navigateToFeature = (featureId: string) => {
  router.push(`/features/${featureId}`)
}

const navigateToBacklog = (backlogId: string) => {
  router.push(`/backlogs/${backlogId}`)
}
</script>

<style scoped>
.panel,
.board-panel {
  padding: 18px;
  overflow: hidden;
}

.feature-list {
  display: grid;
  gap: 10px;
}

.feature-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) max-content 140px;
  gap: 14px;
  align-items: center;
  padding: 14px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
}

.feature-main {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  min-width: 0;
}

.feature-main > div {
  min-width: 0;
}

.feature-main h3 {
  margin: 0;
  font-size: 15px;
}

.feature-main p {
  margin: 4px 0 0;
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.toggle-button {
  width: 28px;
  height: 28px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #f7f8f9;
  color: #44546f;
  cursor: pointer;
}

.muted {
  color: #5e6c84;
  font-size: 13px;
  justify-self: end;
  white-space: nowrap;
}

.nested-list {
  grid-column: 1 / -1;
  display: grid;
  gap: 6px;
  padding: 10px 0 0 40px;
}

.nested-row {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  align-items: center;
  min-height: 36px;
  padding: 8px 10px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #f7f8f9;
  color: #172b4d;
  text-align: left;
  cursor: pointer;
  min-width: 0;
}

.nested-row span:first-child {
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.nested-empty,
.empty-column {
  color: #5e6c84;
  font-size: 13px;
}

.feature-board {
  display: flex;
  gap: 14px;
  overflow-x: auto;
  padding-bottom: 8px;
}

.board-column {
  flex: 0 0 300px;
  min-height: 420px;
  min-width: 0;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #f7f8f9;
}

.board-column header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.board-column h2 {
  margin: 0;
  font-size: 14px;
  color: #172b4d;
}

.board-column header span {
  color: #5e6c84;
  font-weight: 700;
}

.work-card {
  display: grid;
  gap: 6px;
  margin-bottom: 10px;
  padding: 12px;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
  background: #ffffff;
  cursor: pointer;
  min-width: 0;
}

.work-card:hover {
  border-color: #0c66e4;
}

.work-card p,
.work-card small {
  margin: 0;
  color: #5e6c84;
  overflow-wrap: anywhere;
}

@media (max-width: 900px) {
  .feature-row {
    grid-template-columns: 1fr;
  }

  .muted {
    justify-self: start;
  }

  .nested-list {
    padding-left: 0;
  }

  .board-column {
    flex-basis: 280px;
  }
}
</style>
