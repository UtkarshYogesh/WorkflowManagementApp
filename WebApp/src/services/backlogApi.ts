import api from "./api";

export const fetchBacklogs = (featureId: string) =>
  api.get(`/features/${featureId}/backlog-items`);

export const fetchBacklogById = (backlogId: string) =>
  api.get(`/backlog-items/${backlogId}`);

export const createBacklog = (featureId: string, data: any) =>
  api.post(`/features/${featureId}/backlog-items`, data);

export const deleteBacklog = (id: string) =>
  api.delete(`/backlog-items/${id}`);

export const updateBacklogStatus = (backlogId: string, newStatus: any) =>
  api.patch(`/backlog-items/${backlogId}/status`, newStatus);
