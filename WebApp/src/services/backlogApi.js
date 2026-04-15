import api from "./api";

export const fetchBacklogs = (featureId) =>
  api.get(`/features/${featureId}/backlog-items`);

export const fetchBacklogById = (backlogId) =>
  api.get(`/backlog-items/${backlogId}`);

export const createBacklog = (featureId, data) =>
  api.post(`/features/${featureId}/backlog-items`, data);

export const deleteBacklog = (id) =>
  api.delete(`/backlog-items/${id}`);

export const updateBacklogStatus = (backlogId, newStatus) =>
  api.patch(`/backlog-items/${backlogId}/status`, newStatus);
