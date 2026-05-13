<template>
  <section class="page dashboard-page bg-slate-50">
    <div class="mb-5 flex flex-wrap items-start justify-between gap-4">
      <div>
        <p class="text-xs font-bold uppercase tracking-wide text-blue-700">Overview</p>
        <h1 class="mt-1 text-2xl font-bold text-slate-900">Analytics Dashboard</h1>
        <p class="mt-1 text-sm text-slate-500">Workload, priority, blocked items, and project health in one place.</p>
      </div>
      <div class="flex flex-wrap gap-2">
        <router-link class="rounded-md bg-blue-700 px-4 py-2 text-sm font-bold text-white hover:bg-blue-800" to="/projects">
          New project
        </router-link>
        <router-link class="rounded-md border border-slate-300 bg-white px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-100" to="/tasks">
          View tasks
        </router-link>
      </div>
    </div>

    <section class="mb-4 flex w-full items-end gap-2 overflow-x-auto rounded-lg border border-slate-200 bg-white p-3 shadow-sm">
      <label v-for="filter in filterConfig" :key="filter.key" class="min-w-40 flex-1 text-xs font-bold text-slate-500">
        {{ filter.label }}
        <select v-model="filters[filter.key]" class="mt-1 h-9 w-full rounded-md border border-slate-300 bg-white px-2 text-sm font-semibold text-slate-800">
          <option value="">{{ filter.allLabel }}</option>
          <option v-if="filter.key === 'userId'" value="unassigned">Unassigned</option>
          <option v-for="option in filter.options" :key="option.value" :value="option.value">
            {{ option.label }}
          </option>
        </select>
      </label>
      <button
        class="h-9 shrink-0 rounded-md border border-slate-300 px-3 text-sm font-bold text-slate-700 disabled:cursor-not-allowed disabled:opacity-40"
        :disabled="!hasActiveFilters"
        @click="resetFilters"
      >
        Reset
      </button>
    </section>

    <div v-if="isLoading" class="rounded-lg border border-slate-200 bg-white p-8 text-center text-slate-500">Loading analytics...</div>
    <template v-else>
      <div class="mb-4 grid grid-cols-2 gap-3 lg:grid-cols-5">
        <article v-for="stat in stats" :key="stat.label" class="rounded-lg border border-slate-200 bg-white p-4 shadow-sm">
          <div class="flex items-center justify-between gap-3">
            <span class="text-xs font-bold uppercase text-slate-500">{{ stat.label }}</span>
            <span class="rounded-full px-2 py-1 text-xs font-bold" :class="stat.badgeClass">{{ stat.badge }}</span>
          </div>
          <strong class="mt-3 block text-3xl font-bold text-slate-900">{{ stat.value }}</strong>
        </article>
      </div>

      <div class="mb-4 grid gap-4 xl:grid-cols-3">
        <AnalyticsCard title="Task status" subtitle="Current task movement">
          <PieChart :segments="taskStatusSegments" empty-label="No tasks" />
        </AnalyticsCard>
        <AnalyticsCard title="Backlog priority" subtitle="Urgency by backlog item">
          <PieChart :segments="backlogPrioritySegments" empty-label="No backlog" />
        </AnalyticsCard>
        <AnalyticsCard title="Backlog type" subtitle="Story, bug, improvement, technical">
          <PieChart :segments="backlogTypeSegments" empty-label="No backlog" />
        </AnalyticsCard>
      </div>

      <div class="mb-4 grid gap-4 xl:grid-cols-[1.15fr_0.85fr]">
        <AnalyticsCard title="Work by status" subtitle="Feature, backlog, and task totals">
          <div v-if="!statusSummary.length" class="empty-panel">No matching work found.</div>
          <div v-else class="grid gap-3">
            <div v-for="item in statusSummary" :key="item.label" class="grid grid-cols-[120px_1fr_42px] items-center gap-3 text-sm">
              <span class="truncate font-bold text-slate-700">{{ item.label }}</span>
              <div class="h-2 overflow-hidden rounded-full bg-slate-100">
                <span class="block h-full rounded-full" :style="{ width: `${item.percent}%`, background: item.color }"></span>
              </div>
              <strong class="text-right text-slate-900">{{ item.count }}</strong>
            </div>
          </div>
        </AnalyticsCard>

        <WorkList title="My assigned work" subtitle="Assigned to the signed-in user" :items="myWork" empty-label="No assigned work found." />
      </div>

      <div class="mb-4 grid gap-4 xl:grid-cols-[0.85fr_1.15fr]">
        <WorkList title="Blocked work" subtitle="Sorted by highest priority" :items="blockedWork" empty-label="No blocked work in this filter." />
        <WorkList title="Recent projects" subtitle="Latest project activity" :items="recentProjectItems" empty-label="No projects found." />
      </div>

      <AnalyticsCard title="Project health" subtitle="Progress, blocked work, and top open priority">
        <div v-if="!projectHealth.length" class="empty-panel">No project health data found.</div>
        <div v-else class="overflow-x-auto">
          <table class="min-w-[860px] w-full border-separate border-spacing-0 text-left text-sm">
            <thead>
              <tr class="text-xs uppercase text-slate-500">
                <th class="border-b border-slate-200 px-3 py-2">Project</th>
                <th class="border-b border-slate-200 px-3 py-2">Features</th>
                <th class="border-b border-slate-200 px-3 py-2">Backlog</th>
                <th class="border-b border-slate-200 px-3 py-2">Tasks</th>
                <th class="border-b border-slate-200 px-3 py-2">Done</th>
                <th class="border-b border-slate-200 px-3 py-2">Blocked</th>
                <th class="border-b border-slate-200 px-3 py-2">Top priority</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="project in projectHealth" :key="project.projectId" class="hover:bg-slate-50">
                <td class="border-b border-slate-100 px-3 py-3">
                  <router-link class="font-bold text-slate-900 hover:text-blue-700" :to="`/projects/${project.projectId}`">
                    {{ project.name }}
                  </router-link>
                </td>
                <td class="border-b border-slate-100 px-3 py-3">{{ project.featureCount }}</td>
                <td class="border-b border-slate-100 px-3 py-3">{{ project.backlogCount }}</td>
                <td class="border-b border-slate-100 px-3 py-3">{{ project.taskCount }}</td>
                <td class="border-b border-slate-100 px-3 py-3">
                  <div class="flex items-center gap-2">
                    <div class="h-2 w-20 overflow-hidden rounded-full bg-slate-100">
                      <span class="block h-full rounded-full bg-emerald-500" :style="{ width: `${project.donePercent}%` }"></span>
                    </div>
                    <span class="font-bold text-slate-700">{{ project.donePercent }}%</span>
                  </div>
                </td>
                <td class="border-b border-slate-100 px-3 py-3">
                  <span class="rounded-full bg-red-50 px-2 py-1 font-bold text-red-700">{{ project.blockedCount }}</span>
                </td>
                <td class="border-b border-slate-100 px-3 py-3">
                  <span v-if="project.topPriority" class="priority-chip" :class="priorityClass(project.topPriority)">
                    {{ project.topPriority }}
                  </span>
                  <span v-else class="text-slate-400">-</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </AnalyticsCard>
    </template>
  </section>
