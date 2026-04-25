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
  </aside>
</template>

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
</style>
