import { useMutation, useQuery, useQueryClient } from '@tanstack/vue-query'
import { fetchUsers, updateUserRole } from '../services/userApi'

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

export function useUpdateUserRole() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ userId, role }: { userId: string; role: string }) => updateUserRole(userId, role),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['users'] })
    },
  })
}