</template>

<script setup lang="ts">
import { computed, defineComponent, h, reactive } from 'vue'
import { getStoredUser } from '../services/authApi'
import { useBacklogs } from '../composables/useBacklogs'
import { useFeatures } from '../composables/useFeatures'
import { useProjects } from '../composables/useProjects'
import { useTasks } from '../composables/useTasks'
import { useUsers } from '../composables/useUsers'

type FilterKey = 'projectId' | 'userId' | 'status' | 'priority' | 'type'
type Segment = { label: string; count: number; percent: number; color: string }
type WorkItem = { key: string; title: string; meta: string; status?: string; priority?: string; link: string }

const chartColors = ['#2563eb', '#059669', '#eab308', '#dc2626', '#7c3aed', '#0891b2']
const priorityRank: Record<string, number> = { P1: 1, P2: 2, P3: 3 }

const AnalyticsCard = defineComponent({
  props: {
    title: { type: String, required: true },
    subtitle: { type: String, required: true },
  },
  setup(props, { slots }) {
    return () =>
      h('section', { class: 'rounded-lg border border-slate-200 bg-white p-4 shadow-sm' }, [
        h('div', { class: 'mb-4' }, [
          h('h2', { class: 'text-base font-bold text-slate-900' }, props.title),
          h('p', { class: 'mt-0.5 text-sm text-slate-500' }, props.subtitle),
        ]),
        slots.default?.(),
      ])
  },
})

