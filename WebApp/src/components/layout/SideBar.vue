<template>
  <aside class="sidebar">
    <div class="logo">
      <div class="icon">T</div>
      <div>
        <h2>TaskFlow</h2>
        <p>Project management</p>
      </div>
    </div>
    <nav>
      <router-link class="nav-link" to="/">Dashboard</router-link>
      <router-link class="nav-link" to="/projects">Projects</router-link>
      <router-link class="nav-link" to="/features">Features</router-link>
      <router-link class="nav-link" to="/backlogs">Backlogs</router-link>
    </nav>
    <div class="sidebar-footer">
      <div v-if="user" class="user-info">
        <p class="user-name">{{ user.username }}</p>
        <p class="user-email">{{ user.email }}</p>
      </div>
      <button @click="handleLogout" class="logout-btn">Logout</button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuth } from '../../composables/useAuth'

const { user, logout, isAuthenticated } = useAuth()
onMounted(() => {
  console.log('Current user:', user.value)
  console.log('Is authenticated:', isAuthenticated())
})

const handleLogout = () => {
  logout()
}
</script>

<style scoped>
.sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
  min-height: 100vh;
  padding: 28px 20px;
  background: linear-gradient(180deg, #0f172a 0%, #111827 100%);
  border-right: 1px solid rgba(148, 163, 184, 0.12);
}
.logo {
  display: flex;
  align-items: center;
  gap: 14px;
}
.icon {
  width: 42px;
  height: 42px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
  background: #2563eb;
  color: white;
  font-weight: 700;
}
nav {
  display: flex;
  flex-direction: column;
  gap: 14px;
  flex: 1;
}
.nav-link {
  padding: 12px 16px;
  border-radius: 14px;
  color: #cbd5e1;
  text-decoration: none;
  transition:
    background 0.2s ease,
    color 0.2s ease;
}
.nav-link.router-link-active,
.nav-link:hover {
  background: rgba(37, 99, 235, 0.14);
  color: #f8fafc;
}
h2 {
  margin: 0;
  color: #f8fafc;
}
p {
  margin: 4px 0 0;
  color: #94a3b8;
  font-size: 0.95rem;
}

.sidebar-footer {
  margin-top: auto;
  border-top: 1px solid rgba(148, 163, 184, 0.12);
  padding-top: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.user-info {
  padding: 12px;
  background: rgba(37, 99, 235, 0.1);
  border-radius: 8px;
  margin-bottom: 8px;
}

.user-name {
  margin: 0;
  color: #e2e8f0;
  font-weight: 600;
  font-size: 13px;
}

.user-email {
  margin: 4px 0 0;
  color: #94a3b8;
  font-size: 12px;
}

.logout-btn {
  padding: 10px 14px;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.3);
  color: #fca5a5;
  border-radius: 8px;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.logout-btn:hover {
  background: rgba(239, 68, 68, 0.2);
  border-color: rgba(239, 68, 68, 0.5);
}
</style>
