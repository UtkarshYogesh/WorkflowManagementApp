import api from "./api";

export const fetchFeatures = (projectId) =>
  api.get(`/projects/${projectId}/features`);

export const fetchFeatureById = (featureId) =>
  api.get(`/features/${featureId}`);

export const createFeature = (projectId, data) =>
  api.post(`/projects/${projectId}/features`, data);

export const deleteFeature = (featureId) => api.delete(`/features/${featureId}`);
export const updateFeatureStatus = (featureId, newStatus) =>
  api.patch(`/features/${featureId}/status`, newStatus);
