import api from "./api";

export const fetchFeatures = (projectId: string) =>
  api.get(`/projects/${projectId}/features`);

export const fetchAllFeatures = () =>
  api.get(`/features`);

export const fetchFeatureById = (featureId: string) =>
  api.get(`/features/${featureId}`);

export const createFeature = (projectId: string, data: any) =>
  api.post(`/projects/${projectId}/features`, data);

export const deleteFeature = (featureId: string) => api.delete(`/features/${featureId}`);
export const updateFeatureStatus = (featureId: string, newStatus: any) =>
  api.patch(`/features/${featureId}/status`, newStatus);