const PieChart = defineComponent({
  props: {
    segments: { type: Array as () => Segment[], required: true },
    emptyLabel: { type: String, default: 'No data' },
  },
  setup(props) {
    return () => {
      const visibleSegments = props.segments.filter((segment) => segment.count > 0)
      if (!visibleSegments.length) {
        return h('div', { class: 'empty-panel' }, props.emptyLabel)
      }

      let start = 0
      const gradient = visibleSegments
        .map((segment) => {
          const end = start + segment.percent
          const stop = `${segment.color} ${start}% ${end}%`
          start = end
          return stop
        })
        .join(', ')

      return h('div', { class: 'grid items-center gap-4 sm:grid-cols-[128px_1fr]' }, [
        h('div', {
          class: 'pie-chart justify-self-center',
          style: { background: `conic-gradient(${gradient})` },
          'aria-label': 'Pie chart',
        }),
        h(
          'div',
          { class: 'grid gap-2' },
          visibleSegments.map((segment) =>
            h('div', { class: 'grid grid-cols-[12px_1fr_auto] items-center gap-2 text-sm', key: segment.label }, [
              h('span', { class: 'h-2.5 w-2.5 rounded-full', style: { background: segment.color } }),
              h('span', { class: 'truncate font-semibold text-slate-700' }, segment.label),
              h('strong', { class: 'text-slate-900' }, `${segment.count} (${segment.percent}%)`),
            ]),
          ),
        ),
      ])
    }
  },
})

const WorkList = defineComponent({
  props: {
    title: { type: String, required: true },
    subtitle: { type: String, required: true },
    items: { type: Array as () => WorkItem[], required: true },
    emptyLabel: { type: String, required: true },
  },
  setup(props) {
    return () =>
      h('section', { class: 'rounded-lg border border-slate-200 bg-white p-4 shadow-sm' }, [
        h('div', { class: 'mb-4 flex items-start justify-between gap-3' }, [
          h('div', [h('h2', { class: 'text-base font-bold text-slate-900' }, props.title), h('p', { class: 'mt-0.5 text-sm text-slate-500' }, props.subtitle)]),
          h('span', { class: 'rounded-full bg-slate-100 px-2 py-1 text-xs font-bold text-slate-600' }, props.items.length),
        ]),
        props.items.length
          ? h(
              'div',
              { class: 'divide-y divide-slate-100 overflow-hidden rounded-md border border-slate-200' },
              props.items.map((item) =>
                h(
                  'a',
                  {
                    key: item.key,
                    href: item.link,
                    class: 'grid grid-cols-[1fr_auto] items-center gap-3 bg-white px-3 py-2.5 text-sm no-underline hover:bg-slate-50',
                  },
                  [
                    h('span', { class: 'min-w-0' }, [
                      h('strong', { class: 'block truncate text-slate-900' }, item.title),
                      h('small', { class: 'mt-0.5 block truncate text-xs font-semibold text-slate-500' }, item.meta),
                    ]),
                    item.priority
                      ? h('span', { class: ['priority-chip', priorityClass(item.priority)] }, item.priority)
                      : h('span', { class: 'rounded-full bg-blue-50 px-2 py-1 text-xs font-bold text-blue-700' }, item.status || 'Open'),
                  ],
                ),
              ),
            )
          : h('div', { class: 'empty-panel' }, props.emptyLabel),
      ])
  },
})

const { data: projects, isLoading: isProjectsLoading } = useProjects()
const { data: features, isLoading: isFeaturesLoading } = useFeatures()
const { data: backlogs, isLoading: isBacklogsLoading } = useBacklogs()
const { data: tasks, isLoading: isTasksLoading } = useTasks()
const { data: users, isLoading: isUsersLoading } = useUsers()

const currentUserId = computed(() => getStoredUser()?.userId || getStoredUser()?.id || '')
const filters = reactive<Record<FilterKey, string>>({
  projectId: '',
  userId: '',
  status: '',
  priority: '',
  type: '',
})

