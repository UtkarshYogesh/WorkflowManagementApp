import { useQuery } from '@tanstack/vue-query'
import { fetchUsers } from '../services/userApi'

export type AppUser = {
  userId: string
  username: string
  email: string
  role: string
}

export function useUsers() {
  return useQuery<AppUser[]>({
    queryKey: ['users'],
    queryFn: async () => (await fetchUsers()).data,
  })
}
