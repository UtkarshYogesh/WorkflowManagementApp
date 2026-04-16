<template>
  <section class="page backlogs">
    <div class="page-header">
      <h1>Backlogs</h1>
      <p class="subtitle">Manage all backlog items across features.</p>
    </div>

    <div class="list-panel">
      <div v-if="isBacklogsLoading" class="empty-state">Loading backlogs...</div>
      <div v-else-if="backlogs?.length === 0" class="empty-state">
        No backlogs yet. Create one in a feature.
      </div>
      <div v-else class="cards-grid">
        <article v-for="backlog in backlogs" :key="backlog.id" class="card">
          <div class="card-header">
            <h3>{{ backlog.title }}</h3>
            <span class="status-pill">{{ backlog.status }}</span>
          </div>
          <p>{{ backlog.description }}</p>
          <div class="card-actions">
            <router-link :to="`/backlogs/${backlog.id}`">View tasks</router-link>
          </div>
        </article>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useBacklogs } from '../composables/useBacklogs'

const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs()
</script>
