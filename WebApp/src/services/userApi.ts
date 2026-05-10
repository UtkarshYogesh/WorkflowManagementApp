import api from './api'

export const fetchUsers = () => api.get('/users')
export const updateUserRole = (userId: string, role: string) =>
  api.patch(`/users/${userId}`, JSON.stringify(role))
