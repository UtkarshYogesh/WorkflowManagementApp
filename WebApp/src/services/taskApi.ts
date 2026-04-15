import api from "./api";

export const fetchTasks = (backlogId: string) => api.get(`/backlog-items/${backlogId}/tasks`);
export const createTask = (backlogId: string, data: any) => api.post(`/backlog-items/${backlogId}/tasks`, data);
export const deleteTask = (taskId: string) => api.delete(`/tasks/${taskId}`);
export const updateTaskStatus = (taskId: string, newStatus: any) => api.patch(`/tasks/${taskId}/status`, newStatus);
export const fetchTaskById = (taskId: string) => api.get(`/tasks/${taskId}`);
