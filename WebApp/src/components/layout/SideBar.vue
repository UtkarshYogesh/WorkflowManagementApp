<template>
  <aside class="sticky top-0 flex h-screen flex-col gap-3.5 border-r border-slate-950 bg-[#10213b] px-2.5 py-3.5 text-slate-200 max-[900px]:static max-[900px]:h-auto">
    <router-link class="flex items-center gap-3 border-b border-white/10 px-2.5 pb-4 pt-2 text-white no-underline" to="/">
      <span class="inline-flex h-9 w-9 items-center justify-center rounded bg-[#0078d4] text-lg font-extrabold text-white">T</span>
      <span>
        <strong class="block text-sm text-white">TaskFlow</strong>
        <small class="block max-w-40 truncate text-xs text-slate-400 max-[900px]:hidden">Project service</small>
      </span>
    </router-link>

    <nav class="grid gap-0.5 pt-1.5 max-[900px]:grid-cols-5">
      <span class="px-2.5 pb-1.5 pt-2 text-[11px] font-extrabold uppercase tracking-wide text-slate-400 max-[900px]:hidden">Boards</span>
      <router-link class="nav-link" to="/" exact-active-class="router-link-active">
        <span class="nav-icon">D</span>
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
      <router-link class="nav-link" to="/tasks">
        <span class="nav-icon">T</span>
        Tasks
      </router-link>
    </nav>

    <div class="mt-auto grid gap-3 border-t border-white/10 pt-3.5 max-[900px]:hidden">
      <router-link v-if="ability.can('manage', 'User')" class="nav-link" to="/settings/users">
        <span class="nav-icon">U</span>
        User settings
      </router-link>
      <div v-if="user" class="flex items-center gap-2.5">
        <span class="inline-flex h-8 w-8 items-center justify-center rounded-full bg-slate-700 text-xs font-extrabold text-white">{{ userInitials }}</span>
        <span>
          <strong class="block text-sm text-white">{{ user.username }}</strong>
          <small class="block max-w-40 truncate text-xs text-slate-400">{{ user.email }}</small>
        </span>
      </div>
      <button class="min-h-8 cursor-pointer rounded border border-white/15 bg-transparent font-bold text-slate-200 hover:bg-white/10 hover:text-white" @click="handleLogout">
        Logout
      </button>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuth } from '../../composables/useAuth'
import { useAppAbility } from '../../permissions/ability'

const { user, logout } = useAuth()
const ability = useAppAbility()

const userInitials = computed(() => {
  const name = user.value?.username || 'User'
  return name.slice(0, 2).toUpperCase()
})

const handleLogout = () => {
  logout()
}
</script>

<style scoped>
@reference "../../style.css";

.nav-link {
  @apply flex min-h-9 items-center gap-2.5 rounded border-l-[3px] border-transparent px-2.5 text-sm font-semibold text-slate-200 no-underline max-[900px]:justify-center max-[900px]:text-xs;
}

.nav-link:hover,
.nav-link.router-link-active {
  @apply border-l-sky-300 bg-white/10 text-white;
}

.nav-icon {
  @apply inline-flex h-5 w-5 items-center justify-center rounded text-xs font-extrabold text-slate-400 max-[900px]:hidden;
}

.nav-link.router-link-active .nav-icon {
  @apply text-white;
}
</style>
