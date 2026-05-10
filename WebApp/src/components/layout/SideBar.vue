<template>
  <aside class="sidebar">
    <router-link class="brand" to="/">
      <span class="brand-mark">TF</span>
      <span>
        <strong>TaskFlow</strong>
        <small>Delivery workspace</small>
      </span>
    </router-link>

    <nav class="nav">
      <router-link class="nav-link" to="/" exact-active-class="router-link-active">
        <span class="nav-icon">#</span>
        Dashboard
      </router-link>
      <router-link class="nav-link" to="/projects">
        <span class="nav-icon">P</span>
        Projects
      </router-link>
      <router-link class="nav-link" to="/features">
        <span class="nav-icon">F</span>
        Features
      </router-link>
      <router-link class="nav-link" to="/backlogs">
        <span class="nav-icon">B</span>
        Backlog
      </router-link>
    </nav>

    <div class="sidebar-footer">
      <div v-if="user" class="user-info">
        <span class="avatar">{{ userInitials }}</span>
        <span>
          <strong>{{ user.username }}</strong>
          <small>{{ user.email }}</small>
        </span>
      </div>
      <button @click="handleLogout" class="logout-btn">Logout</button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuth } from '../../composables/useAuth'

const { user, logout } = useAuth()

const userInitials = computed(() => {
  const name = user.value?.username || 'User'
  return name.slice(0, 2).toUpperCase()
})

const handleLogout = () => {
  logout()
}
</script>

<style scoped>
.sidebar {
  position: sticky;
  top: 0;
  display: flex;
  flex-direction: column;
  gap: 22px;
  height: 100vh;
  padding: 18px 14px;
  background: #ffffff;
  border-right: 1px solid #dfe1e6;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 6px 8px 14px;
  border-bottom: 1px solid #dfe1e6;
  color: #172b4d;
  text-decoration: none;
}

.brand-mark {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  background: #0c66e4;
  color: #ffffff;
  font-size: 13px;
  font-weight: 800;
}

.brand strong,
.user-info strong {
  display: block;
  color: #172b4d;
  font-size: 14px;
}

.brand small,
.user-info small {
  display: block;
  max-width: 160px;
  overflow: hidden;
  color: #5e6c84;
  font-size: 12px;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.nav {
  display: grid;
  gap: 4px;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 10px;
  min-height: 38px;
  padding: 0 10px;
  border-radius: 6px;
  color: #44546f;
  font-size: 14px;
  font-weight: 700;
  text-decoration: none;
}

.nav-link:hover,
.nav-link.router-link-active {
  background: #e9f2ff;
  color: #0c66e4;
}

.nav-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 22px;
  height: 22px;
  border-radius: 5px;
  background: #f1f2f4;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
}

.nav-link.router-link-active .nav-icon {
  background: #0c66e4;
  color: #ffffff;
}

.sidebar-footer {
  display: grid;
  gap: 12px;
  margin-top: auto;
  padding-top: 14px;
  border-top: 1px solid #dfe1e6;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 10px;
}

.avatar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 34px;
  height: 34px;
  border-radius: 50%;
  background: #fef3c7;
  color: #92400e;
  font-size: 12px;
  font-weight: 800;
}

.logout-btn {
  min-height: 34px;
  border: 1px solid #dfe1e6;
  border-radius: 6px;
  background: #ffffff;
  color: #44546f;
  font-weight: 700;
  cursor: pointer;
}

.logout-btn:hover {
  background: #f1f2f4;
}

@media (max-width: 900px) {
  .sidebar {
    position: static;
    height: auto;
  }

  .nav {
    grid-template-columns: repeat(4, minmax(0, 1fr));
  }

  .nav-link {
    justify-content: center;
    font-size: 12px;
  }

  .nav-icon,
  .brand small {
    display: none;
  }
}
</style>
