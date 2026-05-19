import api from "./api";

export const fetchTasks = (backlogId: string) => api.get(`/backlog-items/${backlogId}/tasks`);
export const fetchAllTasks = () => api.get(`/tasks`);
export const createTask = (backlogId: string, data: any) => api.post(`/backlog-items/${backlogId}/tasks`, data);
export const updateTask = (taskId: string, data: any) => api.put(`/tasks/${taskId}`, data);
export const deleteTask = (taskId: string) => api.delete(`/tasks/${taskId}`);
export const updateTaskStatus = (taskId: string, newStatus: any) => api.patch(`/tasks/${taskId}/status`, newStatus);
export const fetchTaskById = (taskId: string) => api.get(`/tasks/${taskId}`);
export const assignTaskToUser = (taskId: string, userId: string) => api.patch(`/tasks/${taskId}/assign/${userId}`);