const allProjects = computed(() => projects.value || [])
const allFeatures = computed(() => features.value || [])
const allBacklogs = computed(() => backlogs.value || [])
const allTasks = computed(() => tasks.value || [])

const isLoading = computed(
  () => isProjectsLoading.value || isFeaturesLoading.value || isBacklogsLoading.value || isTasksLoading.value || isUsersLoading.value,
)

const featureById = computed(() => new Map(allFeatures.value.map((feature: any) => [feature.id, feature])))
const backlogById = computed(() => new Map(allBacklogs.value.map((backlog: any) => [backlog.id, backlog])))

const selectedProjectFeatureIds = computed(() => {
  if (!filters.projectId) return new Set(allFeatures.value.map((feature: any) => feature.id))
  return new Set(allFeatures.value.filter((feature: any) => feature.projectId === filters.projectId).map((feature: any) => feature.id))
})

const selectedProjectBacklogIds = computed(() => {
  const featureIds = selectedProjectFeatureIds.value
  return new Set(allBacklogs.value.filter((backlog: any) => featureIds.has(backlog.featureId)).map((backlog: any) => backlog.id))
})

const matchesUser = (item: any) => {
  if (!filters.userId) return true
  if (filters.userId === 'unassigned') return !item.assignedToUserId
  return item.assignedToUserId === filters.userId
}
const matchesStatus = (item: any) => !filters.status || item.status === filters.status
const matchesPriority = (item: any) => !filters.priority || item.priority === filters.priority

const filteredProjects = computed(() => allProjects.value.filter((project: any) => !filters.projectId || project.projectId === filters.projectId))
const filteredFeatures = computed(() =>
  allFeatures.value.filter(
    (feature: any) => selectedProjectFeatureIds.value.has(feature.id) && matchesUser(feature) && matchesStatus(feature) && matchesPriority(feature) && !filters.type,
  ),
)
const filteredBacklogs = computed(() =>
  allBacklogs.value.filter(
    (backlog: any) =>
      selectedProjectFeatureIds.value.has(backlog.featureId) &&
      matchesUser(backlog) &&
      matchesStatus(backlog) &&
      matchesPriority(backlog) &&
      (!filters.type || backlog.type === filters.type),
  ),
)
const filteredTasks = computed(() =>
  allTasks.value.filter((task: any) => {
    const backlog = backlogById.value.get(task.backlogItemId) as any
    return (
      selectedProjectBacklogIds.value.has(task.backlogItemId) &&
      matchesUser(task) &&
      matchesStatus(task) &&
      (!filters.priority || (backlog?.priority || 'P3') === filters.priority) &&
      (!filters.type || backlog?.type === filters.type)
    )
  }),
)

const statusOptions = computed(() => {
  const statuses = new Set<string>()
  ;[...allProjects.value, ...allFeatures.value, ...allBacklogs.value, ...allTasks.value].forEach((item: any) => item.status && statuses.add(item.status))
  return [...statuses].sort()
})
const priorityOptions = computed(() => {
  const priorities = new Set<string>()
  ;[...allFeatures.value, ...allBacklogs.value].forEach((item: any) => item.priority && priorities.add(item.priority))
  return [...priorities].sort((a, b) => (priorityRank[a] || 99) - (priorityRank[b] || 99))
})
const typeOptions = computed(() => {
  const types = new Set<string>()
  allBacklogs.value.forEach((backlog: any) => backlog.type && types.add(backlog.type))
  return [...types].sort()
})

const filterConfig = computed(() => [
  { key: 'projectId' as FilterKey, label: 'Project', allLabel: 'All projects', options: allProjects.value.map((project: any) => ({ value: project.projectId, label: project.name })) },
  { key: 'userId' as FilterKey, label: 'User', allLabel: 'All users', options: (users.value || []).map((user: any) => ({ value: user.userId, label: user.username })) },
  { key: 'status' as FilterKey, label: 'Status', allLabel: 'All statuses', options: statusOptions.value.map((status) => ({ value: status, label: status })) },
  { key: 'priority' as FilterKey, label: 'Priority', allLabel: 'All priorities', options: priorityOptions.value.map((priority) => ({ value: priority, label: priority })) },
  { key: 'type' as FilterKey, label: 'Type', allLabel: 'All types', options: typeOptions.value.map((type) => ({ value: type, label: type })) },
])

