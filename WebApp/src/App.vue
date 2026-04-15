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
