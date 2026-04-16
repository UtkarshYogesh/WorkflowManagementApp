<template>
  <section class="page features">
    <div class="page-header">
      <h1>Features</h1>
      <p class="subtitle">Manage all features across projects.</p>
    </div>

    <div class="list-panel">
      <div v-if="isFeaturesLoading" class="empty-state">Loading features...</div>
      <div v-else-if="features?.length === 0" class="empty-state">
        No features yet. Create one in a project.
      </div>
      <div v-else class="cards-grid">
        <article v-for="feature in features" :key="feature.id" class="card">
          <div class="card-header">
            <h3>{{ feature.name }}</h3>
            <span class="status-pill">{{ feature.status }}</span>
          </div>
          <p>{{ feature.description }}</p>
          <div class="card-actions">
            <router-link :to="`/features/${feature.id}`">View backlogs</router-link>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useFeatures } from '../composables/useFeatures'

const { data: features, isLoading: isFeaturesLoading } = useFeatures()
</script>