const hasActiveFilters = computed(() => Object.values(filters).some(Boolean))
const resetFilters = () => {
  filters.projectId = ''
  filters.userId = ''
  filters.status = ''
  filters.priority = ''
  filters.type = ''
}

const activeProjectCount = computed(() => filteredProjects.value.filter((project: any) => !['Done', 'Completed', 'Archived'].includes(project.status)).length)
const p1FeatureCount = computed(() => filteredFeatures.value.filter((feature: any) => feature.priority === 'P1').length)
const p1BacklogCount = computed(() => filteredBacklogs.value.filter((backlog: any) => backlog.priority === 'P1').length)
const completedTaskPercent = computed(() => {
  if (!filteredTasks.value.length) return 0
  return Math.round((filteredTasks.value.filter((task: any) => task.status === 'Done').length / filteredTasks.value.length) * 100)
})

const stats = computed(() => [
  { label: 'Projects', value: filteredProjects.value.length, badge: `${activeProjectCount.value} active`, badgeClass: 'bg-blue-50 text-blue-700' },
  { label: 'Features', value: filteredFeatures.value.length, badge: `${p1FeatureCount.value} P1`, badgeClass: 'bg-purple-50 text-purple-700' },
  { label: 'Backlog', value: filteredBacklogs.value.length, badge: `${p1BacklogCount.value} P1`, badgeClass: 'bg-amber-50 text-amber-700' },
  { label: 'Blocked', value: blockedWork.value.length, badge: `${blockedHighPriorityCount.value} urgent`, badgeClass: 'bg-red-50 text-red-700' },
  { label: 'Tasks', value: filteredTasks.value.length, badge: `${completedTaskPercent.value}% done`, badgeClass: 'bg-emerald-50 text-emerald-700' },
])

const summarize = (items: any[], field: string): Segment[] => {
  const counts = items.reduce((acc: Record<string, number>, item: any) => {
    const value = item[field] || 'Unknown'
    acc[value] = (acc[value] || 0) + 1
    return acc
  }, {})
  const total = items.length
  return Object.entries(counts).map(([label, count], index) => ({
    label,
    count,
    percent: total ? Math.round((count / total) * 100) : 0,
    color: chartColors[index % chartColors.length] ?? '#2563eb',
  }))
}

const taskStatusSegments = computed(() => summarize(filteredTasks.value, 'status'))
const backlogPrioritySegments = computed(() => summarize(filteredBacklogs.value, 'priority'))
const backlogTypeSegments = computed(() => summarize(filteredBacklogs.value, 'type'))
const statusSummary = computed(() => summarize([...filteredFeatures.value, ...filteredBacklogs.value, ...filteredTasks.value], 'status'))

const priorityClass = (priority: string) => {
  if (priority === 'P1') return 'bg-red-50 text-red-700'
  if (priority === 'P2') return 'bg-amber-50 text-amber-700'
  return 'bg-emerald-50 text-emerald-700'
}

const getProjectName = (projectId: string) => allProjects.value.find((project: any) => project.projectId === projectId)?.name || 'Project'
const formatDate = (value: string) => new Date(value).toLocaleDateString()

const toWorkItem = (item: any, kind: 'Feature' | 'Backlog' | 'Task'): WorkItem => {
  if (kind === 'Feature') {
    return { key: `feature-${item.id}`, title: item.name, status: item.status, priority: item.priority, link: `/features/${item.id}`, meta: `Feature | ${getProjectName(item.projectId)}` }
  }
  if (kind === 'Backlog') {
    const feature = featureById.value.get(item.featureId) as any
    return { key: `backlog-${item.id}`, title: item.title, status: item.status, priority: item.priority, link: `/backlogs/${item.id}`, meta: `Backlog | ${feature?.name || 'Feature'} | ${item.type || 'Work item'}` }
  }
  const backlog = backlogById.value.get(item.backlogItemId) as any
  return { key: `task-${item.id}`, title: item.title, status: item.status, priority: backlog?.priority, link: `/tasks/${item.id}`, meta: `Task | ${backlog?.title || 'Backlog item'}` }
}

