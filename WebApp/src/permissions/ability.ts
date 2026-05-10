import { AbilityBuilder, createMongoAbility, subject, type MongoAbility } from '@casl/ability'
import { useAbility } from '@casl/vue'
import { getStoredUser } from '../services/authApi'

export type AppAction = 'manage' | 'read' | 'create' | 'update' | 'delete'
export type AppSubject = 'all' | 'Project' | 'Feature' | 'Backlog' | 'Task' | 'User'
export type AppAbility = MongoAbility<[AppAction, AppSubject]>

export type AbilityUser = {
  id?: string
  userId?: string
  role?: string
}

const normalize = (value: unknown) => String(value ?? '').trim().toLowerCase()

export const getUserId = (user: AbilityUser | null | undefined) => user?.id ?? user?.userId

export function defineAbilityFor(user: AbilityUser | null | undefined): AppAbility {
  const { can, build } = new AbilityBuilder<AppAbility>(createMongoAbility)
  const userId = getUserId(user)

  if (normalize(user?.role) === 'admin') {
    can('manage', 'all')
    return build()
  }

  can('read', 'User')
  can('read', 'Project')
  can(['read', 'create', 'update'], 'Feature')
  can(['read', 'create', 'update'], 'Backlog')
  can(['read', 'create', 'update'], 'Task')

  if (userId) {
    can('delete', 'Feature', { createdByUserId: userId } as any)
    can('delete', 'Backlog', { createdByUserId: userId } as any)
    can('delete', 'Task', { createdByUserId: userId } as any)
  }

  return build()
}

export const ability = defineAbilityFor(getStoredUser())

export function updateAbilityFor(user: AbilityUser | null | undefined) {
  ability.update(defineAbilityFor(user).rules)
}

export const useAppAbility = () => useAbility<AppAbility>()

const withCreatorAlias = (item: any) => ({
  ...item,
  createdByUserId:
    item?.createdByUserId ??
    item?.createdById ??
    item?.createdBy ??
    item?.creatorId ??
    item?.createdByUser?.userId ??
    item?.createdByUser?.id,
})

export const asSubject = (type: Exclude<AppSubject, 'all'>, item: any) => subject(type, withCreatorAlias(item))
