<template>
  <section class="page user-settings">
    <div class="page-header">
      <div>
        <p class="eyebrow">Admin</p>
        <h1>User settings</h1>
        <p class="subtitle">Manage user roles for the workspace.</p>
      </div>
    </div>

    <section class="panel">
      <div v-if="isLoading" class="empty-state">Loading users...</div>
      <div v-else-if="!users?.length" class="empty-state">No users found.</div>
      <div v-else class="user-table">
        <div class="user-table-header">
          <span>User</span>
          <span>Role</span>
          <span></span>
        </div>

        <article v-for="item in users" :key="item.userId" class="user-row">
          <div class="user-main">
            <span class="avatar">{{ initials(item.username) }}</span>
            <div>
              <strong>{{ item.username }}</strong>
              <small>{{ item.email }}</small>
            </div>
          </div>
          <span class="status-pill">{{ item.role || 'User' }}</span>
          <button
            class="button secondary"
            :disabled="isAdminUser(item) || updateRoleMutation.isPending.value"
            @click="makeAdmin(item.userId)"
          >
            {{ isAdminUser(item) ? 'Admin' : 'Make admin' }}
          </button>
        </article>
      </div>
    </section>
  </section>
</template>

<script setup lang="ts">
import { useUpdateUserRole, useUsers, type AppUser } from '../composables/useUsers'

const { data: users, isLoading } = useUsers()
const updateRoleMutation = useUpdateUserRole()

const initials = (name: string) => (name || 'User').slice(0, 2).toUpperCase()
const isAdminUser = (user: AppUser) => String(user.role || '').toLowerCase() === 'admin'

const makeAdmin = async (userId: string) => {
  await updateRoleMutation.mutateAsync({ userId, role: 'Admin' })
}
</script>

<style scoped>
.panel {
  padding: 18px;
  overflow: hidden;
}

.user-table {
  overflow: hidden;
  border: 1px solid #dfe1e6;
  border-radius: 8px;
}

.user-table-header,
.user-row {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 140px 140px;
  align-items: center;
  gap: 14px;
  padding: 12px 14px;
  border-bottom: 1px solid #dfe1e6;
}

.user-table-header {
  background: #f7f8f9;
  color: #44546f;
  font-size: 12px;
  font-weight: 800;
  text-transform: uppercase;
}

.user-row:last-child {
  border-bottom: 0;
}

.user-row:hover {
  background: #f7f8f9;
}

.user-main {
  display: flex;
  align-items: center;
  gap: 10px;
  min-width: 0;
}

.user-main > div {
  min-width: 0;
}

.user-main strong,
.user-main small {
  display: block;
}

.user-main small {
  overflow: hidden;
  color: #5e6c84;
  text-overflow: ellipsis;
  white-space: nowrap;
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

@media (max-width: 900px) {
  .user-table-header,
  .user-row {
    grid-template-columns: 1fr;
  }
}
</style>