const myWork = computed(() => {
  if (!currentUserId.value) return []
  return [
    ...filteredFeatures.value.filter((feature: any) => feature.assignedToUserId === currentUserId.value).map((feature: any) => toWorkItem(feature, 'Feature')),
    ...filteredBacklogs.value.filter((backlog: any) => backlog.assignedToUserId === currentUserId.value).map((backlog: any) => toWorkItem(backlog, 'Backlog')),
    ...filteredTasks.value.filter((task: any) => task.assignedToUserId === currentUserId.value).map((task: any) => toWorkItem(task, 'Task')),
  ].slice(0, 6)
})

const blockedWork = computed(() =>
  [
    ...filteredFeatures.value.filter((feature: any) => feature.status === 'Blocked').map((feature: any) => toWorkItem(feature, 'Feature')),
    ...filteredBacklogs.value.filter((backlog: any) => backlog.status === 'Blocked').map((backlog: any) => toWorkItem(backlog, 'Backlog')),
    ...filteredTasks.value.filter((task: any) => task.status === 'Blocked').map((task: any) => toWorkItem(task, 'Task')),
  ]
    .sort((a, b) => (priorityRank[a.priority || 'P3'] || 99) - (priorityRank[b.priority || 'P3'] || 99))
    .slice(0, 6),
)
const blockedHighPriorityCount = computed(() => blockedWork.value.filter((item) => ['P1', 'P2'].includes(item.priority || '')).length)

const recentProjects = computed(() => [...filteredProjects.value].sort((a: any, b: any) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()).slice(0, 6))
const recentProjectItems = computed<WorkItem[]>(() =>
  recentProjects.value.map((project: any) => ({
    key: `project-${project.projectId}`,
    title: project.name,
    meta: `${project.description || 'No description'} | ${formatDate(project.createdAt)}`,
    status: project.status,
    link: `/projects/${project.projectId}`,
  })),
)

const projectHealth = computed(() =>
  filteredProjects.value.map((project: any) => {
    const projectFeatures = allFeatures.value.filter((feature: any) => feature.projectId === project.projectId)
    const featureIds = new Set(projectFeatures.map((feature: any) => feature.id))
    const projectBacklogs = allBacklogs.value.filter((backlog: any) => featureIds.has(backlog.featureId))
    const backlogIds = new Set(projectBacklogs.map((backlog: any) => backlog.id))
    const projectTasks = allTasks.value.filter((task: any) => backlogIds.has(task.backlogItemId))
    const topPriority = [...projectFeatures, ...projectBacklogs]
      .map((item: any) => item.priority)
      .filter(Boolean)
      .sort((a: string, b: string) => (priorityRank[a] || 99) - (priorityRank[b] || 99))[0]

    return {
      projectId: project.projectId,
      name: project.name,
      featureCount: projectFeatures.length,
      backlogCount: projectBacklogs.length,
      taskCount: projectTasks.length,
      donePercent: projectTasks.length ? Math.round((projectTasks.filter((task: any) => task.status === 'Done').length / projectTasks.length) * 100) : 0,
      blockedCount:
        projectFeatures.filter((feature: any) => feature.status === 'Blocked').length +
        projectBacklogs.filter((backlog: any) => backlog.status === 'Blocked').length +
        projectTasks.filter((task: any) => task.status === 'Blocked').length,
      topPriority,
    }
  }),
)
</script>

<style scoped>
.pie-chart {
  width: 128px;
  height: 128px;
  border-radius: 999px;
  box-shadow: inset 0 0 0 18px #ffffff, 0 0 0 1px #e2e8f0;
}

.empty-panel {
  display: grid;
  min-height: 112px;
  place-items: center;
  border: 1px dashed #cbd5e1;
  border-radius: 8px;
  color: #64748b;
  font-size: 14px;
  font-weight: 600;
}

.priority-chip {
  display: inline-flex;
  min-height: 24px;
  align-items: center;
  justify-content: center;
  border-radius: 999px;
  padding: 0 10px;
  font-size: 12px;
  font-weight: 800;
  white-space: nowrap;
}
</style>
